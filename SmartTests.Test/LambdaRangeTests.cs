using System;

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
            var result = SmartTest.Case( ( byte b ) => b.Range( 10, 20 ), out var value );

            // Assert
            Assert.AreEqual( "b", result.ParameterName );
            Assert.GreaterOrEqual( value, 10 );
            Assert.LessOrEqual( value, 20 );
        }


        class Data
        {
            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public byte Info { get; }
            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public DayOfWeek Day { get; }
        }


        [Test]
        public void PathRange()
        {
            // Act
            var result = SmartTest.Case( ( Data d ) => d.Info.Range( 10, 20 ), out var value );

            // Assert
            Assert.AreEqual( "d.Info", result.ParameterName );
            Assert.GreaterOrEqual( value, 10 );
            Assert.LessOrEqual( value, 20 );
        }


        [Test]
        public void PathRange2()
        {
            // Act
            var result = SmartTest.Case( ( Data d ) => d.Info.Range( 10, 20 ).Range( 30, 40 ), out var value );

            // Assert
            Assert.AreEqual( "d.Info", result.ParameterName );
            Assert.IsTrue( 10 <= value && value <= 20 || 30 <= value && value <= 40 );
        }


        [Test]
        public void EnumValues()
        {
            // Act
            var result = SmartTest.Case( ( DayOfWeek dw ) => dw.Values( DayOfWeek.Saturday, DayOfWeek.Sunday ), out var value );

            // Assert
            Assert.AreEqual( "dw", result.ParameterName );
            Assert.IsTrue( value == DayOfWeek.Saturday || value == DayOfWeek.Sunday );
        }


        [Test]
        public void EnumPathValues()
        {
            // Act
            var result = SmartTest.Case( ( Data d ) => d.Day.Values( DayOfWeek.Saturday, DayOfWeek.Sunday ), out var value );

            // Assert
            Assert.AreEqual( "d.Day", result.ParameterName );
            Assert.IsTrue( value == DayOfWeek.Saturday || value == DayOfWeek.Sunday );
        }
    }
}