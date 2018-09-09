using System;

using NUnit.Framework;

using SmartTests.Helpers;
using SmartTests.Ranges;



namespace SmartTests.Test
{
    [TestFixture]
    public class DoubleRangeTests
    {
        private static void AssertChunks( INumericType<double> type, params double[] expectedChunks )
        {
            Assert.AreEqual( expectedChunks.Length, 2 * type.Chunks.Count, $"Bad chunk parts count {type}" );

            var i = 0;
            var index = 0;
            foreach( var chunk in type.Chunks )
            {
                ++index;
                Assert.AreEqual( expectedChunks[ i ], chunk.Min, $"Chunks[{index}].Min" );
                ++i;
                Assert.AreEqual( expectedChunks[ i ], chunk.Max, $"Chunks[{index}].Max" );
                ++i;
            }
        }


        static readonly double _Before10 = BitConverterHelper.Previous( (double)10 );
        static readonly double _After20 = BitConverterHelper.Next( (double)20 );
        static readonly double _Before30 = BitConverterHelper.Previous( (double)30 );
        static readonly double _After40 = BitConverterHelper.Next( (double)40 );


        [Test]
        public void Range_MinLowerThanMax()
        {
            // Act
            var range = SmartTest.Double.Range( 10, 20 );

            // Assert
            AssertChunks( range, 10, 20 );
        }


        [Test]
        public void Range_MinEqualToMax()
        {
            // Act
            var range = SmartTest.Double.Range( 10, 10 );

            // Assert
            AssertChunks( range, 10, 10 );
        }


        [Test]
        public void Range_MinGreaterThanMax()
        {
            // Act & Assert
            Assert.Throws( typeof(ArgumentException),
                           // ReSharper disable once ObjectCreationAsStatement
                           () => SmartTest.Double.Range( 20, 10 ) );
        }


        #region AddChunk Tests

        #region One Range

        [Test]
        public void AddChunk_Error()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act & Assert
            Assert.Throws( typeof(ArgumentException),
                           // ReSharper disable once ObjectCreationAsStatement
                           () => range.Range( 20, 10 ) );
        }


        [Test]
        public void AddChunk_InfinityBeforeMin()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( double.MinValue, 8 );

            // Assert
            AssertChunks( range, double.MinValue, 8, 10, 20 );
        }


        [Test]
        public void AddChunk_BeforeMin()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 5, 8 );

            // Assert
            AssertChunks( range, 5, 8, 10, 20 );
        }


        [Test]
        public void AddChunk_MaxImmediatelyBeforeMin()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 5, _Before10 );

            // Assert
            AssertChunks( range, 5, 20 );
        }


        [Test]
        public void AddChunk_ImmediatelyBeforeMin1()
        {
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( _Before10, _Before10 );

            // Assert
            AssertChunks( range, _Before10, 20 );
        }


        [Test]
        public void AddChunk_MinImmediatelyBeforeMin()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( _Before10, 15 );

            // Assert
            AssertChunks( range, _Before10, 20 );
        }


        [Test]
        public void AddChunk_ContainingMin()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 5, 14 );

            // Assert
            AssertChunks( range, 5, 20 );
        }


        [Test]
        public void AddChunk_ContainingMinMax()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 5, 30 );

            // Assert
            AssertChunks( range, 5, 30 );
        }


        [Test]
        public void AddChunk_BetweenMinMax()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 12, 14 );

            // Assert
            AssertChunks( range, 10, 20 );
        }


        [Test]
        public void AddChunk_ContainingMax()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 12, 25 );

            // Assert
            AssertChunks( range, 10, 25 );
        }


        [Test]
        public void AddChunk_MinImmediatelyAfterMax()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( _After20, 30 );

            // Assert
            AssertChunks( range, 10, 30 );
        }


        [Test]
        public void AddChunk_ImmediatelyAfterMax1()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( _After20, _After20 );

            // Assert
            AssertChunks( range, 10, _After20 );
        }


        [Test]
        public void AddChunk_MaxImmediatelyAfterMax()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 15, _After20 );

            // Assert
            AssertChunks( range, 10, _After20 );
        }


        [Test]
        public void AddChunk_AfterMax()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 30, 40 );

            // Assert
            AssertChunks( range, 10, 20, 30, 40 );
        }


        [Test]
        public void AddChunk_AfterMaxInfinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );

            // Assume
            AssertChunks( range, 10, 20 );

            // Act
            range.Range( 30, double.MaxValue );

            // Assert
            AssertChunks( range, 10, 20, 30, double.MaxValue );
        }

        #endregion


        #region Two Ranges

        [Test]
        public void AddChunk2_BeforeMin1()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, 8 );

            // Assert
            AssertChunks( range, 5, 8, 10, 20, 30, 40 );
        }


        [Test]
        public void AddChunk2_ImmediatelyBeforeMin1()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, _Before10 );

            // Assert
            AssertChunks( range, 5, 20, 30, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMinMax1_BeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, 25 );

            // Assert
            AssertChunks( range, 5, 25, 30, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMinMax1_ImmediatelyBeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, _Before30 );

            // Assert
            AssertChunks( range, 5, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMinMax1_BetweenMinMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, 35 );

            // Assert
            AssertChunks( range, 5, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMinMax1_AfterMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, 45 );

            // Assert
            AssertChunks( range, 5, 45 );
        }


        [Test]
        public void AddChunk2_ContainingMinMax1_Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 5, double.MaxValue );

            // Assert
            AssertChunks( range, 5, double.MaxValue );
        }


        [Test]
        public void AddChunk2_BetweenMinMax1()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 15, 18 );

            // Assert
            AssertChunks( range, 10, 20, 30, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMax1_BeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 15, 25 );

            // Assert
            AssertChunks( range, 10, 25, 30, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMax1_ImmediatelyBeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 15, _Before30 );

            // Assert
            AssertChunks( range, 10, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMax1_ContainingMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 15, 32 );

            // Assert
            AssertChunks( range, 10, 40 );
        }


        [Test]
        public void AddChunk2_ContainingMax1_ContainingMinMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 15, 42 );

            // Assert
            AssertChunks( range, 10, 42 );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax1_BeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After20, 26 );

            // Assert
            AssertChunks( range, 10, 26, 30, 40 );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax1_ImmediatelyBeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After20, _Before30 );

            // Assert
            AssertChunks( range, 10, 40 );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax1_BetweenMinMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After20, 34 );

            // Assert
            AssertChunks( range, 10, 40 );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax1_AfterMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After20, 45 );

            // Assert
            AssertChunks( range, 10, 45 );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax1_Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After20, double.MaxValue );

            // Assert
            AssertChunks( range, 10, double.MaxValue );
        }


        [Test]
        public void AddChunk2_AfterMax1_BeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 23, 26 );

            // Assert
            AssertChunks( range, 10, 20, 23, 26, 30, 40 );
        }


        [Test]
        public void AddChunk2_AfterMax1_ImmediatelyBeforeMin2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 23, _Before30 );

            // Assert
            AssertChunks( range, 10, 20, 23, 40 );
        }


        [Test]
        public void AddChunk2_AfterMax1_BetweenMinMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 23, 34 );

            // Assert
            AssertChunks( range, 10, 20, 23, 40 );
        }


        [Test]
        public void AddChunk2_AfterMax1_AfterMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 23, 45 );

            // Assert
            AssertChunks( range, 10, 20, 23, 45 );
        }


        [Test]
        public void AddChunk2_AfterMax1_Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 23, double.MaxValue );

            // Assert
            AssertChunks( range, 10, 20, 23, double.MaxValue );
        }


        [Test]
        public void AddChunk2_BetweenMinMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 34, 36 );

            // Assert
            AssertChunks( range, 10, 20, 30, 40 );
        }


        [Test]
        public void AddChunk2_BetweenMinMax2_AfterMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 34, 46 );

            // Assert
            AssertChunks( range, 10, 20, 30, 46 );
        }


        [Test]
        public void AddChunk2_BetweenMinMax2_Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 34, double.MaxValue );

            // Assert
            AssertChunks( range, 10, 20, 30, double.MaxValue );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax2_AfterMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After40, 46 );

            // Assert
            AssertChunks( range, 10, 20, 30, 46 );
        }


        [Test]
        public void AddChunk2_ImmediatelyAfterMax2_Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( _After40, double.MaxValue );

            // Assert
            AssertChunks( range, 10, 20, 30, double.MaxValue );
        }


        [Test]
        public void AddChunk2_AfterMax2()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 45, 100 );

            // Assert
            AssertChunks( range, 10, 20, 30, 40, 45, 100 );
        }


        [Test]
        public void AddChunk2_AfterMax2_Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, 40 );

            // Assume
            AssertChunks( range, 10, 20, 30, 40 );

            // Act
            range.Range( 45, double.MaxValue );

            // Assert
            AssertChunks( range, 10, 20, 30, 40, 45, double.MaxValue );
        }


        [Test]
        public void AddChunk2Infinity()
        {
            // Arrange
            var range = SmartTest.Double.Range( 10, 20 );
            range.Range( 30, double.MaxValue );

            // Assume
            AssertChunks( range, 10, 20, 30, double.MaxValue );

            // Act
            range.Range( 35, 45 );

            // Assert
            AssertChunks( range, 10, 20, 30, double.MaxValue );
        }


        [Test]
        public void AddChunkThatCompleteBoth()
        {
            // Arrange
            var range = SmartTest.Double.Range( 1, double.MaxValue );
            range.Range( double.MinValue, -1 );

            // Assume
            AssertChunks( range, double.MinValue, -1, 1, double.MaxValue );

            // Act
            range.Range( BitConverterHelper.Next( (double)-1 ), BitConverterHelper.Previous( (double)1 ) );

            // Assert
            AssertChunks( range, double.MinValue, double.MaxValue );
        }

        #endregion

        #endregion
    }
}