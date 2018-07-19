using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of long values (with several chunks)
    /// </summary>
    public class LongType: NumericType<long, LongType>
    {
        /// <inheritdoc />
        protected override long MinValue => long.MinValue;
        /// <inheritdoc />
        protected override long MaxValue => long.MaxValue;

        /// <inheritdoc />
        protected override long GetPrevious( long n ) => n - 1;
        /// <inheritdoc />
        protected override long GetNext( long n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out long value )
        {
            // Ensure values are well distributed
            var max = long.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            value = random.NextLong( long.MinValue, max );
            max = long.MinValue;
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


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "Long" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({( chunk.Min == long.MinValue ? "long.MinValue" : chunk.Min.ToString() )}, {( chunk.Max == long.MaxValue ? "long.MaxValue" : chunk.Max.ToString() )})" );
            return result.ToString();
        }
    }
}