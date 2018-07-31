using System;
using System.Text;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class UShortType: NumericType<ushort, UShortType>
    {
        /// <inheritdoc />
        protected override ushort MinValue => ushort.MinValue;
        /// <inheritdoc />
        protected override ushort MaxValue => ushort.MaxValue;


        /// <inheritdoc />
        protected override ushort GetPrevious( ushort n ) => (ushort)( n - 1 );


        /// <inheritdoc />
        protected override ushort GetNext( ushort n ) => (ushort)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out ushort value )
        {
            // Ensure values are well distributed
            int max = ushort.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            value = (ushort)random.Next( ushort.MinValue, max );
            max = ushort.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if( value > max )
                    continue;
                value = (ushort)( value - min + chunk.Min );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        private static string ToString( ushort n )
        {
            if( n == ushort.MaxValue )
                return "ushort.MaxValue";
            return n.ToString();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "UShort" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({ToString( chunk.Min )}, {ToString( chunk.Max )})" );
            return result.ToString();
        }
    }
}