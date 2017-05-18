using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MultipleCriteriasTests
{
    [TestFixture]
    public class OrMissingNoCaseTest
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
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue | NotifyPropertyChanged.HasSubscriberOtherValue,
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }


        // No error!
    }
}