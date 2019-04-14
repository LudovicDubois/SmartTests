using System;

using SmartTests.Criterias;



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
        public override Criteria GetValidValue( out sbyte value )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = (sbyte)random.Next( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( value > max )
                    continue;
                value = (sbyte)( value - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
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
        public override string ToString() => ToString( "SByte" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for sbyte from a sbyte.
    /// </summary>
    public static class SByteTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> Range( this sbyte @this, sbyte min, sbyte max ) => SmartTest.SByte.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> Range( this sbyte @this, sbyte min, bool minIncluded, sbyte max, bool maxIncluded ) => SmartTest.SByte.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> AboveOrEqual( this sbyte @this, sbyte min ) => SmartTest.SByte.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above max
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> Above( this sbyte @this, sbyte min ) => SmartTest.SByte.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> BelowOrEqual( this sbyte @this, sbyte max ) => SmartTest.SByte.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> Below( this sbyte @this, sbyte max ) => SmartTest.SByte.Below( max );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to min
        /// </summary>
        /// <param name="this">A sbyte we do not care about, except to know to create a <see cref="INumericType{T}" /> for sbyte.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for sbyte so that adding chunks can be chained.</returns>
        public static INumericType<sbyte> Value( this sbyte @this, sbyte value ) => SmartTest.SByte.Value( value );
    }
}