using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Dal.Audit
{
    /// <summary>
    /// Class LoggingProxy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Runtime.Remoting.Proxies.RealProxy" />
    public class LoggingProxy<T> : RealProxy
    {
        private readonly T _instance;

        private LoggingProxy(T instance)
            : base(typeof(T))
        {
            _instance = instance;
        }

        /// <summary>
        /// Creates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>T.</returns>
        public static T Create(T instance)
        {
            return (T)new LoggingProxy<T>(instance).GetTransparentProxy();
        }

        /// <summary>
        /// When overridden in a derived class, invokes the method that is specified in the provided <see cref="T:System.Runtime.Remoting.Messaging.IMessage" /> on the remote object that is represented by the current instance.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Runtime.Remoting.Messaging.IMessage" /> that contains a <see cref="T:System.Collections.IDictionary" /> of information about the method call.</param>
        /// <returns>The message returned by the invoked method, containing the return value and any out or ref parameters.</returns>
        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = (IMethodCallMessage)msg;
            var method = (MethodInfo)methodCall.MethodBase;

            try
            {
                Console.WriteLine("Before invoke: " + method.Name);
                var result = method.Invoke(_instance, methodCall.InArgs);
                Console.WriteLine("After invoke: " + method.Name);
                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                if (e is TargetInvocationException && e.InnerException != null)
                {
                    return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
                }

                return new ReturnMessage(e, msg as IMethodCallMessage);
            }
        }
    }
}
