using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.TwoCasesTests
{
    [TestFixture]
    public class ParameterNameTest
    {
        // This method is here to be independent of the other tests
        private static double Sqrt( double value ) => Math.Sqrt( value );


        private static double DivRem( int a, int b, out int result ) => Math.DivRem( a, b, out result );
        private static double DivRem2( int a, int b, out int result ) => Math.DivRem( a, b, out result );


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
        public void Missing1ParameterCase()
        {
            var reminder = default(int);
            var result = RunTest( Case( "a", AnyValue.Valid ),
                                  () => DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }


        [Test]
        public void Missing2ParameterCases()
        {
            var reminder = default(int);
            var result = RunTest( Case( AnyValue.Valid ),
                                  () => DivRem2( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }


        [Test]
        public void MissingNoParameterCases1()
        {
            var reminder = default(int);
            var result = RunTest( Case( "a", AnyValue.Valid ) &
                                  Case( "b", ValidValue.Valid ),
                                  () => DivRem2( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }


        [Test]
        public void MissingNoParameterCases2()
        {
            int reminder;
            Assert.Throws<DivideByZeroException>( () => RunTest( Case( "a", AnyValue.Valid ) &
                                                                 Case( "b", ValidValue.Invalid ),
                                                                 () => DivRem2( 7, 0, out reminder ) ) );
        }


        private static void NoParameter()
        { }


        [Test]
        public void NoParameterNeeded_NoCase()
        {
            RunTest( AnyValue.Valid,
                     () => NoParameter() );
        }


        [Test]
        public void NoParameterNeeded_Case()
        {
            RunTest( Case( AnyValue.Valid ),
                     () => NoParameter() );
        }

        [Test]
        public void NoParameterNeeded_NullCase()
        {
            RunTest( Case( null, AnyValue.Valid ),
                     () => NoParameter() );
        }

        [Test]
        public void NoParameterNeeded_ParameterCase()
        {
            RunTest( Case( "value", AnyValue.Valid ),
                     () => NoParameter() );
        }
    }
}