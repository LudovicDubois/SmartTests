using System;
using System.Text;

using SmartTests.Criterias;



namespace SmartTests.Ranges
{
    /// <summary>
    ///     Represents a Range of integer values (with several chunks)
    /// </summary>
    public class IntType: NumericType<int, IntType>
    {
        /// <inheritdoc />
        protected override int MinValue => int.MinValue;
        /// <inheritdoc />
        protected override int MaxValue => int.MaxValue;


        /// <inheritdoc />
        protected override int GetPrevious( int n ) => n - 1;


        /// <inheritdoc />
        protected override int GetNext( int n ) => n + 1;


        /// <inheritdoc />
        public override Criteria GetValidValue( out int value )
        {
            // Ensure values are well distributed
            var max = int.MinValue;
            foreach( var chunk in Chunks )
                max += chunk.Max - chunk.Min;
            var random = new Random();

            if( max == int.MaxValue )
            {
                value = random.Next();
                return AnyValue.IsValid;
            }

            value = random.Next( int.MinValue, max );
            max = int.MinValue;
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


        private static string ToString( int n )
        {
            if( n == int.MinValue )
                return "int.MinValue";
            if( n == int.MaxValue )
                return "int.MaxValue";
            return n.ToString();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder( "Int" );
            foreach( var chunk in Chunks )
                result.Append( $".Range({ToString( chunk.Min )}, {ToString( chunk.Max )})" );
            return result.ToString();
        }
    }
}