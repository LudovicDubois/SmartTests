using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class UInt64Type: NumericType<ulong, UInt64Type>
    {
        /// <inheritdoc />
        protected override ulong MinValue => ulong.MinValue;
        /// <inheritdoc />
        protected override ulong MaxValue => ulong.MaxValue;


        /// <inheritdoc />
        protected override ulong GetPrevious( ulong n ) => n - 1;


        /// <inheritdoc />
        protected override ulong GetNext( ulong n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out ulong value, params ulong[] avoidedValues )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextUInt64();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextUInt64( MinValue, max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += chunk.IncludedMax - chunk.IncludedMin + 1;
                    if( val >= max )
                        continue;
                    value = val - min + chunk.IncludedMin;
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
        }


        /// <inheritdoc />
        protected override string ToString( ulong value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt64Range" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for ulong from a ulong.
    /// </summary>
    public static class UInt64TypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> Range( this ulong @this, ulong min, ulong max ) => SmartTest.UInt64Range.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> Range( this ulong @this, ulong min, bool minIncluded, ulong max, bool maxIncluded ) => SmartTest.UInt64Range.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> AboveOrEqual( this ulong @this, ulong min ) => SmartTest.UInt64Range.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> Above( this ulong @this, ulong min ) => SmartTest.UInt64Range.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> BelowOrEqual( this ulong @this, ulong max ) => SmartTest.UInt64Range.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> Below( this ulong @this, ulong max ) => SmartTest.UInt64Range.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A ulong we do not care about, except to know to create a <see cref="INumericType{T}" /> for ulong.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ulong so that adding chunks can be chained.</returns>
        public static INumericType<ulong> Value( this ulong @this, ulong value ) => SmartTest.UInt64Range.Value( value );
    }
}