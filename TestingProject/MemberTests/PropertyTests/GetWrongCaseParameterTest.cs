using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.PropertyTests
{
    [TestFixture]
    public class GetWrongCaseParameterTest
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

            var result = RunTest( Case( "value", ValidValue.Valid ),
                                  () => mc.Property );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_WrongParameterName",
                            Message = "Test for 'TestingProject.ConstructorTests.MyClass.Property [get]' has some invalid parameter 'value'.",
                            Severity = DiagnosticSeverity.Error,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 27, 41 )
                                        }
                        };
         */
    }
}