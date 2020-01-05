using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global


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

                case ExpressionType.Call:
                    return ( (MethodCallExpression)@this ).Method;

                default:
                    throw new NotImplementedException();
            }
        }


        public static MemberInfo GetMember<T>( this Expression<Func<T>> @this ) => @this.Body.GetMember();


        public static object GetInstance( this Expression @this )
        {
            var closureExpression = @this as MemberExpression;
            var closure = ( closureExpression?.Expression as ConstantExpression )?.Value;
            return ( closureExpression?.Member as FieldInfo )?.GetValue( closure );
        }


        public static bool GetMemberContext<T>( this Expression<Func<T>> @this,
                                                out object instance,
                                                out MemberInfo member )
        {
            if( @this.Body is MemberExpression memberExpression )
            {
                instance = memberExpression.Expression.GetInstance();
                member = memberExpression.Member;
                Debug.Assert( member != null );
                return true;
            }

            if( @this.Body is MethodCallExpression methodCall )
            {
                instance = methodCall.Object.GetInstance();
                member = methodCall.Method;
                return true;
            }

            if( @this.Body is NewExpression newExpression )
            {
                instance = null;
                member = newExpression.Constructor;
                return true;
            }

            instance = null;
            member = null;
            return false;
        }


        public static bool GetMemberContext<T>( this Expression<Func<T>> @this,
                                                out object instance,
                                                out MemberInfo member,
                                                out Expression[] arguments )
        {
            if( @this.Body is MemberExpression memberExpression )
            {
                instance = memberExpression.Expression.GetInstance();
                member = memberExpression.Member;
                arguments = null;
                Debug.Assert( member != null );
                return true;
            }

            if( @this.Body is MethodCallExpression methodCall )
            {
                instance = methodCall.Object.GetInstance();
                var method = methodCall.Method;
                member = method;
                arguments = methodCall.Arguments.ToArray();
                return true;
            }

            instance = null;
            member = null;
            arguments = null;
            return false;
        }


        public static bool GetMemberContext( this LambdaExpression @this,
                                             out object instance, out MemberInfo member )
        {
            if( @this.Body is MemberExpression memberExpression )
            {
                instance = memberExpression.Expression.GetInstance();
                member = memberExpression.Member;
                Debug.Assert( member != null );
                return true;
            }

            if( @this.Body is MethodCallExpression methodCall )
            {
                member = methodCall.Method;
                instance = methodCall.Object.GetInstance();
                return true;
            }

            if( @this.Body is NewExpression newExpression )
            {
                instance = null;
                member = newExpression.Constructor;
                return true;
            }

            instance = null;
            member = null;
            return false;
        }
    }
}