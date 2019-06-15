using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of decimal values (with several chunks)
    /// </summary>
    public class DecimalType: NumericType<decimal, DecimalType>
    {
        /// <inheritdoc />
        protected override decimal MinValue => decimal.MinValue;
        /// <inheritdoc />
        protected override decimal MaxValue => decimal.MaxValue;


        /// <inheritdoc />
        protected override decimal GetPrevious( decimal n ) => BitConverterHelper.Previous( n );


        /// <inheritdoc />
        protected override decimal GetNext( decimal n ) => BitConverterHelper.Next( n );


        /// <inheritdoc />
        public override Criteria GetValidValue( out decimal value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin ); // GetNext because both are included

            var random = new Random();
            value = random.NextDecimal( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin );
                if( value >= max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( decimal value )
        {
            if( value == decimal.MinValue )
                return "decimal.MinValue";
            if( value == decimal.MaxValue )
                return "decimal.MaxValue";

            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Decimal" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for decimal from a decimal.
    /// </summary>
    public static class DecimalTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> Range( this decimal @this, decimal min, decimal max ) => SmartTest.Decimal.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> Range( this decimal @this, decimal min, bool minIncluded, decimal max, bool maxIncluded ) => SmartTest.Decimal.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> AboveOrEqual( this decimal @this, decimal min ) => SmartTest.Decimal.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> Above( this decimal @this, decimal min ) => SmartTest.Decimal.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> BelowOrEqual( this decimal @this, decimal max ) => SmartTest.Decimal.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> Below( this decimal @this, decimal max ) => SmartTest.Decimal.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A decimal we do not care about, except to know to create a <see cref="INumericType{T}" /> for decimal.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for decimal so that adding chunks can be chained.</returns>
        public static INumericType<decimal> Value( this decimal @this, decimal value ) => SmartTest.Decimal.Value( value );
    }
}