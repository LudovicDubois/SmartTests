using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.ThreeCasesTests
{
    [TestFixture]
    public class MissingThirdCaseTest
    {
        // This method is here to be independent of the other tests
        private static double Sqrt( double value ) => Math.Sqrt( value );


        [Test]
        public void TestMethod()
        {
            // Arrange
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>( () => RunTest( Case( MinIncluded.IsBelowMin ), () => Sqrt( 4 ) ) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( MinIncluded.IsMin ), () => Sqrt( 0 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}