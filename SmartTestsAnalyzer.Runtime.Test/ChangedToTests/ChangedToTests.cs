using System.Collections.Generic;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.ChangedToTests
{
    [TestFixture]
    public class ChangedToTests
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
        public void Property()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.ChangedTo( () => mc.MyProperty, 1 ) );
        }


        [Test]
        public void PropertyBad()
        {
            var exception = Assert.Catch<BadTestException>( () =>
                                                            {
                                                                var mc = new MyClass( true, false, false, false );

                                                                RunTest( AnyValue.Valid,
                                                                         () => mc.Method(),
                                                                         SmartAssert.ChangedTo( () => mc.MyProperty, 0 ) );
                                                            } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }


        [Test]
        public void PropertyError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.ChangedTo( () => mc.MyProperty, 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 1", exception.Message );
        }


        [Test]
        public void Indirect()
        {
            var mc = new MyClass( false, false, true, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.ChangedTo( () => mc.Items.Count, 2 ) );
        }


        [Test]
        public void IndirectBad()
        {
            var exception = Assert.Catch<BadTestException>( () =>
                                                            {
                                                                var mc = new MyClass( false, false, true, false );

                                                                RunTest( AnyValue.Valid,
                                                                         () => mc.Method(),
                                                                         SmartAssert.ChangedTo( () => mc.Items.Count, 1 ) );
                                                            } );

            Assert.AreEqual( "BAD TEST: unexpected value 1", exception.Message );
        }


        [Test]
        public void IndirectError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, true, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.ChangedTo( () => mc.Items.Count, 3 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 3, but was 2", exception.Message );
        }


        [Test]
        public void Indexer()
        {
            var mc = new MyClass( false, true, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.ChangedTo( () => mc[ 0 ], 2 ) );
        }


        [Test]
        public void IndexerBad()
        {
            var exception = Assert.Catch<BadTestException>( () =>
                                                            {
                                                                var mc = new MyClass( false, true, false, false );

                                                                RunTest( AnyValue.Valid,
                                                                         () => mc.Method(),
                                                                         SmartAssert.ChangedTo( () => mc[ 0 ], 0 ) );
                                                            } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }


        [Test]
        public void IndexerError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, true, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.ChangedTo( () => mc[ 0 ], 3 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 3, but was 2", exception.Message );
        }


        [Test]
        public void Method()
        {
            var mc = new MyClass( true, false, false, false );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.ChangedTo( () => mc.MyMethod(), 1 ) );
        }


        [Test]
        public void MethodBad()
        {
            var exception = Assert.Catch<BadTestException>( () =>
                                                            {
                                                                var mc = new MyClass( true, false, false, false );

                                                                RunTest( AnyValue.Valid,
                                                                         () => mc.Method(),
                                                                         SmartAssert.ChangedTo( () => mc.MyMethod(), 0 ) );
                                                            } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }


        [Test]
        public void MethodChangeError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true, false, false, false );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.ChangedTo( () => mc.MyMethod(), 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 1", exception.Message );
        }


        [Test]
        public void Field()
        {
            var mc = new MyClass( false, false, false, true );

            RunTest( AnyValue.Valid,
                     () => mc.Method(),
                     SmartAssert.ChangedTo( () => mc.MyField, 4 ) );
        }


        [Test]
        public void FieldBad()
        {
            var exception = Assert.Catch<BadTestException>( () =>
                                                            {
                                                                var mc = new MyClass( false, false, false, true );

                                                                RunTest( AnyValue.Valid,
                                                                         () => mc.Method(),
                                                                         SmartAssert.ChangedTo( () => mc.MyField, 0 ) );
                                                            } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }


        [Test]
        public void FieldError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false, false, false, true );

                                                                  RunTest( AnyValue.Valid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.ChangedTo( () => mc.MyField, 2 ) );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 4", exception.Message );
        }
    }
}