using System;
using System.Text;

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
        public override Criteria GetValue( out byte value )
        {
            // Ensure values are well distributed
            int max = byte.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            value = (byte)random.Next( byte.MinValue, max );
            max = byte.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if( value > max )
                    continue;
                value = (byte)( value - min + chunk.Min );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "Byte" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({chunk.Min}, {( chunk.Max == byte.MaxValue ? "byte.MaxValue" : chunk.Max.ToString() )})" );
            return result.ToString();
        }
    }
}