using Microsoft.VisualStudio.TestTools.UnitTesting;

using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTests.MSTests.Test
{
    public static class MyClass
    {
        public static int Property { get; set; }
    }


    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        [ExpectedException( typeof(AssertInconclusiveException) )]
        public void InconclusiveTest()
        {
            InconclusiveExceptionType = null; // To be sure to use the default one

            RunTest( AnyValue.IsValid,
                     Assign( () => MyClass.Property, 0 ),
                     SmartAssert.ChangedTo() );
        }


        [TestMethod]
        [ExpectedException( typeof(BadTestException) )]
        public void BadTestTest()
        {
            InconclusiveExceptionType = typeof(BadTestException);

            RunTest( AnyValue.IsValid,
                     Assign( () => MyClass.Property, 0 ),
                     SmartAssert.ChangedTo() );
        }
    }
}