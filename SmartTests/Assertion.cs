namespace SmartTests
{
    /// <summary>
    ///     Base class of all Smart Assertions
    /// </summary>
    public abstract class Assertion
    {
        /// <summary>
        ///     The method called before the Act in a test.
        /// </summary>
        /// <param name="act">The Act of your test</param>
        /// <seealso cref="O:SmartTests.SmartTest.RunTest" />
        public abstract void BeforeAct( ActBase act );


        /// <summary>
        ///     The method called after the Act in a test.
        /// </summary>
        /// <param name="act">The Act of your test</param>
        /// <seealso cref="O:SmartTests.SmartTest.RunTest" />
        public abstract void AfterAct( ActBase act );
    }
}