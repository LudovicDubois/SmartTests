using System;



namespace SmartTests.Helpers
{
    internal static class RandomHelper
    {
        public static byte NextByte( this Random rnd ) => rnd.NextByte( byte.MinValue, byte.MaxValue );
        public static byte NextByte( this Random rnd, byte min, byte max ) => (byte)rnd.Next( min, max );


        public static sbyte NextSByte( this Random rnd ) => rnd.NextSByte( sbyte.MinValue, sbyte.MaxValue );
        public static sbyte NextSByte( this Random rnd, sbyte min, sbyte max ) => (sbyte)rnd.Next( min, max );


        public static short NextInt16( this Random rnd ) => rnd.NextInt16( short.MinValue, short.MaxValue );
        public static short NextInt16( this Random rnd, short min, short max ) => (short)rnd.Next( min, max );


        public static ushort NextUInt16( this Random rnd ) => rnd.NextUInt16( ushort.MinValue, ushort.MaxValue );
        public static ushort NextUInt16( this Random rnd, ushort min, ushort max ) => (ushort)rnd.Next( min, max );


        public static uint NextUInt32( this Random rnd ) => rnd.NextUInt32( uint.MinValue, uint.MaxValue );
        public static uint NextUInt32( this Random rnd, uint min, uint max ) => (uint)rnd.NextInt64( min, max );


        public static long NextInt64( this Random rnd )
        {
            var buffer = new byte[8];
            rnd.NextBytes( buffer );
            return BitConverter.ToInt64( buffer, 0 );
        }


        // TODO: not perfectly distributed, but we do not care for tests. If someone want to propose a better way... <G>
        public static long NextInt64( this Random rnd, long minValue, long maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( nameof(minValue), "minValue should be less or equal to maxValue" );

            var min = new Decimal( minValue );
            var max = new Decimal( maxValue );
            return (long)( rnd.NextInt64() % ( max - min ) + min );
        }


        public static ulong NextUInt64( this Random rnd )
        {
            var buffer = new byte[8];
            rnd.NextBytes( buffer );
            return BitConverter.ToUInt64( buffer, 0 );
        }


        // TODO: not perfectly distributed, but we do not care for tests. If someone want to propose a better way... <G>
        public static ulong NextUInt64( this Random rnd, ulong minValue, ulong maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( nameof(minValue), "minValue should be less or equal to maxValue" );

            var min = new Decimal( minValue );
            var max = new Decimal( maxValue );
            return (ulong)( rnd.NextUInt64() % ( max - min ) + min );
        }


        public static float NextSingle( this Random rnd ) => rnd.NextSingle( float.MinValue, float.MaxValue );
        public static float NextSingle( this Random rnd, float minValue, float maxValue ) => (float)rnd.NextDouble() * ( maxValue - minValue ) + minValue;


        public static double NextDouble( this Random rnd, double minValue, double maxValue ) => rnd.NextDouble() * (maxValue - minValue) + minValue;

        public static decimal NextDecimal( this Random rnd ) => rnd.NextDecimal( decimal.MinValue, decimal.MaxValue );


        public static decimal NextDecimal( this Random rnd, decimal minValue, decimal maxValue )
        {
            decimal result;
            do
            {
                // The high bits of 0.9999999999999999999999999999m are 542101086.
                result = new decimal( rnd.Next(), rnd.Next(), rnd.Next( 542101087 ), false, 28 );
            } while( result >= 1 );

            return result * ( maxValue - minValue ) + minValue;
        }


        public static DateTime NextDateTime( this Random rnd ) => rnd.NextDateTime( DateTime.MinValue, DateTime.MaxValue );


        public static DateTime NextDateTime( this Random rnd, DateTime minValue, DateTime maxValue )
        {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException( nameof(minValue), "minValue should be less or equal to maxValue" );

            return new DateTime( rnd.NextInt64( minValue.Ticks, maxValue.Ticks ) );
        }
    }
}