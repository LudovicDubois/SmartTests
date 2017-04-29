using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests
{
    [TestFixture]
    public class ConstructorTests
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
        public void Valid()
        {
            var result = RunTest( AnyValue.Valid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }


        public class MyClass2
        {
            public MyClass2( int property )
            {
                Property = property;
            }


            public int Property { get; }
        }


        [Test]
        public void Missing1()
        {
            var result = RunTest( ValidValue.Valid,
                                  () => new MyClass2( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }
    }
}