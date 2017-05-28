using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MultipleCasesTests
{
    [TestFixture]
    public class Missing3Cases
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


        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsMin ) &
                                  Case( "j", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 20 ) );
        }


        [Test]
        public void MyTest3()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsAboveMin ) &
                                  Case( "j", MinIncluded.IsMin ),
                                  () => mc.Add( 10, 0 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
        }
    }


    /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_MissingCases",
                            Message = "Tests for 'TestingProject.MyTestClass.MyClass.Add(int, int)' has some missing Test Cases: i:MinIncluded.IsBelowMin and j:MinIncluded.IsBelowMin and i:MinIncluded.IsMin & j:MinIncluded.IsMin",
                            Severity = DiagnosticSeverity.Warning,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 29, 35 ),
                                            new DiagnosticResultLocation( "Test0.cs", 41, 35 ),
                                            new DiagnosticResultLocation( "Test0.cs", 53, 35 )
                                        }
                        };
    */
}