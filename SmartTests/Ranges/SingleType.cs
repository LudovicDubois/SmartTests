using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


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
            // Impossible to ensure well distributed values (because of MinValue and MaxValue and the imprecision of double)
            // => choose a random chunk, then a random value in the chunk
            var random = new Random();
            var index = random.Next( Chunks.Count );
            var chunk = Chunks[ index ];

            while( true )
            {
                value = (float)random.NextDouble( chunk.IncludedMin, chunk.IncludedMax );
                // Ensure it is valid for the chunk (because of the representation error)
                if( value < chunk.IncludedMin )
                    continue;
                if( value > chunk.IncludedMax )
                    continue;
                if( !avoidedValues.Contains( value ) )
                    return AnyValue.IsValid;
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
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> Range( this float _, float min, float max ) => SmartTest.SingleRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> Range( this float _, float min, bool minIncluded, float max, bool maxIncluded ) => SmartTest.SingleRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> AboveOrEqual( this float _, float min ) => SmartTest.SingleRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> Above( this float _, float min ) => SmartTest.SingleRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> BelowOrEqual( this float _, float max ) => SmartTest.SingleRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> Below( this float _, float max ) => SmartTest.SingleRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<float> Value( this float _, float value ) => SmartTest.SingleRange.Value( value );
    }
}