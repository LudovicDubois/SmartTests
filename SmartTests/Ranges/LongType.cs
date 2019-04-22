using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class LongType: NumericType<long, LongType>
    {
        /// <inheritdoc />
        protected override long MinValue => long.MinValue;
        /// <inheritdoc />
        protected override long MaxValue => long.MaxValue;


        /// <inheritdoc />
        protected override long GetPrevious( long n ) => n - 1;


        /// <inheritdoc />
        protected override long GetNext( long n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out long value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = random.NextLong( long.MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( value >= max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( long value )
        {
            if( value == long.MinValue )
                return "long.MinValue";
            if( value == long.MaxValue )
                return "long.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Long" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for long from a long.
    /// </summary>
    public static class LongTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> Range( this long @this, long min, long max ) => SmartTest.Long.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> Range( this long @this, long min, bool minIncluded, long max, bool maxIncluded ) => SmartTest.Long.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> AboveOrEqual( this long @this, long min ) => SmartTest.Long.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> Above( this long @this, long min ) => SmartTest.Long.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> BelowOrEqual( this long @this, long max ) => SmartTest.Long.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> Below( this long @this, long max ) => SmartTest.Long.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        public static INumericType<long> Value( this long @this, long value ) => SmartTest.Long.Value( value );
    }
}