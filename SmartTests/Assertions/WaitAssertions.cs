using System;
using System.Threading;

using JetBrains.Annotations;



namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test asynchronous tests.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest" />
    [PublicAPI]
    public static class WaitAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an asynchronous call from the Act is done before continuing Smart
        ///     Assertions validations.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="evt">The <see cref="WaitHandle" /> to wait for before continuing Smart Assertions validations.</param>
        /// <param name="timeout">The maximum milliseconds to wait for the handle to be set.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="evt" /> is not set within <paramref name="timeout" /> milliseconds.
        /// </exception>
        /// <example>
        ///     <para> In this example, the Smart Assertion verifies that the <c>MyMethod</c> call launches a callback within 1s.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     var handle = new ManualResetEvent( false );
        /// 
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod( () => handle.set() ),
        ///              SmartAssert.Wait( handle, 1000 ) );
        /// }</code>
        /// </example>
        public static Assertion Wait( this SmartAssertPlaceHolder _, WaitHandle evt, double timeout ) => new WaitAssertion( evt, TimeSpan.FromMilliseconds( timeout ) );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an asynchronous call from the Act is done before continuing Smart
        ///     Assertions validations.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="evt">The <see cref="WaitHandle" /> to wait for before continuing Smart Assertions validations.</param>
        /// <param name="timeout">The maximum milliseconds to wait for the handle to be set.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="evt" /> is not set within <paramref name="timeout" />.
        /// </exception>
        /// <example>
        ///     <para> In this example, the Smart Assertion verifies that the <c>MyMethod</c> call launches a callback within 1s.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     var handle = new ManualResetEvent( false );
        /// 
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod( () => handle.set() ),
        ///              SmartAssert.Wait( handle, TimeSpan.FromSeconds( 1 ) ) );
        /// }</code>
        /// </example>
        public static Assertion Wait( this SmartAssertPlaceHolder _, WaitHandle evt, TimeSpan timeout ) => new WaitAssertion( evt, timeout );


        private class WaitAssertion: Assertion
        {
            public WaitAssertion( WaitHandle evt, TimeSpan fromMilliseconds )
            {
                _Event = evt;
                _FromMilliseconds = fromMilliseconds;
            }


            private readonly WaitHandle _Event;
            private readonly TimeSpan _FromMilliseconds;


            public override void BeforeAct( ActBase act )
            { }


            public override void AfterAct( ActBase act )
            {
                if( !_Event.WaitOne( _FromMilliseconds ) )
                    throw new SmartTestException( Resource.TimeoutReached, _FromMilliseconds.TotalMilliseconds );
            }
        }
    }
}