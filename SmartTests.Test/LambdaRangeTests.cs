using NUnit.Framework;

using SmartTests.Ranges;



namespace SmartTests.Test
{
    [TestFixture]
    public class LambdaRangeTests
    {
        [Test]
        public void Range()
        {
            // Act
            SmartTest.Case( ( byte b ) => b.Range( 10, 20 ), out var value );

            // Assert
            Assert.GreaterOrEqual( value, 10 );
            Assert.LessOrEqual( value, 20 );
        }


        class Data
        {
            public byte Info { get; }
        }


        [Test]
        public void PathRange()
        {
            // Act
            SmartTest.Case( ( Data d ) => d.Info.Range( 10, 20 ), out var value );

            // Assert
            Assert.GreaterOrEqual( value, 10 );
            Assert.LessOrEqual( value, 20 );
        }


        [Test]
        public void PathRange2()
        {
            // Act
            SmartTest.Case( ( Data d ) => d.Info.Range( 10, 20 ).Range( 30, 40 ), out var value );

            // Assert
            Assert.IsTrue( 10 <= value && value <= 20 || 30 <= value && value <= 40 );
        }
    }
}