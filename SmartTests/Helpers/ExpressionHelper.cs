using System;
using System.Linq.Expressions;
using System.Reflection;



namespace SmartTests.Helpers
{
    static class ExpressionHelper
    {
        public static MemberInfo GetMember( this Expression @this )
        {
            switch( @this.NodeType )
            {
                case ExpressionType.New:
                    return ( (NewExpression)@this ).Constructor;

                case ExpressionType.MemberAccess:
                    return ( (MemberExpression)@this ).Member;

                default:
                    throw new NotImplementedException();
            }
        }


        public static MemberInfo GetMember<T>( this Expression<Func<T>> @this ) => @this.Body.GetMember();
    }
}