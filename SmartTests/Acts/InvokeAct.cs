using System;
using System.Linq.Expressions;
using System.Reflection;

using SmartTests.Helpers;



namespace SmartTests.Acts
{
    public class InvokeAct: Act
    {
        public InvokeAct( Expression<Action> invocation )
        {
            _Invocation = invocation;

            object instance;
            MemberInfo member;
            if( invocation.GetMemberContext( out instance, out member ) )
            {
                Instance = instance;
                Constructor = member as ConstructorInfo;
                Method = member as MethodInfo;
            }
            if( Method == null )
                throw new SmartTestException();
        }


        private readonly Expression<Action> _Invocation;

        public override void Invoke() => _Invocation.Compile().Invoke();
    }


    public class InvokeAct<T>: Act<T>
    {
        public InvokeAct( Expression<Func<T>> invocation )
        {
            _Invocation = invocation;

            object instance;
            MemberInfo member;
            if( invocation.GetMemberContext( out instance, out member ) )
            {
                Instance = instance;
                Constructor = member as ConstructorInfo;
                if( Constructor != null )
                    return;
                Method = member as MethodInfo;
                if( Method == null )
                {
                    Property = member as PropertyInfo;
                    if( Property != null )
                        Method = Property.GetMethod;
                    else
                        Field = member as FieldInfo;
                }
                else if( Method.IsSpecialName )
                    // An indexer
                    foreach( var property in Method.DeclaringType.GetRuntimeProperties() )
                        if( property.GetMethod == Method )
                        {
                            Property = property;
                            break;
                        }
            }
            if( Constructor == null && Method == null && Field == null )
                throw new BadTestException( string.Format( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() ) );
        }


        private readonly Expression<Func<T>> _Invocation;

        public override T Invoke() => _Invocation.Compile().Invoke();
    }
}