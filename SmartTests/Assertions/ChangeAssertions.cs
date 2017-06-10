// ReSharper disable UnusedParameter.Global

using System;
using System.Linq.Expressions;



namespace SmartTests.Assertions
{
    public static class ChangeAssertions
    {
        public static Assertion Change<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> after ) => new ChangeAssertion<T>( after );


        private class ChangeAssertion<T>: Assertion
        {
            public ChangeAssertion( Expression<Func<T>> after )
            {
                _After = after;
            }


            private readonly Expression<Func<T>> _After;
            private T _FutureValue;


            class ChangeVisitor: ExpressionVisitor
            {
                public Expression Result { get; private set; }


                protected override Expression VisitMethodCall( MethodCallExpression node )
                {
                    if( Result == null )
                        Result = node;
                    return node;
                }


                protected override Expression VisitMember( MemberExpression node )
                {
                    if( Result == null )
                        Result = node;
                    return node;
                }
            }


            public override void BeforeAct( ActBase act )
            {
                _FutureValue = _After.Compile().Invoke();
            }


            public override void AfterAct( ActBase act )
            {
                var visitor = new ChangeVisitor();
                visitor.Visit( _After.Body );

                var newValue = Expression.Lambda( visitor.Result ).Compile().DynamicInvoke();
                if( !Equals( newValue, _FutureValue ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _FutureValue, newValue ) );
            }
        }
    }
}