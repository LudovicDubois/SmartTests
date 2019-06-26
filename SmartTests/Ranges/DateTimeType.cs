using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of DateTime values (with several chunks)
    /// </summary>
    public class DateTimeType: NumericType<DateTime, DateTimeType>
    {
        /// <inheritdoc />
        protected override DateTime MinValue => DateTime.MinValue;
        /// <inheritdoc />
        protected override DateTime MaxValue => DateTime.MaxValue;


        /// <inheritdoc />
        protected override DateTime GetPrevious( DateTime n ) => n.AddTicks( -1 );


        /// <inheritdoc />
        protected override DateTime GetNext( DateTime n ) => n.AddTicks( 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out DateTime value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax.AddTicks( 1 ) - chunk.IncludedMin; // +1 because both are included

            var random = new Random();
            value = random.NextDateTime( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax.AddTicks( 1 ) - chunk.IncludedMin;
                if( value >= max )
                    continue;
                value = chunk.IncludedMin + ( value - min );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( DateTime value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "DateTimeRange" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for DateTime from a DateTime.
    /// </summary>
    public static class DateTimeTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> Range( this DateTime @this, DateTime min, DateTime max ) => SmartTest.DateTimeRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> Range( this DateTime @this, DateTime min, bool minIncluded, DateTime max, bool maxIncluded ) => SmartTest.DateTimeRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> AboveOrEqual( this DateTime @this, DateTime min ) => SmartTest.DateTimeRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> Above( this DateTime @this, DateTime min ) => SmartTest.DateTimeRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> BelowOrEqual( this DateTime @this, DateTime max ) => SmartTest.DateTimeRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> Below( this DateTime @this, DateTime max ) => SmartTest.DateTimeRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">
        ///     A DateTime we do not care about, except to know to create a <see cref="INumericType{T}" /> for
        ///     DateTime.
        /// </param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for DateTime so that adding chunks can be chained.</returns>
        public static INumericType<DateTime> Value( this DateTime @this, DateTime value ) => SmartTest.DateTimeRange.Value( value );
    }
}