using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotChangedTests
{
    [TestFixture]
    public class NotChangedTestsExceptImplicit
    {
        public class MyClass
        {
            public int MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }
            public int MyProperty3 => MyProperty2;
        }


        [Test]
        public void NotChangedExcept()
        {
            var mc = new MyClass();

            RunTest( AnyValue.IsValid,
                     Assign( () => mc.MyProperty1, 1 ),
                     SmartAssert.NotChangedExcept(),
                     SmartAssert.ChangedTo() );
        }


        [Test]
        public void NotChangedExcept_BadExcept()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass();

                                                                  RunTest( AnyValue.IsValid,
                                                                           Assign( () => mc.MyProperty2, 1 ),
                                                                           SmartAssert.NotChangedExcept(),
                                                                           SmartAssert.ChangedTo() );
                                                              } );

            StringAssert.Contains( "Property 'NotChangedTestsExceptImplicit+MyClass.MyProperty3' has changed", exception.Message );
        }
    }
}