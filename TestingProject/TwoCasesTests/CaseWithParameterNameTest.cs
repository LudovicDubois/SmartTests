using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.TwoCasesTests
{
    [TestFixture]
    public class CaseWithParameterNameTest
    {
        // This method is here to be independent of the other tests
        private static double Sqrt( double value ) => Math.Sqrt( value );


        private static double DivRem( int a, int b, out int result ) => Math.DivRem( a, b, out result );


        [Test]
        public void RightParameterName()
        {
            var result = RunTest( Case( "value", ValidValue.Valid ),
                                  () => Sqrt( 4 ) );

            Assert.That( result, Is.EqualTo( 2 ) );
        }


        [Test]
        public void WrongParameterName()
        {
            var result = RunTest( Case( "d", ValidValue.Valid ),
                                  () => Sqrt( 4 ) );

            Assert.That( result, Is.EqualTo( 2 ) );
        }


        [Test]
        public void Missing1ParameterName()
        {
            var reminder = default(int);
            var result = RunTest( Case( "a", ValidValue.Valid ),
                                  () => DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }
    }
}