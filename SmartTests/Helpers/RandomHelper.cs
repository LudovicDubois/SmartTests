using System;



namespace SmartTests.Helpers
{
    public static class RandomHelper
    {
        public static long NextLong( this Random rnd )
        {
            var buffer = new byte[8];
            rnd.NextBytes( buffer );
            return BitConverter.ToInt64( buffer, 0 );
        }


        // TODO: not perfectly distributed, but we do not care for tests. If someone want to propose a better way... <G>
        public static long NextLong( this Random rnd, long minValue, long maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( "minValue", "minValue should be less or equal to maxValue" );

            var min = new Decimal( minValue );
            var max = new Decimal( maxValue );
            return (long)( rnd.NextLong() % ( max - min ) + min );
        }
    }
}