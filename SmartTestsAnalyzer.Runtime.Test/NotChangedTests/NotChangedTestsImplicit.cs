using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTestsImplicit
    {
        public class MyClass
        {
            public MyClass( bool change1, bool change2, bool change3, bool change4 )
            {
                _Change1 = change1;
                _Change2 = change2;
                _Change3 = change3;
                _Change4 = change4;
            }


            private readonly bool _Change1;
            private readonly bool _Change2;
            private readonly bool _Change3;
            private readonly bool _Change4;


            public int MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }
            private int MyProperty3 { get; set; }
            protected int MyProperty4 { get; set; }


            public void Method()
            {
                if( _Change1 )
                    MyProperty1 = 1;
                if( _Change2 )
                    MyProperty2 = 2;
                if( _Change3 )
                    MyProperty3 = 3;
                if( _Change4 )
                    MyProperty4 = 4;
            }
        }


        [Test]
        public void NotChangedPublic()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.NotChanged() );
        }


        [Test]
        public void NotChangedNonPublic()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.NonPublicProperties ) );
        }


        [Test]
        public void NotChangedAll()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.AllProperties ) );
        }


        [Test]
        public void Property1ChangedPublic()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged() );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsImplicit+MyClass.MyProperty1' has changed", exception.Message );
        }


        [Test]
        public void Property1ChangedNonPublic()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.NonPublicProperties ) );
        }


        [Test]
        public void Property1ChangedAll()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsImplicit+MyClass.MyProperty1' has changed", exception.Message );
        }


        [Test]
        public void Property3ChangedPublic()
        {
            var mc = new MyClass( false, false, true, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.NotChanged() );
        }


        [Test]
        public void Property3ChangedNonPublic()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.NonPublicProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsImplicit+MyClass.MyProperty3' has changed", exception.Message );
        }


        [Test]
        public void Property3ChangedAll()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsImplicit+MyClass.MyProperty3' has changed", exception.Message );
        }
    }
}