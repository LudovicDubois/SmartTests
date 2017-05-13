using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.PropertyTests
{
    [TestFixture]
    public class GetMissingCaseTest
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

            var result = RunTest( ValidValue.Valid,
                                  () => mc.Property );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_MissingCases",
                            Message = "Tests for 'TestingProject.ConstructorTests.MyClass.Property [get]' has some missing Test Cases: ValidValue.Invalid",
                            Severity = DiagnosticSeverity.Warning,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                        }
                        };
         */
    }
}