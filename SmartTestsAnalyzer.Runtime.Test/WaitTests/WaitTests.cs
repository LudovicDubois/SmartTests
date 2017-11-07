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
            public MyClass( int wait, bool generateException = false )
            {
                _Wait = wait;
                _GenerateException = generateException;
            }


            private readonly int _Wait;
            private readonly bool _GenerateException;

            public bool Done { get; private set; }
            public Exception Exception { get; private set; }


            public void Method( Action action ) => Task.Factory.StartNew( () =>
                                                                          {
                                                                              try
                                                                              {
                                                                                  if( _GenerateException )
                                                                                  {
                                                                                      Thread.Sleep( _Wait / 2 );
                                                                                      throw new Exception( "OOPS!" );
                                                                                  }
                                                                                  Thread.Sleep( _Wait );
                                                                                  Done = true;
                                                                                  action();
                                                                              }
                                                                              catch( Exception e )
                                                                              {
                                                                                  Exception = e;
                                                                              }
                                                                          } );
        }


        [Test]
        public void OK()
        {
            var handle = new AutoResetEvent( false );
            var mc = new MyClass( 300 );

            RunTest( AnyValue.IsValid,
                     () => mc.Method( () => handle.Set() ),
                     SmartAssert.Within( 100 ),
                     SmartAssert.Wait( handle, 1000 ) );

            Assert.IsTrue( mc.Done );
            Assert.IsNull( mc.Exception );
        }


        [Test]
        public void Timeout()
        {
            var mc = new MyClass( 300 );
            var handle = new AutoResetEvent( false );

            var exception = Assert.Catch<SmartTestException>( () =>
                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method( () => handle.Set() ),
                                                                           SmartAssert.Within( 100 ),
                                                                           SmartAssert.Wait( handle, 200 ) )
                                                            );

            Assert.IsFalse( mc.Done );
            Assert.IsNull( mc.Exception );
            Assert.AreEqual( "Timeout reached (200ms)", exception.Message );
        }


        [Test]
        public void Exception()
        {
            var mc = new MyClass( 300, true );
            var handle = new AutoResetEvent( false );

            var exception = Assert.Catch<SmartTestException>( () =>
                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method( () => handle.Set() ),
                                                                           SmartAssert.Within( 100 ),
                                                                           SmartAssert.Wait( handle, 200 ) )
                                                            );

            Assert.IsFalse( mc.Done );
            Assert.AreEqual( "OOPS!", mc.Exception.Message );
            Assert.AreEqual( "Timeout reached (200ms)", exception.Message );
        }


        [Test]
        public void Context_OK()
        {
            var mc = new MyClass( 300 );

            RunTest( AnyValue.IsValid,
                     ctx => mc.Method( ctx.SetHandle ),
                     SmartAssert.Within( 100 ),
                     SmartAssert.WaitContextHandle( 1000 ) );

            Assert.IsTrue( mc.Done );
            Assert.IsNull( mc.Exception );
        }


        [Test]
        public void Context_Timeout()
        {
            var mc = new MyClass( 300 );

            var exception = Assert.Catch<SmartTestException>( () =>
                                                                  RunTest( AnyValue.IsValid,
                                                                           ctx => mc.Method( ctx.SetHandle ),
                                                                           SmartAssert.Within( 100 ),
                                                                           SmartAssert.WaitContextHandle( 200 ) )
                                                            );

            Assert.IsFalse( mc.Done );
            Assert.IsNull( mc.Exception );
            Assert.AreEqual( "Timeout reached (200ms)", exception.Message );
        }


        [Test]
        public void NoWaitHandle()
        {
            var handle = new AutoResetEvent( false );
            var mc = new MyClass( 300 );

            var exception = Assert.Catch<SmartTestException>( () =>
                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method( () => handle.Set() ),
                                                                           SmartAssert.Within( 100 ),
                                                                           SmartAssert.WaitContextHandle( 500 ) )
                                                            );

            Assert.IsTrue( mc.Done );
            Assert.IsNull( mc.Exception );
            Assert.AreEqual( "Timeout reached (500ms)", exception.Message );
        }


        [Test]
        public void NoContextHandle()
        {
            var handle = new AutoResetEvent( false );
            var mc = new MyClass( 300 );

            var exception = Assert.Catch<BadTestException>( () =>
                                                                RunTest( AnyValue.IsValid,
                                                                         ctx => mc.Method( ctx.SetHandle ),
                                                                         SmartAssert.Within( 100 ),
                                                                         SmartAssert.Wait( handle, 500 ) )
                                                          );

            Assert.IsTrue( mc.Done );
            Assert.AreEqual( "BAD TEST: ActContext.SetHandle called, but specified handle expected", mc.Exception.Message );
            Assert.AreEqual( "BAD TEST: ActContext.SetHandle called, but specified handle expected", exception.Message );
        }
    }
}