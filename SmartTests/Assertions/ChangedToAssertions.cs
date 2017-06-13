using System;
using System.Linq.Expressions;



namespace SmartTests.Assertions
{
    public static class ChangedToAssertions
    {
        public static Assertion ChangedTo<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> after, T value ) => new ChangedToAssertion<T>( after, value );


        private class ChangedToAssertion<T>: Assertion
        {
            public ChangedToAssertion( Expression<Func<T>> after, T value )
            {
                _After = after;
                _Value = value;
            }


            private readonly Expression<Func<T>> _After;
            private readonly T _Value;
            private Func<T> _AfterCompiled;


            public override void BeforeAct( ActBase act )
            {
                _AfterCompiled = _After.Compile();
                if( Equals( _AfterCompiled(), _Value ) )
                    throw new BadTestException( string.Format( Resource.BadTest_UnexpectedValue, _Value ) );
            }


            public override void AfterAct( ActBase act )
            {
                var actualValue = _AfterCompiled();
                if( !Equals( actualValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, actualValue ) );
            }
        }
    }
}