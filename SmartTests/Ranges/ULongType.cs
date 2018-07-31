using System;
using System.Text;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class ULongType: NumericType<ulong, ULongType>
    {
        /// <inheritdoc />
        protected override ulong MinValue => ulong.MinValue;
        /// <inheritdoc />
        protected override ulong MaxValue => ulong.MaxValue;


        /// <inheritdoc />
        protected override ulong GetPrevious( ulong n ) => n - 1;


        /// <inheritdoc />
        protected override ulong GetNext( ulong n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out ulong value )
        {
            // Ensure values are well distributed
            var max = ulong.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;

            var random = new Random();
            value = random.NextULong( ulong.MinValue, max );
            max = ulong.MinValue;
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




        private static string ToString( ulong n )
        {
            if( n == ulong.MaxValue )
                return "ulong.MaxValue";
            return n.ToString();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "ULong" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({ToString( chunk.Min )}, {ToString( chunk.Max )})" );
            return result.ToString();
        }
    }
}