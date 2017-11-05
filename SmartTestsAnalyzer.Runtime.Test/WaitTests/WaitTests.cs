using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.WaitTests
{
    [TestFixture]
    public class WaitTests
    {
        public class MyClass
        {
            public MyClass( int wait )
            {
                _Wait = wait;
            }


            private readonly int _Wait;

            public bool Done { get; private set; }


            public void Method( Action action ) => Task.Factory.StartNew( () =>
                                                                          {
                                                                              Thread.Sleep( _Wait );
                                                                              action();
                                                                              Done = true;
                                                                          } );
        }


        [Test]
        public void OK()
        {
            var handle = new AutoResetEvent( false );
            var mc = new MyClass( 200 );

            RunTest( AnyValue.IsValid,
                     () => mc.Method( () => handle.Set() ),
                     SmartAssert.Within( 100 ),
                     SmartAssert.Wait( handle, 1000 ) );

            Assert.IsTrue( mc.Done );
        }


        [Test]
        public void Timeout()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var handle = new AutoResetEvent( false );
                                                                  var mc = new MyClass( 200 );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method( () => handle.Set() ),
                                                                           SmartAssert.Within( 100 ),
                                                                           SmartAssert.Wait( handle, 150 ) );

                                                                  Assert.IsTrue( mc.Done );
                                                              } );

            Assert.AreEqual( "Timeout reached (150ms)", exception.Message );
        }
    }
}