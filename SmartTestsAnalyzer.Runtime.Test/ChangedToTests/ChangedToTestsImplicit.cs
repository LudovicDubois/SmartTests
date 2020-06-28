using System.Collections.Generic;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;

// ReSharper disable UnusedMember.Global
// ReSharper disable ValueParameterNotUsed
// ReSharper disable UnusedAutoPropertyAccessor.Global


namespace SmartTestsAnalyzer.Runtime.Test.ChangedToTests
{
    [TestFixture]
    public class ChangedToTestsImplicit
    {
        public class MyClass
        {
            public int MyProperty { get; set; }
            public int MyPropertyError
            {
                get { return 0; }
                set { }
            }

            public int this[ int index ]
            {
                get { return _Items[ index ]; }
                set { _Items[ index ] = value; }
            }
            private readonly List<int> _Items = new List<int> { 0 };
            public int this[ int index1, int index2 ]
            {
                get { return 0; }
                set { }
            }


            public int MyField;
        }


        [Test]
        public void Property()
        {
            var mc = new MyClass();

            RunTest( AnyValue.IsValid,
                     Assign( () => mc.MyProperty, 1 ),
                     SmartAssert.ChangedTo() );
        }


        [Test]
        public void PropertyNotAssignment()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass();

                                                                     RunTest( AnyValue.IsValid,
                                                                              () => mc.MyProperty,
                                                                              SmartAssert.ChangedTo() );
                                                                 } );

            Assert.AreEqual( "BAD TEST: Act is not an assignment", exception.Message );
        }


        [Test]
        public void PropertyBad()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass();

                                                                     RunTest( AnyValue.IsValid,
                                                                              Assign( () => mc.MyProperty, 0 ),
                                                                              SmartAssert.ChangedTo() );
                                                                 } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }


        [Test]
        public void PropertyError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass();

                                                                  RunTest( AnyValue.IsValid,
                                                                           Assign( () => mc.MyPropertyError, 1 ),
                                                                           SmartAssert.ChangedTo() );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 1, but was 0", exception.Message );
        }


        [Test]
        public void Indexer()
        {
            var mc = new MyClass();

            RunTest( AnyValue.IsValid,
                     Assign( () => mc[ 0 ], 1 ),
                     SmartAssert.ChangedTo() );
        }


        [Test]
        public void IndexerNotAssignment()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass();

                                                                     RunTest( AnyValue.IsValid,
                                                                              () => mc[ 0 ],
                                                                              SmartAssert.ChangedTo() );
                                                                 } );

            Assert.AreEqual( "BAD TEST: Act is not an assignment", exception.Message );
        }


        [Test]
        public void IndexerBad()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass();

                                                                     RunTest( AnyValue.IsValid,
                                                                              Assign( () => mc[ 0 ], 0 ),
                                                                              SmartAssert.ChangedTo() );
                                                                 } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }


        [Test]
        public void IndexerError()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass();

                                                                  RunTest( AnyValue.IsValid,
                                                                           Assign( () => mc[ 0, 0 ], 2 ),
                                                                           SmartAssert.ChangedTo() );
                                                              } );

            Assert.AreEqual( "Change is wrong. Expected 2, but was 0", exception.Message );
        }


        [Test]
        public void Field()
        {
            var mc = new MyClass();

            RunTest( AnyValue.IsValid,
                     Assign( () => mc.MyField, 4 ),
                     SmartAssert.ChangedTo() );
        }


        [Test]
        public void FieldNotAssignment()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass();

                                                                     RunTest( AnyValue.IsValid,
                                                                              () => mc.MyField,
                                                                              SmartAssert.ChangedTo() );
                                                                 } );

            Assert.AreEqual( "BAD TEST: Act is not an assignment", exception.Message );
        }


        [Test]
        public void FieldBad()
        {
            var exception = Assert.Catch<InconclusiveException>( () =>
                                                                 {
                                                                     var mc = new MyClass();

                                                                     RunTest( AnyValue.IsValid,
                                                                              Assign( () => mc.MyField, 0 ),
                                                                              SmartAssert.ChangedTo() );
                                                                 } );

            Assert.AreEqual( "BAD TEST: unexpected value 0", exception.Message );
        }
    }
}