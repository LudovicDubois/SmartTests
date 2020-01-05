using System;
using System.Linq;

using SmartTests.Criterias;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


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
        public override Criteria GetValidValue( out byte value, params byte[] avoidedValues )
        {
            // Ensure values are well distributed
            int max = MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin + 1; // +1 because both are included

            var random = new Random();
            if( max == MaxValue )
                while( true )
                {
                    value = random.NextByte();
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }

            while( true )
            {
                var val = random.NextByte( MinValue, (byte)max );

                max = MinValue;
                foreach( var chunk in Chunks )
                {
                    var min = max;
                    max += chunk.IncludedMax - chunk.IncludedMin + 1;
                    if( val >= max )
                        continue;
                    value = (byte)( val - min + chunk.IncludedMin );
                    if( !avoidedValues.Contains( value ) )
                        return AnyValue.IsValid;
                }
            }
        }


        /// <inheritdoc />
        protected override string ToString( byte value ) => SmartTest.ToString( value );


        /// <inheritdoc />
        public override string ToString() => ToString( "ByteRange" );
    }


    /// <summary>
    ///     A helper type to create <see cref="INumericType{T}" /> for byte from a byte.
    /// </summary>
    public static class ByteTypeHelper
    {
        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> Range( this byte _, byte min, byte max ) => SmartTest.ByteRange.Range( min, max );


        /// <summary>
        ///     Adds a chunk of numeric values
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (included) of the chunk.</param>
        /// <param name="minIncluded"><c>true</c> to include min, <c>false</c> otherwise.</param>
        /// <param name="max">The max value (included) of the chunk.</param>
        /// <param name="maxIncluded"><c>true</c> to include max, <c>false</c> otherwise.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> Range( this byte _, byte min, bool minIncluded, byte max, bool maxIncluded ) => SmartTest.ByteRange.Range( min, minIncluded, max, maxIncluded );


        /// <summary>
        ///     Adds a chunk of numeric values above a min
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (included) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> AboveOrEqual( this byte _, byte min ) => SmartTest.ByteRange.AboveOrEqual( min );


        /// <summary>
        ///     Adds a chunk of numeric values above min
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="min">The min value (excluded) of the created chunk.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> Above( this byte _, byte min ) => SmartTest.ByteRange.Above( min );


        /// <summary>
        ///     Adds a chunk of numeric values below or equal to max
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> BelowOrEqual( this byte _, byte max ) => SmartTest.ByteRange.BelowOrEqual( max );


        /// <summary>
        ///     Adds a chunk of numeric values below max
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> Below( this byte _, byte max ) => SmartTest.ByteRange.Below( max );


        /// <summary>
        ///     Adds a chunk of one numeric value
        /// </summary>
        /// <param name="_">A byte we do not care about, except to know to create a <see cref="INumericType{T}" /> for byte.</param>
        /// <param name="value">A random value within _ range.</param>
        /// <returns>Return a new <see cref="INumericType{T}" /> for byte so that adding chunks can be chained.</returns>
        // ReSharper disable once UnusedParameter.Global
        public static INumericType<byte> Value( this byte _, byte value ) => SmartTest.ByteRange.Value( value );
    }
}