using System;
using System.Linq.Expressions;

using SmartTests.Acts;



namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test changes of properties/indexers.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest.Assign{T}" />
    /// <seealso cref="SmartTest" />
    public static class ChangedToAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed in the Act part of
        ///     your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">if the property/indexer value BEFORE the Act is the assigned value.</exception>
        /// <exception cref="SmartTestException">if the property/indexer value AFTER the Act is not the assigned value.</exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Evaluates the property involved in the Act and raises a <see cref="BadTestException" /> if it is
        ///                     the same value as the one involved in the Act.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Evaluates the property involved in the Act and raises a <see cref="SmartTestException" /> if it
        ///                     is not the same value as the one involved in the Act.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        ///     If you want to test changes of another property/indexer or value, prefer using <see cref="ChangedTo{T}" />
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that <c>!Equals(mc.MyProperty,10)</c> before the Act part of your
        ///     test (otherwise a <see cref="BadTestException" /> is raised) and that <c>Equals(mc.MyProperty,10)</c> after the Act
        ///     (otherwise a <see cref="SmartTestException" /> is raised).
        ///     <code>
        /// [Test]
        /// public void MyPropertyTest_Set()
        /// {
        ///     var mc = new MyClass();
        /// 
        ///     RunTest( AnyValue.Valid,
        ///              Assign( () => mc.MyProperty, 10 ),
        ///              SmartAssert.ChangedTo() );
        /// }</code>
        /// </example>
        /// <seealso cref="Assertion" />
        /// <seealso cref="SmartTest.SmartAssert" />
        /// <seealso cref="SmartTest" />
        // ReSharper disable once UnusedParameter.Global
        public static Assertion ChangedTo( this SmartAssertPlaceHolder _ ) => new ChangedToAssertion();


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed in the Act part of
        ///     your test.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the property/indexer.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="after">
        ///     The expression involving a property/indexer whose value should change in the Act part of your test.
        /// </param>
        /// <param name="value">The value to assign to the property/indexer.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">if the property/indexer value BEFORE the Act is <paramref name="value" />.</exception>
        /// <exception cref="SmartTestException">if the property/indexer value AFTER the Act is not <paramref name="value" />.</exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Evaluates the <paramref name="after" /> expression and raises a <see cref="BadTestException" />
        ///                     if it is <paramref name="value" />.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Evaluates the <paramref name="after" /> expression and raises a <see cref="SmartTestException" />
        ///                     if it is not <paramref name="value" />.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        ///     If you want to test changes of property/indexer and value involved in the Act, prefer using
        ///     <see cref="ChangedTo" />
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that <c>!Equals(mc.MyProperty,10)</c> before the Act part of your
        ///     test (otherwise a <see cref="BadTestException" /> is raised) and that <c>Equals(mc.MyProperty,10)</c> after the Act
        ///     (otherwise a <see cref="SmartTestException" /> is raised).
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        /// 
        ///     RunTest(AnyValue.Valid,
        ///             () => mc.MyMethod(),
        ///             SmartAssert.ChangedTo(() => mc.MyProperty, 10));
        /// }</code>
        /// </example>
        /// <seealso cref="Assertion" />
        /// <seealso cref="SmartTest.SmartAssert" />
        /// <seealso cref="SmartTest" />
        // ReSharper disable once UnusedParameter.Global
        public static Assertion ChangedTo<T>( this SmartAssertPlaceHolder _, Expression<Func<T>> after, T value ) => new ChangedToAssertion<T>( after, value );


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
                if( act.Exception != null )
                    return;

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
                if( act.Exception != null )
                    return;

                var actualValue = _Compiled();
                if( !Equals( actualValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, actualValue ) );
            }
        }
    }
}