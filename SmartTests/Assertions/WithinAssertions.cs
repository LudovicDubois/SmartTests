using System;
using System.Diagnostics;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test duration of Act part of your test.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest" />
    // ReSharper disable once UnusedMember.Global
    public static class WithinAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure the Act is done within a specific duration.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="maximumDuration">The maximum allowed duration of the Act.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If the Act part of your test is longer than <paramref name="maximumDuration" /> ms.
        /// </exception>
        /// <remarks>For this assertion to be accurate, you have to put it as the first Smart Assertion.</remarks>
        /// <example>
        ///     <para> In this example, the Smart Assertion verifies that the <c>MyMethod</c> call takes no longer than 10ms.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Within(10) );
        /// }</code>
        /// </example>
        public static Assertion Within( this SmartAssertPlaceHolder _, long maximumDuration ) => new WithinAssertion( maximumDuration );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure the Act is done within a specific duration.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="maximumDuration">The maximum allowed duration of the Act</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If the Act part of your test is longer than <paramref name="maximumDuration" /> ms.
        /// </exception>
        /// <remarks>For this assertion to be accurate, you have to put it as the first Smart Assertion.</remarks>
        /// <example>
        ///     <para> In this example, the Smart Assertion verifies that the <c>MyMethod</c> call takes no longer than 10ms.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Within( TimeSpan.FromMilliseconds( 10 ) ) );
        /// }</code>
        /// </example>
        public static Assertion Within( this SmartAssertPlaceHolder _, TimeSpan maximumDuration ) => new WithinAssertion( (long)maximumDuration.TotalMilliseconds );


        private class WithinAssertion: Assertion
        {
            public WithinAssertion( long maximumMilliseconds )
            {
                if( maximumMilliseconds <= 0 )
                    throw new BadTestException( Resource.BadTest_NegativeTimeSpan, maximumMilliseconds );
                _MaximumMilliseconds = maximumMilliseconds;
            }


            private readonly long _MaximumMilliseconds;
            private Stopwatch _Stopwatch;


            public override void BeforeAct( ActBase act )
            {
                _Stopwatch = new Stopwatch();
                _Stopwatch.Start();
            }


            public override void AfterAct( ActBase act )
            {
                _Stopwatch.Stop();
                if( _Stopwatch.ElapsedMilliseconds > _MaximumMilliseconds )
                    throw new SmartTestException( Resource.TimespanExceeded, _MaximumMilliseconds, _Stopwatch.ElapsedMilliseconds );
            }
        }
    }
}