﻿using System;
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


            public int MyMethod( int i ) => i;

            public int MyField;
        }


        private MyClass _Mc;
        private static readonly FieldInfo _MyField = typeof(MyClass).GetField( nameof(MyClass.MyField) );
        private static readonly PropertyInfo _MyIndexer = typeof(MyClass).GetProperty( "Item", new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethod0 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), Type.EmptyTypes );
        private static readonly MethodInfo _MyMethod1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new Type[] { typeof(int) } );
        private static readonly PropertyInfo _MyProperty = typeof(MyClass).GetProperty( nameof(MyClass.MyProperty) );


        [SetUp]
        public void Setup() => _Mc = new MyClass();


        [Test]
        public void PropertyGet()
        {
            RunTest( AnyValue.Valid,
                     () => _Mc.MyProperty,
                     new ActValidator( _Mc, _MyProperty, _MyProperty.GetMethod ) );
        }


        [Test]
        public void PropertySet()
        {
            RunTest( AnyValue.Valid,
                     Assign( () => _Mc.MyProperty, 1 ),
                     new ActValidator( _Mc, _MyProperty, _MyProperty.SetMethod ) );
        }


        [Test]
        public void IndexerGet()
        {
            RunTest( AnyValue.Valid,
                     () => _Mc[ 0 ],
                     new ActValidator( _Mc, _MyIndexer, _MyIndexer.GetMethod ) );
        }


        [Test]
        public void IndexerSet()
        {
            RunTest( AnyValue.Valid,
                     Assign( () => _Mc[ 0 ], 1 ),
                     new ActValidator( _Mc, _MyIndexer, _MyIndexer.SetMethod ) );
        }


        [Test]
        public void MethodWithoutParameter()
        {
            RunTest( AnyValue.Valid,
                     () => _Mc.MyMethod(),
                     new ActValidator( _Mc, _MyMethod0 ) );
        }


        [Test]
        public void MethodWithParameter()
        {
            RunTest( AnyValue.Valid,
                     () => _Mc.MyMethod( 1 ),
                     new ActValidator( _Mc, _MyMethod1 ) );
        }


        [Test]
        public void FieldGet()
        {
            RunTest( AnyValue.Valid,
                     () => _Mc.MyField,
                     new ActValidator( _Mc, _MyField ) );
        }


        [Test]
        public void FieldSet()
        {
            RunTest( AnyValue.Valid,
                     Assign( () => _Mc.MyField, 1 ),
                     new ActValidator( _Mc, _MyField ) );
        }
    }
}