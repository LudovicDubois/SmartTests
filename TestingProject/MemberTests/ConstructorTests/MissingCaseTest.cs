using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.ConstructorTests
{
    [TestFixture]
    public class MissingCaseTest
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
            var result = RunTest( ValidValue.IsValid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }


        /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_MissingCases",
                            Message = "Tests for 'TestingProject.ConstructorTests.MyClass.MyClass(int)' has some missing Test Cases: ValidValue.IsInvalid",
                            Severity = DiagnosticSeverity.Warning,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 25, 35 )
                                        }
                        };
         */
    }
}