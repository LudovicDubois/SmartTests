using System;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class ByteType: NumericType<byte, ByteType>
    {
        /// <inheritdoc />
        protected override byte MinValue => byte.MinValue;
        /// <inheritdoc />
        protected override byte MaxValue => byte.MaxValue;


        /// <inheritdoc />
        protected override byte GetPrevious( byte n ) => (byte)( n - 1 );


        /// <inheritdoc />
        protected override byte GetNext( byte n ) => (byte)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out byte value )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            value = (byte)random.Next( MinValue, max );
            if( max == MaxValue )
                return AnyValue.IsValid;

            max = MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max;
                max += chunk.IncludedMax - chunk.IncludedMin + 1;
                if( value >= max )
                    continue;
                value = (byte)( value - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( byte value )
        {
            if( value == byte.MaxValue )
                return "byte.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Byte" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for byte from a byte.
    /// </summary>
    public static class ByteTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> Range( this byte @this, byte min, byte max ) => SmartTest.Byte.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> Range( this byte @this, byte min, bool minIncluded, byte max, bool maxIncluded ) => SmartTest.Byte.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> AboveOrEqual( this byte @this, byte min ) => SmartTest.Byte.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> Above( this byte @this, byte min ) => SmartTest.Byte.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> BelowOrEqual( this byte @this, byte max ) => SmartTest.Byte.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> Below( this byte @this, byte max ) => SmartTest.Byte.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="this">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="value">A random value within this range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        public static INumericType<byte> Value( this byte @this, byte value ) => SmartTest.Byte.Value( value );
    }
}