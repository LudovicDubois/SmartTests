using System;
using System.Linq.Expressions;

using SmartTests.Acts;



namespace SmartTests.Assertions
{
    public static class ChangedToAssertions
    {
        public static Assertion ChangedTo( this SmartAssertPlaceHolder @this ) => new ChangedToAssertion();
        public static Assertion ChangedTo<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> after, T value ) => new ChangedToAssertion<T>( after, value );


        private class ChangedToAssertion: Assertion
        {
            private IAssignee _Assignee;
            private object _Value;


            public override void BeforeAct( ActBase act )
            {
                _Assignee = act as IAssignee;
                if( _Assignee == null )
                    throw new BadTestException( Resource.BadTest_NotAssignment );

                _Value = _Assignee.AssignedValue;
                if( Equals( _Assignee.AssigneeValue, _Value ) )
                    throw new BadTestException( string.Format( Resource.BadTest_UnexpectedValue, _Value ) );
            }


            public override void AfterAct( ActBase act )
            {
                var actualValue = _Assignee.AssigneeValue;
                if( !Equals( actualValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, actualValue ) );
            }
        }


        private class ChangedToAssertion<T>: Assertion
        {
            public ChangedToAssertion( Expression<Func<T>> after, T value )
            {
                _After = after;
                _Value = value;
            }


            private readonly Expression<Func<T>> _After;
            private readonly T _Value;
            private Func<T> _Compiled;


            public override void BeforeAct( ActBase act )
            {
                _Compiled = _After.Compile();
                if( Equals( _Compiled(), _Value ) )
                    throw new BadTestException( string.Format( Resource.BadTest_UnexpectedValue, _Value ) );
            }


            public override void AfterAct( ActBase act )
            {
                var actualValue = _Compiled();
                if( !Equals( actualValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, actualValue ) );
            }
        }
    }
}