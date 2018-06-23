using NUnit.Framework;

using static SmartTests.SmartTest;



namespace TestingProject.Ranges
{
    [TestFixture]
    public class RangeTests
    {
        class MyClass
        {
            public static int M1( int i ) => i;
            public static int M2( int i ) => i;
        }


        [Test]
        public void RangeIITest()
        {
            var result = RunTest( Range( 0, int.MaxValue ).GetValue( out var value ),
                                  () => MyClass.M1( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void RangeIIOutTest()
        {
            var result = RunTest( Range( 0, int.MaxValue, out var value ),
                                  () => MyClass.M1( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void AboveOrEqualTest()
        {
            var result = RunTest( AboveOrEqual( 0 ).GetValue( out var value ),
                                  () => MyClass.M1( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void AboveOrEqualOutTest()
        {
            var result = RunTest( AboveOrEqual( 0, out var value ),
                                  () => MyClass.M1( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void BelowOrEqualTest()
        {
            var result = RunTest( BelowOrEqual( 0 ).GetValue( out var value ),
                                  () => MyClass.M1( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void BelowOrEqualOutTest()
        {
            var result = RunTest( BelowOrEqual( 0, out var value ),
                                  () => MyClass.M1( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void RangeIITestTwice()
        {
            var result = RunTest( Range( 1, int.MaxValue )
                                      .Add( int.MinValue, -1 )
                                      .GetValue( out var value ),
                                  () => MyClass.M2( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }


        [Test]
        public void RangeIITestOutTwice()
        {
            var result = RunTest( Range( 1, int.MaxValue )
                                      .Add( int.MinValue, -1, out var value ),
                                  () => MyClass.M2( value ) );

            Assert.That( result, Is.EqualTo( value ) );
        }
    }
}