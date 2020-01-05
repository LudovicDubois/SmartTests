using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class Int64Type: NumericType<long, Int64Type>
    {
        /// <inheritdoc />
        protected override long MinValue => long.MinValue;
        /// <inheritdoc />
        protected override long MaxValue => long.MaxValue;


        /// <inheritdoc />
        protected override long GetPrevious( long n ) => n - 1;


        /// <inheritdoc />
        protected override long GetNext( long n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out long value, params long[] avoidedValues )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextInt64();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextInt64( MinValue, max );

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
        protected override string ToString( long value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "Int64Range" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for long from a long.
    /// </summary>
    public static class Int64TypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> Range( this long _, long min, long max ) => SmartTest.Int64Range.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> Range( this long _, long min, bool minIncluded, long max, bool maxIncluded ) => SmartTest.Int64Range.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> AboveOrEqual( this long _, long min ) => SmartTest.Int64Range.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> Above( this long _, long min ) => SmartTest.Int64Range.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> BelowOrEqual( this long _, long max ) => SmartTest.Int64Range.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> Below( this long _, long max ) => SmartTest.Int64Range.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A long we do not care about, except to know to create a <see cref="INumericType{T}" /> for long.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for long so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<long> Value( this long _, long value ) => SmartTest.Int64Range.Value( value );
    }
}