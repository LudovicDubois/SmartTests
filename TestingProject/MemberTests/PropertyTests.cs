using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests
{
    [TestFixture]
    public class PropertyTests
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
        public void GetValid()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  () => mc.Property );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        [Test]
        public void SetValid()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }


        public class MyClass2
        {
            public MyClass2( int property )
            {
                Property = property;
            }


            public int Property { get; set; }
        }


        [Test]
        public void GetMissingCase()
        {
            var mc = new MyClass2( 10 );

            var result = RunTest( ValidValue.Valid,
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }


        [Test]
        public void GetCaseParameter()
        {
            var mc = new MyClass2( 10 );

            var result = RunTest( Case( "value", ValidValue.Valid ),
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}