using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.ThreeCasesTests
{
    [TestFixture]
    public class MissingFirstAndSecondCasesTest
    {
        // This method is here to be independent of the other tests
        private static double Sqrt( double value ) => Math.Sqrt( value );


        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( MinIncluded.IsAboveMin ), () => Sqrt( 4 ) );

            Assert.That( result, Is.EqualTo( 2 ) );
        }
    }
}