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

            public static int MyField;
        }


        private static readonly FieldInfo _MyField = typeof(MyClass).GetField( nameof(MyClass.MyField) );
        private static readonly PropertyInfo _MyIndexer = typeof(MyClass).GetProperty( "Item", new Type[] { typeof(int) } );
        private static readonly MethodInfo _MyMethod0 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), Type.EmptyTypes );
        private static readonly MethodInfo _MyMethod1 = typeof(MyClass).GetMethod( nameof(MyClass.MyMethod), new Type[] { typeof(int) } );
        private static readonly PropertyInfo _MyProperty = typeof(MyClass).GetProperty( nameof(MyClass.MyProperty) );


        [Test]
        public void PropertyGet()
        {
            RunTest( AnyValue.Valid,
                     () => MyClass.MyProperty,
                     new ActValidator( null, _MyProperty, _MyProperty.GetMethod ) );
        }


        [Test]
        public void PropertySet()
        {
            RunTest( AnyValue.Valid,
                     Assign( () => MyClass.MyProperty, 1 ),
                     new ActValidator( null, _MyProperty, _MyProperty.SetMethod ) );
        }


        [Test]
        public void MethodWithoutParameter()
        {
            RunTest( AnyValue.Valid,
                     () => MyClass.MyMethod(),
                     new ActValidator( null, _MyMethod0 ) );
        }


        [Test]
        public void MethodWithParameter()
        {
            RunTest( AnyValue.Valid,
                     () => MyClass.MyMethod( 1 ),
                     new ActValidator( null, _MyMethod1 ) );
        }


        [Test]
        public void FieldGet()
        {
            RunTest( AnyValue.Valid,
                     () => MyClass.MyField,
                     new ActValidator( null, _MyField ) );
        }


        [Test]
        public void FieldSet()
        {
            RunTest( AnyValue.Valid,
                     Assign( () => MyClass.MyField, 1 ),
                     new ActValidator( null, _MyField ) );
        }
    }
}