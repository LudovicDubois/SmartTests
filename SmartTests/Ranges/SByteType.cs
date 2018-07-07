using System;
using System.Text;

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
        public override Criteria GetValue( out sbyte value )
        {
            // Ensure values are well distributed
            int max = sbyte.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            value = (sbyte)random.Next( sbyte.MinValue, max );
            max = sbyte.MinValue;
            foreach( var chunk in Chunks )
            {
                var min = max + 1;
                max += chunk.Max - chunk.Min;
                if( value > max )
                    continue;
                value = (sbyte)( value - min + chunk.Min );
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "SByte" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({( chunk.Min == sbyte.MinValue ? "sbyte.MinValue" : chunk.Min.ToString() )}, {( chunk.Max == sbyte.MaxValue ? "sbyte.MaxValue" : chunk.Max.ToString() )})" );
            return result.ToString();
        }
    }
}