using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MultipleCasesTests
{
    [TestFixture]
    public class Missing1Case
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


        [Test]
        public void MyTest4()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsMin ) &
                                  Case( "j", MinIncluded.IsMin ),
                                  () => mc.Add( 0, 0 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }


        [Test]
        public void MyTest5()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsBelowMin ),
                                  () => mc.Add( -1, 0 ) );

            Assert.That( result, Is.EqualTo( -1 ) );
        }
    }


    /*
        var expected = new DiagnosticResult
                {
                    Id = "SmartTestsAnalyzer_MissingCases",
                    Message = "Tests for 'TestingProject.MyTestClass.MyClass.Range(int, int)' has some missing Test Cases: j:MinIncluded.IsBelowMin",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[]
                                {
                                    new DiagnosticResultLocation( "Test0.cs", 29, 35 ),
                                    new DiagnosticResultLocation( "Test0.cs", 41, 35 ),
                                    new DiagnosticResultLocation( "Test0.cs", 53, 35 ),
                                    new DiagnosticResultLocation( "Test0.cs", 65, 35 ),
                                    new DiagnosticResultLocation( "Test0.cs", 77, 35 )
                                }
                };
    */
}