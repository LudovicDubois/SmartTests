using System;
using System.Text;

using SmartTests.Criterias;
using SmartTests.Helpers;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of short values (with several chunks)
    /// </summary>
    public class ShortType: NumericType<short, ShortType>
    {
        /// <inheritdoc />
        protected override short MinValue => short.MinValue;
        /// <inheritdoc />
        protected override short MaxValue => short.MaxValue;


        /// <inheritdoc />
        protected override short GetPrevious( short n ) => (short)( n - 1 );


        /// <inheritdoc />
        protected override short GetNext( short n ) => (short)( n + 1 );


        /// <inheritdoc />
        public override Criteria GetValidValue( out short value )
        {
            // Ensure values are well distributed
            int max = short.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;

            var random = new Random();
            var val = random.Next( short.MinValue, max );
            max = short.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if( val > max )
                    continue;
                value = (short)(val - min + chunk.Min);
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "Short" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({( chunk.Min == short.MinValue ? "short.MinValue" : chunk.Min.ToString() )}, {( chunk.Max == short.MaxValue ? "short.MaxValue" : chunk.Max.ToString() )})" );
            return result.ToString();
        }
    }
}