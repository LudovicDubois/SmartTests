using System;
using System.Text;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class UIntType: NumericType<uint, UIntType>
    {
        /// <inheritdoc />
        protected override uint MinValue => uint.MinValue;
        /// <inheritdoc />
        protected override uint MaxValue => uint.MaxValue;


        /// <inheritdoc />
        protected override uint GetPrevious( uint n ) => n - 1;


        /// <inheritdoc />
        protected override uint GetNext( uint n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out uint value )
        {
            // Ensure values are well distributed
            var max = uint.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            value = (uint)random.NextLong( uint.MinValue, max );
            max = uint.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if( value > max )
                    continue;
                value = value - min + chunk.Min;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }



        private static string ToString( uint n )
        {
            if( n == uint.MaxValue )
                return "uint.MaxValue";
            return n.ToString();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "UInt" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({ToString( chunk.Min )}, {ToString( chunk.Max )})" );
            return result.ToString();
        }
    }
}