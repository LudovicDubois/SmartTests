using System;

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
                max += chunk.IncludedMax - chunk.IncludedMin;
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
                max += chunk.IncludedMax - chunk.IncludedMin;
                if( value > max )
                    continue;
                value = value - min + chunk.IncludedMin;
                return AnyValue.IsValid;
            }

            throw new NotImplementedException();
        }


        /// <inheritdoc />
        protected override string ToString( int value )
        {
            if( value == int.MinValue )
                return "int.MinValue";
            if( value == int.MaxValue )
                return "int.MaxValue";
            return value.ToString();
        }


        /// <inheritdoc />
        public override string ToString() => ToString( "Int" );
    }
}