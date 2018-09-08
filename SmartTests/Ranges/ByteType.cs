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
            int max = byte.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.IncludedMax - chunk.IncludedMin;
            var random = new Random();

            value = (byte)random.Next( byte.MinValue, max );
            max = byte.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.IncludedMax - chunk.IncludedMin;
                if( value > max )
                    continue;
                value = (byte)( value - min + chunk.IncludedMin );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( byte n )
        {
            if( n == byte.MaxValue )
                return "byte.MaxValue";
            return n.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Byte" );
    }
}