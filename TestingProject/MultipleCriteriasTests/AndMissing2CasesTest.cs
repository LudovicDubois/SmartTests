using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MultipleCriteriasTests
{
    [TestFixture]
    public class AndMissing2CasesTest
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

            var result = RunTest( ValidValue.Valid & NotifyPropertyChanged.HasNoSubscriber,
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }


        [Test]
        public void MyTest2()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.Valid & NotifyPropertyChanged.HasSubscriberSameValue,
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }


        /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_MissingCases",
                            Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.Valid & NotifyPropertyChanged.HasSubscriberOtherValue and ValidValue.Invalid",
                            Severity = DiagnosticSeverity.Warning,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 27, 35 ),
                                            new DiagnosticResultLocation( "Test0.cs", 40, 35 )
                                        }
                        };
         */
    }
}