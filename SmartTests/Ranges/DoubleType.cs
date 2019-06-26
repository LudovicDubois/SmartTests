using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of double values (with several chunks)
    /// </summary>
    public class DoubleType: NumericType<double, DoubleType>
    {
        /// <inheritdoc />
        protected override double MinValue => double.MinValue;
        /// <inheritdoc />
        protected override double MaxValue => double.MaxValue;


        /// <inheritdoc />
        protected override double GetPrevious( double n ) => BitConverterHelper.Previous( n );


        /// <inheritdoc />
        protected override double GetNext( double n ) => BitConverterHelper.Next( n );


        /// <inheritdoc />
        public override Criteria GetValidValue( out double value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin ); // GetNext because both are included

            var random = new Random();
            value = random.NextDouble() * ( max - MinValue ) + MinValue;
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
        protected override string ToString( double value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "DoubleRange" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for double from a double.
    /// </summary>
    public static class DoubleTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> Range( this double @this, double min, double max ) => SmartTest.DoubleRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> Range( this double @this, double min, bool minIncluded, double max, bool maxIncluded ) => SmartTest.DoubleRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> AboveOrEqual( this double @this, double min ) => SmartTest.DoubleRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> Above( this double @this, double min ) => SmartTest.DoubleRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> BelowOrEqual( this double @this, double max ) => SmartTest.DoubleRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> Below( this double @this, double max ) => SmartTest.DoubleRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        public static INumericType<double> Value( this double @this, double value ) => SmartTest.DoubleRange.Value( value );
    }
}