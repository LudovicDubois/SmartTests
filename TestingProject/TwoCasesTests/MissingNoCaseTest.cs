using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.TwoCasesTests
{
    [TestFixture]
    public class MissingNoCaseTest
    {
        // This method is here to be independent of the other tests
        private static double Sqrt( double value ) => Math.Sqrt( value );


        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ValidValue.IsValid ), () => Sqrt( 4 ) );

            Assert.That( result, Is.EqualTo( 2 ) );
        }


        [Test]
        public void TestMethod2()
        {
            Assert.Throws<IndexOutOfRangeException>( () => RunTest( Case( ValidValue.IsInvalid ), () => Sqrt( -4 ) ) );
        }
    }
}