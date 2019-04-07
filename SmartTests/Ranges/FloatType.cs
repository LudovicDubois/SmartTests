﻿using System;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of float values (with several chunks)
    /// </summary>
    public class FloatType: NumericType<float, FloatType>
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
        public override Criteria GetValidValue( out float value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += GetNext( chunk.IncludedMax - chunk.IncludedMin ); // GetNext because both are included

            var random = new Random();
            value = (float)( random.NextDouble() * ( max - MinValue ) + MinValue );
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
        protected override string ToString( float value )
        {
            if( value == float.MinValue )
                return "float.MinValue";
            if( value == float.MaxValue )
                return "float.MaxValue";

            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Float" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for float from a float.
    /// </summary>
    public static class FloatTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Range( this float @this, float min, float max ) => SmartTest.Float.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Range( this float @this, float min, bool minIncluded, float max, bool maxIncluded ) => SmartTest.Float.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> AboveOrEqual( this float @this, float min ) => SmartTest.Float.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above max
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Above( this float @this, float min ) => SmartTest.Float.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> BelowOrEqual( this float @this, float max ) => SmartTest.Float.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Below( this float @this, float max ) => SmartTest.Float.Below( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A float we do not care about, except to know to create a <see cref="INumericType{T}" /> for float.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for float so that adding chunks can be chained.</returns>
        public static INumericType<float> Value( this float @this, float value ) => SmartTest.Float.Value( value );
    }
}