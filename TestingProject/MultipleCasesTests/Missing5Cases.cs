using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MultipleCasesTests
{
    [TestFixture]
    public class Missing5Cases
    {
        public class MyClass
        {
            public int Add( int i, int j )
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsAboveMin ) &
                                  Case( "j", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }
    }


    /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_MissingCases",
                            Message = "Tests for 'TestingProject.MyTestClass.MyClass.Range(int, int)' has some missing Test Cases: i:MinIncluded.IsBelowMin and j:MinIncluded.IsBelowMin and i:MinIncluded.IsMin & j:MinIncluded.IsMin and i:MinIncluded.IsMin & j:MinIncluded.IsAboveMin and i:MinIncluded.IsAboveMin & j:MinIncluded.IsMin",
                            Severity = DiagnosticSeverity.Warning,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 29, 35 )
                                        }
                        };
    */
}