﻿using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.ConstructorTests
{
    [TestFixture]
    public class ValidTest
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }


            public int Property { get; }
        }


        [Test]
        public void MyTest()
        {
            var result = RunTest( AnyValue.IsValid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }
    }


    // No error
}