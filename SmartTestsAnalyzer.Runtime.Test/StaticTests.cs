using System;
using System.Reflection;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test
{
    [TestFixture]
    public class StaticTests
    {
        public static class MyClass
        {
            public static int MyProperty { get; set; }


            public static void MyMethod()
            { }


            public static int MyMethod( int i ) => i;


            public static void MyMethod( ref int i )
            {
                i++;
            }


            public static void MyMethodOut( out int i )
            {
                i = 2;
            }


            public static int MyField;
        }


        private static readonly FieldInfo _MyField = typeof(MyClass).GetField( nameof(MyClass.MyField) );
        // ReSharper disable once UnusedMember.Local
        private static readonly PropertyInfo _MyIndexer = typeof(MyClass).GetProperty( "Item", new[] { typeof(int) } );
        private static readonly MethodInfo _MyMethod0 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), Type.EmptyTypes );
        private static readonly MethodInfo _MyMethod1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new[] { typeof(int) } );
        private static readonly MethodInfo _MyMethodRef1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new[] { typeof(int).MakeByRefType() } );
        private static readonly MethodInfo _MyMethodOut1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethodOut), new[] { typeof(int).MakeByRefType() } );
        private static readonly PropertyInfo _MyProperty = typeof(MyClass).GetProperty( nameof(MyClass.MyProperty) );


        [Test]
        public void PropertyGet()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.MyProperty,
                     new ActValidator( null, _MyProperty, _MyProperty.GetMethod ) );
        }


        [Test]
        public void PropertySet()
        {
            RunTest( AnyValue.IsValid,
                     Assign( () => MyClass.MyProperty, 1 ),
                     new ActValidator( null, _MyProperty, _MyProperty.SetMethod ) );
        }


        [Test]
        public void MethodWithoutParameter()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.MyMethod(),
                     new ActValidator( null, _MyMethod0 ) );
        }


        [Test]
        public void MethodWithParameter()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.MyMethod( 1 ),
                     new ActValidator( null, _MyMethod1 ) );
        }


        [Test]
        public void MethodWithRefParameter()
        {
            var i = 0;
            RunTest( AnyValue.IsValid,
                     () => MyClass.MyMethod( ref i ),
                     new ActValidator( null, _MyMethodRef1 ) );
            Assert.AreEqual( 1, i );
        }


        [Test]
        public void MethodWithOutParameter()
        {
            var i = 0;
            RunTest( AnyValue.IsValid,
                     () => MyClass.MyMethodOut( out i ),
                     new ActValidator( null, _MyMethodOut1 ) );
            Assert.AreEqual( 2, i );
        }


        [Test]
        public void FieldGet()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.MyField,
                     new ActValidator( null, _MyField ) );
        }


        [Test]
        public void FieldSet()
        {
            RunTest( AnyValue.IsValid,
                     Assign( () => MyClass.MyField, 1 ),
                     new ActValidator( null, _MyField ) );
        }
    }
}