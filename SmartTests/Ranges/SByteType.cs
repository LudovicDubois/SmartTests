﻿using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class SByteType: NumericType<sbyte, SByteType>
    {
        /// <inheritdoc />
        protected override sbyte MinValue => sbyte.MinValue;
        /// <inheritdoc />
        protected override sbyte MaxValue => sbyte.MaxValue;


        /// <inheritdoc />
        protected override sbyte GetPrevious( sbyte n ) => (sbyte)( n - 1 );


        /// <inheritdoc />
        protected override sbyte GetNext( sbyte n ) => (sbyte)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out sbyte value, params sbyte[] avoidedValues )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextSByte();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextSByte( MinValue, (sbyte)max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += chunk.IncludedMax - chunk.IncludedMin + 1;
                    if( val > max )
                        continue;
                    value = (sbyte)( val - min + chunk.IncludedMin );
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
        }


        /// <inheritdoc />
        protected override string ToString( sbyte value )
        {
            if( value == sbyte.MinValue )
                return "sbyte.MinValue";
            if( value == sbyte.MaxValue )
                return "sbyte.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "SByteRange" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for sbyte from a sbyte.
    /// </summary>
    public static class SByteTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> Range( this sbyte _, sbyte min, sbyte max ) => SmartTest.SByteRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> Range( this sbyte _, sbyte min, bool minIncluded, sbyte max, bool maxIncluded ) => SmartTest.SByteRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> AboveOrEqual( this sbyte _, sbyte min ) => SmartTest.SByteRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> Above( this sbyte _, sbyte min ) => SmartTest.SByteRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> BelowOrEqual( this sbyte _, sbyte max ) => SmartTest.SByteRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> Below( this sbyte _, sbyte max ) => SmartTest.SByteRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<sbyte> Value( this sbyte _, sbyte value ) => SmartTest.SByteRange.Value( value );
    }
}