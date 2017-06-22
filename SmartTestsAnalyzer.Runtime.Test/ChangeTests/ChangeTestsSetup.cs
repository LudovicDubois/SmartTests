using System.Collections.Generic;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.ChangeTests
{
    [TestFixture]
    public class ChangeTestsSetup
    {
        public class MyClass
        {
            public bool ChangeProperty;
            public bool ChangeIndexer;
            public bool ChangeIndirect;
            public bool ChangeField;


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
                if( ChangeProperty )
                    MyProperty += 1;
                if( ChangeIndexer )
                    this[ 0 ] += 2;
                if( ChangeIndirect )
                    Items.Add( 3 );
                if( ChangeField )
                    MyField += 4;
            }
        }


        private MyClass _Mc;


        [SetUp]
        public void Setup() => _Mc = new MyClass();


        [Test]
        public void PropertyChange()
        {
            _Mc.ChangeProperty = true;

            RunTest( AnyValue.IsValid,
                     () => _Mc.Method(),
                     SmartAssert.Change( () => _Mc.MyProperty + 1 ) );
        }


        [Test]
        public void PropertyChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  _Mc.ChangeProperty = true;

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => _Mc.Method(),
                                                                           SmartAssert.Change( () => _Mc.MyProperty + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 1", exception.Message );
        }


        [Test]
        public void IndirectChange()
        {
            _Mc.ChangeIndirect = true;

            RunTest( AnyValue.IsValid,
                     () => _Mc.Method(),
                     SmartAssert.Change( () => _Mc.Items.Count + 1 ) );
        }


        [Test]
        public void IndirectChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  _Mc.ChangeIndirect = true;

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => _Mc.Method(),
                                                                           SmartAssert.Change( () => _Mc.Items.Count + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 3, but was 2", exception.Message );
        }


        [Test]
        public void IndexerChange()
        {
            _Mc.ChangeIndexer = true;

            RunTest( AnyValue.IsValid,
                     () => _Mc.Method(),
                     SmartAssert.Change( () => _Mc[ 0 ] + 2 ) );
        }


        [Test]
        public void IndexerChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  _Mc.ChangeIndexer = true;

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => _Mc.Method(),
                                                                           SmartAssert.Change( () => _Mc[ 0 ] + 3 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 3, but was 2", exception.Message );
        }


        [Test]
        public void MethodChange()
        {
            _Mc.ChangeProperty = true;

            RunTest( AnyValue.IsValid,
                     () => _Mc.Method(),
                     SmartAssert.Change( () => _Mc.MyMethod() + 1 ) );
        }


        [Test]
        public void MethodChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  _Mc.ChangeProperty = true;

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => _Mc.Method(),
                                                                           SmartAssert.Change( () => _Mc.MyMethod() + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 1", exception.Message );
        }


        [Test]
        public void FieldChange()
        {
            _Mc.ChangeField = true;

            RunTest( AnyValue.IsValid,
                     () => _Mc.Method(),
                     SmartAssert.Change( () => _Mc.MyField + 4 ) );
        }


        [Test]
        public void FieldChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  _Mc.ChangeField = true;

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => _Mc.Method(),
                                                                           SmartAssert.Change( () => _Mc.MyField + 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 4", exception.Message );
        }
    }
}