using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class IntType: NumericType<int, IntType>
    {
        /// <inheritdoc />
        protected override int MinValue => int.MinValue;
        /// <inheritdoc />
        protected override int MaxValue => int.MaxValue;


        /// <inheritdoc />
        protected override int GetPrevious( int n ) => n - 1;


        /// <inheritdoc />
        protected override int GetNext( int n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out int value )
        {
            // Ensure values are well distributed
            var max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = random.Next( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( value >= max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( int value )
        {
            if( value == int.MinValue )
                return "int.MinValue";
            if( value == int.MaxValue )
                return "int.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Int" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for int from a int.
    /// </summary>
    public static class IntTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> Range( this int @this, int min, int max ) => SmartTest.Int.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> Range( this int @this, int min, bool minIncluded, int max, bool maxIncluded ) => SmartTest.Int.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> AboveOrEqual( this int @this, int min ) => SmartTest.Int.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> Above( this int @this, int min ) => SmartTest.Int.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> BelowOrEqual( this int @this, int max ) => SmartTest.Int.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> Below( this int @this, int max ) => SmartTest.Int.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A int we do not care about, except to know to create a <see cref="INumericType{T}" /> for int.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for int so that adding chunks can be chained.</returns>
        public static INumericType<int> Value( this int @this, int value ) => SmartTest.Int.Value( value );
    }
}