using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Dal.Audit
{
    public static class Helpers
    {
        public static MemberExpression GetMemberExpression(LambdaExpression expression)
        {
            if (expression == null) return null;

            if (expression.Body is UnaryExpression)
            {
                return ((expression.Body as UnaryExpression).Operand as MemberExpression);
            }
            else if (expression.Body is MemberExpression)
            {
                return expression.Body as MemberExpression;
            }
            return null;
        }


        // code adjusted to prevent horizontal overflow
        public static string GetFullPropertyName<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        // code adjusted to prevent horizontal overflow
        private static bool TryFindMemberExpression
        (Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked
            );
        }


        public static object GetDbPropertyValue(this DbPropertyValues dbValues, string propertyPath)
        {
            var path = propertyPath.Split('.');
            var currentPropertyValues = dbValues;

            for(var i=0;i<path.Count()-1;i++)
            {
                currentPropertyValues = currentPropertyValues[path[i]] as DbPropertyValues;
            }
            return currentPropertyValues[path.Last()];
        }
    }


}