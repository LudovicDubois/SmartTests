using NUnit.Framework;

using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTests.NUnit.Test
{
    public static class MyClass
    {
        public static int Property { get; set; }
    }


    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void InconclusiveTest()
        {
            InconclusiveExceptionType = null; // To be sure to use the default one

            Assert.Throws<InconclusiveException>(
                                                 () => RunTest( AnyValue.IsValid,
                                                                Assign( () => MyClass.Property, 0 ),
                                                                SmartAssert.ChangedTo() )
                                                );
        }


        [Test]
        public void BadTestTest()
        {
            InconclusiveExceptionType = typeof(BadTestException);

            Assert.Throws<BadTestException>(
                                            () => RunTest( AnyValue.IsValid,
                                                           Assign( () => MyClass.Property, 0 ),
                                                           SmartAssert.ChangedTo() )
                                           );
        }
    }
}