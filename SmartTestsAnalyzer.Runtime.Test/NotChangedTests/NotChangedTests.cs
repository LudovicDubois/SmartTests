using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTests
    {
        public class MyClass
        {
            public MyClass( bool change1, bool change2 = false )
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
            var mc = new MyClass( false );
            Assert.AreEqual( 0, mc.MyProperty1 );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( mc ) );

            Assert.AreEqual( 0, mc.MyProperty1 );
        }


        [Test]
        public void PropertyOneChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true );
                                                                  Assert.AreEqual( 0, mc.MyProperty1 );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTests+MyClass.MyProperty1' has changed", exception.Message );
        }


        [Test]
        public void PropertyTwoChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, true );
                                                                  Assert.AreEqual( 0, mc.MyProperty2 );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc ) );
                                                              } );

            Assert.AreEqual( "Property 'NotChangedTests+MyClass.MyProperty2' has changed", exception.Message );
        }


        [Test]
        public void BothPropertiesChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, true );
                                                                  Assert.AreEqual( 0, mc.MyProperty2 );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( mc ) );
                                                              } );

            Assert.AreEqual( @"Property 'NotChangedTests+MyClass.MyProperty1' has changed
Property 'NotChangedTests+MyClass.MyProperty2' has changed",
                             exception.Message );
        }
    }
}