using System;
using System.Linq.Expressions;



namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test changes of properties/indexers.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest" />
    public static class ChangeAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure relative changes to a property/indexer of an object.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the property/indexer.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="after">
        ///     The expression involving a property/indexer whose value should be equal to the value of the member
        ///     only after the Act.
        /// </param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     if the property/indexer value AFTER the Act is not the value of the expression
        ///     <paramref name="after" />.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>Evaluates the <paramref name="after" /> expression.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Evaluates the <paramref name="after" /> expression stripped of any computation so that there is
        ///                     only a property/indexer call.
        ///                 </para>
        ///                 <para>
        ///                     This value should be equal to the previously computed value (<paramref name="after" /> before the
        ///                     Act); otherwise a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In the following example, we expect that the <c>mc.MyList.Count</c> value after the Act is the value of the same
        ///     property before the Act plus 1.
        ///     <code>
        /// [Test]
        /// public void AddItemTest()
        /// {
        ///     var mc = new MyClass();
        /// 
        ///     RunTest( AnyValue.Valid,
        ///              () => mc.AddItem(),
        ///              SmartAssert.Change( () => mc.MyList.Count + 1 ) );
        /// }</code>
        /// </example>
        /// <seealso cref="Assertion" />
        /// <seealso cref="SmartTest.SmartAssert" />
        /// <seealso cref="SmartTest" />
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Change<T>( this SmartAssertPlaceHolder _, Expression<Func<T>> after ) => new ChangeAssertion<T>( after );


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
                if( act.Exception != null )
                    return;

                var visitor = new ChangeVisitor();
                visitor.Visit( _After.Body );

                var newValue = Expression.Lambda( visitor.Result ).Compile().DynamicInvoke();
                if( !Equals( newValue, _FutureValue ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _FutureValue, newValue ) );
            }
        }
    }
}