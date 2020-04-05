using System;
using System.Threading.Tasks;

using NUnit.Framework;

using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTests.Test
{
    [TestFixture]
    class AsyncTests
    {
        public static async Task<int> GetNumberAsync( int number )
        {
            await Task.Delay( 100 );

            if( number < 0 )
                throw new ArgumentOutOfRangeException();
            return number;
        }


        [Test]
        public void Result()
        {
            var result = RunTest( AnyValue.IsValid,
                                  () => GetNumberAsync( 10 ).Result );

            Assert.AreEqual( 10, result );
        }


        [Test]
        public void Exception()
        {
            RunTest( AnyValue.IsValid,
                     () => GetNumberAsync( -10 ).Result,
                     SmartAssert.Throw( ( AggregateException e ) =>
                                        {
                                            Assert.AreEqual( 1, e.InnerExceptions.Count );
                                            Assert.IsAssignableFrom( typeof(ArgumentOutOfRangeException), e.InnerExceptions[ 0 ] );
                                        } ) );
        }
    }
}