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
        private const string _ContextHandle = "WaitAssertions_Handle";
        private const string _ContextHandleSet = "WaitAssertions_HandleSet";


        /// <summary>
        /// Sets the implicit wait handle of the <see cref="ActContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="ActContext"/> for which to set the implicit wait handle</param>
        /// <exception cref="BadTestException"> If the Wait assertion is not waiting the implicit wait handle (<see cref="WaitContextHandle(SmartTests.SmartAssertPlaceHolder,System.TimeSpan)"/> nor <see cref="WaitContextHandle(SmartTests.SmartAssertPlaceHolder,double)"/> is called).</exception>
        /// <example>
        ///     <para> In this example, the <c>SetHandle</c> set the implicit wait handle expected by the <see cref="WaitContextHandle(SmartTests.SmartAssertPlaceHolder,double)"/> Smart Assertion call.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///   var mc = new MyClass(300);
        ///   
        ///   RunTest( AnyValue.IsValid,
        ///            ctx => mc.Method( ctx.SetHandle ),
        ///            SmartAssert.Within( 100 ),
        ///            SmartAssert.WaitContextHandle( 1000 ) );
        ///   
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
        /// }</code>
        /// </example>
        public static void SetHandle( this ActContext context )
        {
            context[ _ContextHandleSet ] = true;
            if( !context.TryGetValue( _ContextHandle, out EventWaitHandle obj ) )
                throw new BadTestException( Resource.BadTest_UnexpectedContextSetHandle );
            obj.Set();
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an asynchronous call from the Act is done before continuing Smart
        ///     Assertions validations.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="evt">The <see cref="WaitHandle" /> to wait for before continuing Smart Assertions validations.</param>
        /// <param name="timeout">The maximum milliseconds to wait for the handle to be set.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="evt"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="timeout"/> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="BadTestException">If you call <see cref="SetHandle"/>, as you provided a <paramref name="evt"/> to wait for.</exception>
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
        public static Assertion Wait( this SmartAssertPlaceHolder _, [NotNull] WaitHandle evt, double timeout )
        {
            if( evt == null )
                throw new ArgumentNullException( nameof(evt) );

            return new WaitAssertion( evt, TimeSpan.FromMilliseconds( timeout ) );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an asynchronous call from the Act is done before continuing Smart
        ///     Assertions validations.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="timeout">The maximum milliseconds to wait for the handle to be set.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="timeout"/> is less than or equal to <c>0</c>.</exception>
        /// <example>
        ///     <para> In this example, the <c>WaitContextHandle</c> wait for the implicit wait handle set by <see cref="SetHandle"/>.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///   var mc = new MyClass(300);
        ///   
        ///   RunTest( AnyValue.IsValid,
        ///            ctx => mc.Method( ctx.SetHandle ),
        ///            SmartAssert.Within( 100 ),
        ///            SmartAssert.WaitContextHandle( 1000 ) );
        ///   
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
        /// }</code>
        /// </example>
        public static Assertion WaitContextHandle( this SmartAssertPlaceHolder _, double timeout ) => new WaitAssertion( null, TimeSpan.FromMilliseconds( timeout ) );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an asynchronous call from the Act is done before continuing Smart
        ///     Assertions validations.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="evt">The <see cref="WaitHandle" /> to wait for before continuing Smart Assertions validations.</param>
        /// <param name="timeout">The maximum milliseconds to wait for the handle to be set.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="evt"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="timeout"/> is less than or equal to <c>0</c>.</exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="evt" /> is not set within <paramref name="timeout" />.
        /// </exception>
        /// <exception cref="BadTestException">If you call <see cref="SetHandle"/>, as you provided a <paramref name="evt"/> to wait for.</exception>
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
        public static Assertion Wait( this SmartAssertPlaceHolder _, [NotNull] WaitHandle evt, TimeSpan timeout )
        {
            if( evt == null )
                throw new ArgumentNullException( nameof(evt) );
            return new WaitAssertion( evt, timeout );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an asynchronous call from the Act is done before continuing Smart
        ///     Assertions validations.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="timeout">The maximum milliseconds to wait for the handle to be set.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="timeout"/> is less than or equal to <c>0</c>.</exception>
        /// <example>
        ///     <para> In this example, the <c>WaitContextHandle</c> wait for the implicit wait handle set by <see cref="SetHandle"/>.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///   var mc = new MyClass(300);
        ///   
        ///   RunTest( AnyValue.IsValid,
        ///            ctx => mc.Method( ctx.SetHandle ),
        ///            SmartAssert.Within( 100 ),
        ///            SmartAssert.WaitContextHandle( TimeSpan.FromSeconds( 1 ) ) );
        ///   
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
        /// }</code>
        /// </example>
        public static Assertion WaitContextHandle( this SmartAssertPlaceHolder _, TimeSpan timeout ) => new WaitAssertion( null, timeout );


        private class WaitAssertion: Assertion
        {
            public WaitAssertion( WaitHandle evt, TimeSpan timeout )
            {
                if( timeout.TotalMilliseconds <= 0 )
                    throw new ArgumentOutOfRangeException( nameof(timeout) );

                _Event = evt;
                _Timeout = timeout;
            }


            private WaitHandle _Event;
            private readonly TimeSpan _Timeout;


            public override void BeforeAct( ActBase act )
            {
                if( _Event == null )
                {
                    _Event = new AutoResetEvent( false );
                    act.Context[ _ContextHandle ] = _Event;
                    act.Context[ _ContextHandleSet ] = false;
                }
            }


            public override void AfterAct( ActBase act )
            {
                if( _Event.WaitOne( _Timeout ) )
                    // Everything is OK
                    return;

                if( !act.Context.TryGetValue( _ContextHandleSet, out bool handleSet ) )
                    // Not a context handle
                    throw new SmartTestException( Resource.TimeoutReached, _Timeout.TotalMilliseconds );


                // context handle is set? => waiting for the wrong handle!
                if( handleSet )
                    throw new BadTestException( Resource.BadTest_UnexpectedContextSetHandle );
                throw new SmartTestException( Resource.TimeoutReached, _Timeout.TotalMilliseconds );
            }
        }
    }
}