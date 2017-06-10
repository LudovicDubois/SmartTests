using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test
{
    [TestFixture]
    public class ClosureTests
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


            public int MyMethod( int i ) => i;

            public int MyField;
        }


        private static readonly FieldInfo _MyField = typeof(MyClass).GetField( nameof(MyClass.MyField) );
        private static readonly PropertyInfo _MyIndexer = typeof(MyClass).GetProperty( "Item", new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethod0 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), Type.EmptyTypes );
        private static readonly MethodInfo _MyMethod1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new Type[] { typeof(int) } );
        private static readonly PropertyInfo _MyProperty = typeof(MyClass).GetProperty( nameof(MyClass.MyProperty) );


        [Test]
        public void Constructor()
        {
            RunTest( AnyValue.Valid,
                     () => new MyClass(),
                     new ActValidator( typeof(MyClass).GetConstructor( Type.EmptyTypes ) ) );
        }


        [Test]
        public void PropertyGet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     () => mc.MyProperty,
                     new ActValidator( mc, _MyProperty, _MyProperty.GetMethod ) );
        }


        [Test]
        public void PropertySet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     Assign( () => mc.MyProperty, 1 ),
                     new ActValidator( mc, _MyProperty, _MyProperty.SetMethod ) );
        }


        [Test]
        public void IndexerGet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     () => mc[ 0 ],
                     new ActValidator( mc, _MyIndexer, _MyIndexer.GetMethod ) );
        }


        [Test]
        public void IndexerSet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     Assign( () => mc[ 0 ], 1 ),
                     new ActValidator( mc, _MyIndexer, _MyIndexer.SetMethod ) );
        }


        [Test]
        public void MethodWithoutParameter()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     () => mc.MyMethod(),
                     new ActValidator( mc, _MyMethod0 ) );
        }


        [Test]
        public void MethodWithParameter()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     () => mc.MyMethod( 1 ),
                     new ActValidator( mc, _MyMethod1 ) );
        }


        [Test]
        public void FieldGet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     () => mc.MyField,
                     new ActValidator( mc, _MyField ) );
        }


        [Test]
        public void FieldSet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.Valid,
                     Assign( () => mc.MyField, 1 ),
                     new ActValidator( mc, _MyField ) );
        }
    }
}