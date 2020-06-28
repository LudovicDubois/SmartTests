using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTestsExcept
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


            // ReSharper disable UnusedAutoPropertyAccessor.Global
            public int MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private int MyProperty3 { get; set; }
            protected int MyProperty4 { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Global


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
        public void NotChanged()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1" ) );
        }


        [Test]
        public void BadProperty()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass( false, false, false, false );

                                                                     RunTest( AnyValue.IsValid,
                                                                              () => mc.Method(),
                                                                              SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "BadProperty" ) );
                                                                 } );

            Assert.AreEqual( "BAD TEST: 'BadProperty' is not a property nor a field of type 'NotChangedTestsExcept+MyClass'", exception.Message );
        }


        [Test]
        public void Property1Changed()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1" ) );
        }


        [Test]
        public void Property2Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, true, false, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1" ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsExcept+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void Property3Changed()
        {
            var mc = new MyClass( false, false, true, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty3" ) );
        }


        [Test]
        public void Property4Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, false, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty3" ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsExcept+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_Except1()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1" ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty2' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty3' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_Except12()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1", "MyProperty2" ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty3' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_Except123()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1", "MyProperty2", "MyProperty3" ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_Except1234()
        {
            var mc = new MyClass( true, true, true, true );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChangedExcept( mc, NotChangedKind.AllProperties, "MyProperty1", "MyProperty2", "MyProperty3", "MyProperty4" ) );
        }


        [Test]
        public void AllPropertiesChanged_ImplicitPublic()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( mc, "MyProperty1" ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsExcept+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_ImplicitPublicWithNonPublic()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass( true, true, true, true );

                                                                     RunTest( AnyValue.IsValid,
                                                                              () => mc.Method(),
                                                                              SmartAssert.NotChangedExcept( mc, "MyProperty1", "MyProperty3" ) );
                                                                 } );

            Assert.AreEqual( "BAD TEST: 'MyProperty3' is not a property nor a field of type 'NotChangedTestsExcept+MyClass'", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_ImplicitInstance()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( NotChangedKind.AllProperties, "MyProperty1", "MyProperty3" ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty2' has changed", exception.Message );
            StringAssert.Contains( "Property 'NotChangedTestsExcept+MyClass.MyProperty4' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_ImplicitPublicImplicitInstance()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true, true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( "MyProperty1" ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsExcept+MyClass.MyProperty2' has changed", exception.Message );
        }
    }
}