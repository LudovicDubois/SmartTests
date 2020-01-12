using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTestsFieldsImplicit
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


            public int MyField1;
            public int MyField2;
            // ReSharper disable once InconsistentNaming
#pragma warning disable 414
            private int MyField3;
#pragma warning restore 414
            protected int MyField4;


            public void Method()
            {
                if( _Change1 )
                    MyField1 = 1;
                if( _Change2 )
                    MyField2 = 2;
                if( _Change3 )
                    MyField3 = 3;
                if( _Change4 )
                    MyField4 = 4;
            }
        }


        [Test]
        public void NotChangedPublic()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChanged() );
        }


        [Test]
        public void NotChangedNonPublic()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.NonPublicFields ) );
        }


        [Test]
        public void NotChangedAll()
        {
            var mc = new MyClass( false, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.AllFields ) );
        }


        [Test]
        public void Field1ChangedPublic()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.PublicFields ) );
                                                              } );

            Assert.AreEqual( "Field 'NotChangedTestsFieldsImplicit+MyClass.MyField1' has changed", exception.Message );
        }


        [Test]
        public void Field1ChangedNonPublic()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.NonPublicFields ) );
        }


        [Test]
        public void Field1ChangedAll()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.AllFields ) );
                                                              } );

            Assert.AreEqual( "Field 'NotChangedTestsFieldsImplicit+MyClass.MyField1' has changed", exception.Message );
        }


        [Test]
        public void Field3ChangedPublic()
        {
            var mc = new MyClass( false, false, true, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.NotChanged( NotChangedKind.PublicFields ) );
        }


        [Test]
        public void Field3ChangedNonPublic()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.NonPublicFields ) );
                                                              } );

            Assert.AreEqual( "Field 'NotChangedTestsFieldsImplicit+MyClass.MyField3' has changed", exception.Message );
        }


        [Test]
        public void Field3ChangedAll()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.NotChanged( NotChangedKind.AllFields ) );
                                                              } );

            Assert.AreEqual( "Field 'NotChangedTestsFieldsImplicit+MyClass.MyField3' has changed", exception.Message );
        }
    }
}