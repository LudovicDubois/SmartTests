using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;
// ReSharper disable UnusedMember.Global



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
        public override Criteria GetValidValue( out double value, params double[] avoidedValues )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin ); // GetNext because both are included

            var random = new Random();
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextDouble();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextDouble( MinValue, max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += GetNext( chunk.IncludedMax - chunk.IncludedMin );
                    if( val >= max )
                        continue;
                    value = val - min + chunk.IncludedMin;
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
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
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> Range( this double _, double min, double max ) => SmartTest.DoubleRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> Range( this double _, double min, bool minIncluded, double max, bool maxIncluded ) => SmartTest.DoubleRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> AboveOrEqual( this double _, double min ) => SmartTest.DoubleRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> Above( this double _, double min ) => SmartTest.DoubleRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> BelowOrEqual( this double _, double max ) => SmartTest.DoubleRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> Below( this double _, double max ) => SmartTest.DoubleRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A double we do not care about, except to know to create a <see cref="INumericType{T}" /> for double.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for double so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<double> Value( this double _, double value ) => SmartTest.DoubleRange.Value( value );
    }
}