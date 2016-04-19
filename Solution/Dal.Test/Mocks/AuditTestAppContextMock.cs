using Dal.Audit;

namespace Dal.Test.Mocks
{
    internal class AuditTestAppContextMock:IAppContext
    {
        public string UserName => "TestUser";
    }
}