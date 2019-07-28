using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of float values (with several chunks)
    /// </summary>
    public class SingleType: NumericType<float, SingleType>
    {
        /// <inheritdoc />
        protected override float MinValue => float.MinValue;
        /// <inheritdoc />
        protected override float MaxValue => float.MaxValue;


        /// <inheritdoc />
        protected override float GetPrevious( float n ) => BitConverterHelper.Previous( n );


        /// <inheritdoc />
        protected override float GetNext( float n ) => BitConverterHelper.Next( n );


        /// <inheritdoc />
        public override Criteria GetValidValue( out float value, params float[] avoidedValues )
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
                    value = random.NextSingle();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextSingle( MinValue, max );

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
        protected override string ToString( float value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "SingleRange" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for float from a float.
    /// </summary>
    public static class SingleTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Range( this float @this, float min, float max ) => SmartTest.SingleRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Range( this float @this, float min, bool minIncluded, float max, bool maxIncluded ) => SmartTest.SingleRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> AboveOrEqual( this float @this, float min ) => SmartTest.SingleRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Above( this float @this, float min ) => SmartTest.SingleRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> BelowOrEqual( this float @this, float max ) => SmartTest.SingleRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Below( this float @this, float max ) => SmartTest.SingleRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Value( this float @this, float value ) => SmartTest.SingleRange.Value( value );
    }
}