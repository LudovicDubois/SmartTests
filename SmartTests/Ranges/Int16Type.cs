using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of short values (with several chunks)
    /// </summary>
    public class Int16Type: NumericType<short, Int16Type>
    {
        /// <inheritdoc />
        protected override short MinValue => short.MinValue;
        /// <inheritdoc />
        protected override short MaxValue => short.MaxValue;


        /// <inheritdoc />
        protected override short GetPrevious( short n ) => (short)( n - 1 );


        /// <inheritdoc />
        protected override short GetNext( short n ) => (short)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out short value, params short[] avoidedValues )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextInt16();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.Next( MinValue, max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += chunk.IncludedMax - chunk.IncludedMin + 1;
                    if( val >= max )
                        continue;
                    value = (short)( val - min + chunk.IncludedMin );
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
        }


        /// <inheritdoc />
        protected override string ToString( short value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "Int16Range" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for short from a short.
    /// </summary>
    public static class Int16TypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> Range( this short _, short min, short max ) => SmartTest.Int16Range.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> Range( this short _, short min, bool minIncluded, short max, bool maxIncluded ) => SmartTest.Int16Range.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> AboveOrEqual( this short _, short min ) => SmartTest.Int16Range.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above max
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> Above( this short _, short min ) => SmartTest.Int16Range.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> BelowOrEqual( this short _, short max ) => SmartTest.Int16Range.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> Below( this short _, short max ) => SmartTest.Int16Range.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<short> Value( this short _, short value ) => SmartTest.Int16Range.Value( value );
    }
}