using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTestsExceptLambda
    {
        public class MyClass
        {
            public MyClass( bool change1, bool change2 )
            {
                _Change1 = change1;
                _Change2 = change2;
            }


            private readonly bool _Change1;
            private readonly bool _Change2;


            public int MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }


            public void Method()
            {
                if( _Change1 )
                    MyProperty1 = 1;
                if( _Change2 )
                    MyProperty2 = 2;
            }
        }


        [Test]
        public void NotChanged()
        {
            var mc = new MyClass( false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChangedExcept( () => mc.MyProperty1, NotChangedKind.AllProperties ) );
        }


        [Test]
        public void Property1Changed()
        {
            var mc = new MyClass( true, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChangedExcept( () => mc.MyProperty1, NotChangedKind.AllProperties ) );
        }


        [Test]
        public void Property2Changed()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( () => mc.MyProperty1, NotChangedKind.AllProperties ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsExceptLambda+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_Except1()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( () => mc.MyProperty1, NotChangedKind.AllProperties ) );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsExceptLambda+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void AllPropertiesChanged_ImplicitPublic()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChangedExcept( () => mc.MyProperty1 ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTestsExceptLambda+MyClass.MyProperty2' has changed", exception.Message );
        }
    }
}