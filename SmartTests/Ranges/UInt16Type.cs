using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class UInt16Type: NumericType<ushort, UInt16Type>
    {
        /// <inheritdoc />
        protected override ushort MinValue => ushort.MinValue;
        /// <inheritdoc />
        protected override ushort MaxValue => ushort.MaxValue;


        /// <inheritdoc />
        protected override ushort GetPrevious( ushort n ) => (ushort)( n - 1 );


        /// <inheritdoc />
        protected override ushort GetNext( ushort n ) => (ushort)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out ushort value, params ushort[] avoidedValues )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextUInt16();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextUInt16( MinValue, (ushort)max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += chunk.IncludedMax - chunk.IncludedMin + 1;
                    if( val >= max )
                        continue;
                    value = (ushort)( val - min + chunk.IncludedMin );
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
        }


        /// <inheritdoc />
        protected override string ToString( ushort value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "UInt16Range" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for ushort from a ushort.
    /// </summary>
    public static class UInt16TypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> Range( this ushort @this, ushort min, ushort max ) => SmartTest.UInt16Range.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> Range( this ushort @this, ushort min, bool minIncluded, ushort max, bool maxIncluded ) => SmartTest.UInt16Range.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> AboveOrEqual( this ushort @this, ushort min ) => SmartTest.UInt16Range.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> Above( this ushort @this, ushort min ) => SmartTest.UInt16Range.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> BelowOrEqual( this ushort @this, ushort max ) => SmartTest.UInt16Range.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> Below( this ushort @this, ushort max ) => SmartTest.UInt16Range.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A ushort we do not care about, except to know to create a <see cref="INumericType{T}" /> for ushort.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for ushort so that adding chunks can be chained.</returns>
        public static INumericType<ushort> Value( this ushort @this, ushort value ) => SmartTest.UInt16Range.Value( value );
    }
}