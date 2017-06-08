using System;
using System.Linq.Expressions;
using System.Reflection;

using SmartTests.Helpers;



namespace SmartTests.Acts
{
    public class AssignAct<T>: Act<T>
    {
        public AssignAct( Expression<Func<T>> property, T value )
        {
            _Property = property;
            _Value = value;

            object instance;
            MemberInfo member;
            if( _Property.GetMemberContext( out instance, out member ) )
            {
                Instance = instance;
                Property = member as PropertyInfo;
                Method = Property?.GetSetMethod();
            }
            if( Method == null )
                throw new SmartTestException( string.Format( Resource.BadTest_NotWritableProperty, member.GetFullName() ) );
        }


        private readonly Expression<Func<T>> _Property;
        private readonly T _Value;


        public override T Invoke()
        {
            var propertyGetExpression = (MemberExpression)_Property.Body;
            var closureExpression = propertyGetExpression.Expression as MemberExpression;

            var property = Expression.Property( closureExpression, (MethodInfo)Method );
            var lambda = Expression.Lambda( Expression.Assign( property, Expression.Constant( _Value ) ) ).Compile();

            return (T)lambda.DynamicInvoke();
        }
    }
}