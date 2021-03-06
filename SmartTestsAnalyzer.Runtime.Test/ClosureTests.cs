﻿using System;
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


        private static readonly FieldInfo _MyField = typeof(MyClass).GetField( nameof(MyClass.MyField) );
        private static readonly PropertyInfo _MyIndexer = typeof(MyClass).GetProperty( "Item", new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethod0 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), Type.EmptyTypes );
        private static readonly MethodInfo _MyMethod1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethodRef1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new[] { typeof(int).MakeByRefType() } );
        private static readonly MethodInfo _MyMethodOut1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethodOut), new[] { typeof(int).MakeByRefType() } );
        private static readonly PropertyInfo _MyProperty = typeof(MyClass).GetProperty( nameof(MyClass.MyProperty) );


        [Test]
        public void Constructor()
        {
            RunTest( AnyValue.IsValid,
                     () => new MyClass(),
                     new ActValidator( typeof(MyClass).GetConstructor( Type.EmptyTypes ) ) );
        }


        [Test]
        public void PropertyGet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     () => mc.MyProperty,
                     new ActValidator( mc, _MyProperty, _MyProperty.GetMethod ) );
        }


        [Test]
        public void PropertySet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     Assign( () => mc.MyProperty, 1 ),
                     new ActValidator( mc, _MyProperty, _MyProperty.SetMethod ) );
        }


        [Test]
        public void IndexerGet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     () => mc[ 0 ],
                     new ActValidator( mc, _MyIndexer, _MyIndexer.GetMethod ) );
        }


        [Test]
        public void IndexerSet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     Assign( () => mc[ 0 ], 1 ),
                     new ActValidator( mc, _MyIndexer, _MyIndexer.SetMethod ) );
        }


        [Test]
        public void MethodWithoutParameter()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     () => mc.MyMethod(),
                     new ActValidator( mc, _MyMethod0 ) );
        }


        [Test]
        public void MethodWithParameter()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     () => mc.MyMethod( 1 ),
                     new ActValidator( mc, _MyMethod1 ) );
        }


        [Test]
        public void MethodWithRefParameter()
        {
            var mc = new MyClass();
            var i = 0;
            RunTest( AnyValue.IsValid,
                     () => mc.MyMethod( ref i ),
                     new ActValidator( mc, _MyMethodRef1 ) );
            Assert.AreEqual( 1, i );
        }


        [Test]
        public void MethodWithOutParameter()
        {
            var mc = new MyClass();
            var i = 0;
            RunTest( AnyValue.IsValid,
                     () => mc.MyMethodOut( out i ),
                     new ActValidator( mc, _MyMethodOut1 ) );
            Assert.AreEqual( 2, i );
        }


        [Test]
        public void FieldGet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     () => mc.MyField,
                     new ActValidator( mc, _MyField ) );
        }


        [Test]
        public void FieldSet()
        {
            var mc = new MyClass();
            RunTest( AnyValue.IsValid,
                     Assign( () => mc.MyField, 1 ),
                     new ActValidator( mc, _MyField ) );
        }
    }
}