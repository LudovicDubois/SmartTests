using System;
using System.Reflection;



namespace SmartTests
{
    public abstract class ActBase
    {
        public object Instance { get; internal set; }
        public ConstructorInfo Constructor { get; internal set; }
        public FieldInfo Field { get; internal set; }
        public MethodInfo Method { get; internal set; }
        public PropertyInfo Property { get; internal set; }

        public Exception Exception { get; internal set; }


        internal virtual void BeforeAct( Assertion[] assertions )
        {
            foreach( var assertion in assertions )
                assertion.BeforeAct( this );
        }


        internal void AfterAct( Assertion[] assertions )
        {
            foreach( var assertion in assertions )
            {
                assertion.AfterAct( this );
            }
        }
    }


    public abstract class Act: ActBase
    {
        public abstract void Invoke();
    }


    public abstract class Act<T>: ActBase
    {
        public abstract T Invoke();
    }
}