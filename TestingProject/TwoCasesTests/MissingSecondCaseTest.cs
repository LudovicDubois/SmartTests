using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.TwoCasesTests
{
    [TestFixture]
    public class MissingSecondCaseTest
    {
        // This method is here to be independent of the other tests
        private static double Sqrt( double value ) => Math.Sqrt( value );


        [Test]
        public void TestMethod()
        {
            Assert.Throws<ArgumentOutOfRangeException>( () => RunTest( Case( ValidValue.IsInvalid ), () => Sqrt( -4 ) ) );
        }
    }
}