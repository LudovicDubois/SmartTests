using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;



namespace SmartTests.Acts
{
    public class AssignAct<T>: Act<T>
    {
        public AssignAct( Expression<Func<T>> property, T value )
        {
            _Property = property;
            _Value = value;
        }


        private readonly Expression<Func<T>> _Property;
        private readonly T _Value;


        public override T Invoke()
        {
            var memberExpression = (MemberExpression)_Property.Body;
            var instance = memberExpression.Expression;
            Debug.Assert( instance != null );
            var member = memberExpression.Member as PropertyInfo;
            Debug.Assert( member != null );


            var property = Expression.Property( instance, member );
            return (T)Expression.Lambda( Expression.Assign( property, Expression.Constant( _Value ) ) ).Compile().DynamicInvoke();
        }
    }
}