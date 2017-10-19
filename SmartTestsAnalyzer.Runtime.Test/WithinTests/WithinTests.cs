using System.Threading;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.WithinTests
{
    [TestFixture]
    public class WithinTests
    {
        public class MyClass
        {
            public MyClass( int wait )
            {
                _Wait = wait;
            }


            private readonly int _Wait;


            public void Method() => Thread.Sleep( _Wait );
        }


        [Test]
        public void OK()
        {
            var mc = new MyClass( 10 );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Within( 100 ) );
        }


        [Test]
        public void Fail()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( 100 );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Within( 90 ) );
                                                              } );

            StringAssert.IsMatch( @"Should be less than 90ms, but was \d+ms", exception.Message );
        }


        [Test]
        public void NegativeTimeout()
        {
            var exception = Assert.Catch<BadTestException>( () =>
                                                            {
                                                                var mc = new MyClass( 100 );

                                                                RunTest( AnyValue.IsValid,
                                                                         () => mc.Method(),
                                                                         SmartAssert.Within( -90 ) );
                                                            } );

            Assert.AreEqual( @"BAD TEST: Time should be strictly positive, but was -90ms", exception.Message );
        }
    }
}