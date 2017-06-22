using System.Collections.Generic;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.ChangeTests
{
    [TestFixture]
    public class ChangeTests
    {
        public class MyClass
        {
            public MyClass( bool changeProperty, bool changeIndexer, bool changeIndirect, bool changeField )
            {
                _ChangeProperty = changeProperty;
                _ChangeIndexer = changeIndexer;
                _ChangeIndirect = changeIndirect;
                _ChangeField = changeField;
            }


            private readonly bool _ChangeProperty;
            private readonly bool _ChangeIndexer;
            private readonly bool _ChangeIndirect;
            private readonly bool _ChangeField;


            public int MyProperty { get; set; }
            public int this[ int index ]
            {
                get { return Items[ index ]; }
                set { Items[ index ] = value; }
            }
            public List<int> Items { get; } = new List<int> { 0 };


            public int MyMethod() => MyProperty;

            public int MyField;


            public void Method()
            {
                if( _ChangeProperty )
                    MyProperty += 1;
                if( _ChangeIndexer )
                    this[ 0 ] += 2;
                if( _ChangeIndirect )
                    Items.Add( 3 );
                if( _ChangeField )
                    MyField += 4;
            }
        }


        [Test]
        public void PropertyChange()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Change( () => mc.MyProperty + 1 ) );
        }


        [Test]
        public void PropertyChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Change( () => mc.MyProperty + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 1", exception.Message );
        }


        [Test]
        public void IndirectChange()
        {
            var mc = new MyClass( false, false, true, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Change( () => mc.Items.Count + 1 ) );
        }


        [Test]
        public void IndirectChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Change( () => mc.Items.Count + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 3, but was 2", exception.Message );
        }


        [Test]
        public void IndexerChange()
        {
            var mc = new MyClass( false, true, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Change( () => mc[ 0 ] + 2 ) );
        }


        [Test]
        public void IndexerChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, true, false, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Change( () => mc[ 0 ] + 3 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 3, but was 2", exception.Message );
        }


        [Test]
        public void MethodChange()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Change( () => mc.MyMethod() + 1 ) );
        }


        [Test]
        public void MethodChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Change( () => mc.MyMethod() + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 1", exception.Message );
        }


        [Test]
        public void FieldChange()
        {
            var mc = new MyClass( false, false, false, true );

            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Change( () => mc.MyField + 4 ) );
        }


        [Test]
        public void FieldChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, false, true );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Change( () => mc.MyField + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 4", exception.Message );
        }
    }
}