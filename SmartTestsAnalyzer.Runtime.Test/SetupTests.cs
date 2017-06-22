using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test
{
    [TestFixture]
    public class SetupTests
    {
        public class MyClass
        {
            public int MyProperty { get; set; }
            private readonly List<int> _Items = new List<int> { 0 };
            public int this[ int index ]
            {
                get { return _Items[ index ]; }
                set { _Items[ index ] = value; }
            }


            public void MyMethod()
            { }


            public void MyMethod( ref int i )
            {
                i++;
            }


            public void MyMethodOut( out int i )
            {
                i = 2;
            }


            public int MyMethod( int i ) => i;

            public int MyField;
        }


        private MyClass _Mc;
        private static readonly FieldInfo _MyField = typeof(MyClass).GetField( nameof(MyClass.MyField) );
        private static readonly PropertyInfo _MyIndexer = typeof(MyClass).GetProperty( "Item", new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethod0 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), Type.EmptyTypes );
        private static readonly MethodInfo _MyMethod1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethodRef1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new[] { typeof(int).MakeByRefType() } );
        private static readonly MethodInfo _MyMethodOut1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethodOut), new[] { typeof(int).MakeByRefType() } );
        private static readonly PropertyInfo _MyProperty = typeof(MyClass).GetProperty( nameof(MyClass.MyProperty) );


        [SetUp]
        public void Setup() => _Mc = new MyClass();


        [Test]
        public void PropertyGet()
        {
            RunTest( AnyValue.IsValid,
                     () => _Mc.MyProperty,
                     new ActValidator( _Mc, _MyProperty, _MyProperty.GetMethod ) );
        }


        [Test]
        public void PropertySet()
        {
            RunTest( AnyValue.IsValid,
                     Assign( () => _Mc.MyProperty, 1 ),
                     new ActValidator( _Mc, _MyProperty, _MyProperty.SetMethod ) );
        }


        [Test]
        public void IndexerGet()
        {
            RunTest( AnyValue.IsValid,
                     () => _Mc[ 0 ],
                     new ActValidator( _Mc, _MyIndexer, _MyIndexer.GetMethod ) );
        }


        [Test]
        public void IndexerSet()
        {
            RunTest( AnyValue.IsValid,
                     Assign( () => _Mc[ 0 ], 1 ),
                     new ActValidator( _Mc, _MyIndexer, _MyIndexer.SetMethod ) );
        }


        [Test]
        public void MethodWithoutParameter()
        {
            RunTest( AnyValue.IsValid,
                     () => _Mc.MyMethod(),
                     new ActValidator( _Mc, _MyMethod0 ) );
        }


        [Test]
        public void MethodWithParameter()
        {
            RunTest( AnyValue.IsValid,
                     () => _Mc.MyMethod( 1 ),
                     new ActValidator( _Mc, _MyMethod1 ) );
        }


        [Test]
        public void MethodWithRefParameter()
        {
            var i = 0;
            RunTest( AnyValue.IsValid,
                     () => _Mc.MyMethod( ref i ),
                     new ActValidator( _Mc, _MyMethodRef1 ) );
            Assert.AreEqual( 1, i );
        }


        [Test]
        public void MethodWithOutParameter()
        {
            var i = 0;
            RunTest( AnyValue.IsValid,
                     () => _Mc.MyMethodOut( out i ),
                     new ActValidator( _Mc, _MyMethodOut1 ) );
            Assert.AreEqual( 2, i );
        }


        [Test]
        public void FieldGet()
        {
            RunTest( AnyValue.IsValid,
                     () => _Mc.MyField,
                     new ActValidator( _Mc, _MyField ) );
        }


        [Test]
        public void FieldSet()
        {
            RunTest( AnyValue.IsValid,
                     Assign( () => _Mc.MyField, 1 ),
                     new ActValidator( _Mc, _MyField ) );
        }
    }
}