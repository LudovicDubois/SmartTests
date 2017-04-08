using NUnit.Framework;

using SmartTests.Criterias;

using TestedProject;

using static SmartTests.SmartTest;



namespace TestingProject
{
    [TestFixture]
    public class ProductTest
    {
        [Test]
        public void NormalConstructorTest()
        {
            var product = RunTest( Case( "id", MinExcluded.IsAboveMin ) &
                                   Case( "description", ValidValue.Valid ) &
                                   Case( "price", MinIncluded.IsAboveMin ),
                                   () => new Product( 1, "SmartTest", 100 ) );

            Assert.That( product.Id, Is.EqualTo( 1 ) );
            Assert.That( product.Description, Is.EqualTo( "SmartTest" ) );
            Assert.That( product.Price, Is.EqualTo( 100.0 ) );
        }
    }
}