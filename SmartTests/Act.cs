using System;
using System.Collections.Generic;
using System.Linq;
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

        public Assertion[] Assertions { get; internal set; }
        private readonly List<Assertion> _DoneAssertions = new List<Assertion>();


        internal virtual void BeforeAct()
        {
            var assertions = Assertions.ToList();
            assertions.Reverse();
            foreach( var assertion in assertions )
            {
                assertion.BeforeAct( this );
                _DoneAssertions.Insert( 0, assertion );
            }
        }


        internal void AfterAct()
        {
            foreach( var assertion in _DoneAssertions )
                assertion.AfterAct( this );
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