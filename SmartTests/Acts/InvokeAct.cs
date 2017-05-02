using System;
using System.Linq.Expressions;



namespace SmartTests.Acts
{
    public class InvokeAct: Act
    {
        public InvokeAct( Expression<Action> invocation )
        {
            _Invocation = invocation;
        }


        private readonly Expression<Action> _Invocation;

        public override void Invoke() => _Invocation.Compile().Invoke();
    }


    public class InvokeAct<T>: Act<T>
    {
        public InvokeAct( Expression<Func<T>> invocation )
        {
            _Invocation = invocation;
        }


        private readonly Expression<Func<T>> _Invocation;

        public override T Invoke() => _Invocation.Compile().Invoke();
    }
}