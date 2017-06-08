using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTestsAll
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
                     SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
        }


        [Test]
        public void Property1Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
        }


        [Test]
        public void Property2Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, true, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsAll+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void Property1Property2Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void Property3Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsAll+MyClass.MyProperty3' has changed", exception.Message );
        }


        [Test]
        public void Property1Property3Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, true, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty3' has changed", exception.Message );
        }


        [Test]
        public void Property1Property2Property3Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty2' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty3' has changed", exception.Message );
        }


        [Test]
        public void Property4Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, false, true );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsAll+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void Property1Property4Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, true );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void Property1Property2Property4Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, false, true );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty2' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void Property1Property2Property3Property4Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty1' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty2' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty3' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsAll+MyClass.MyProperty4' has changed", exception.Message );
        }
    }
}