using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of short values (with several chunks)
    /// </summary>
    public class ShortType: NumericType<short, ShortType>
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
        public override Criteria GetValidValue( out short value )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            var val = random.Next( MinValue, max );
            if( max == MaxValue )
            {
                value = (short)val;
                return AnyValue.IsValid;
            }

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( val >= max )
                    continue;
                value = (short)( val - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( short value )
        {
            if( value == short.MinValue )
                return "short.MinValue";
            if( value == short.MaxValue )
                return "short.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Short" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for short from a short.
    /// </summary>
    public static class ShortTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> Range( this short @this, short min, short max ) => SmartTest.Short.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> Range( this short @this, short min, bool minIncluded, short max, bool maxIncluded ) => SmartTest.Short.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> AboveOrEqual( this short @this, short min ) => SmartTest.Short.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above max
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> Above( this short @this, short min ) => SmartTest.Short.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> BelowOrEqual( this short @this, short max ) => SmartTest.Short.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> Below( this short @this, short max ) => SmartTest.Short.Below( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A short we do not care about, except to know to create a <see cref="INumericType{T}" /> for short.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for short so that adding chunks can be chained.</returns>
        public static INumericType<short> Value( this short @this, short value ) => SmartTest.Short.Value( value );
    }
}