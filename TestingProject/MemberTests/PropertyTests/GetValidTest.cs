using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.PropertyTests
{
    [TestFixture]
    public class GetValidTest
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }


            public int Property { get; set; }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.IsValid,
                                  () => mc.Property );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        // No error
    }
}