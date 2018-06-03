using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.RangeTests
{
    [TestFixture]
    class IntRangeTests: CodeFixVerifier
    {
        [Test]
        public void RangeTest()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Range( 10, int.MaxValue, out var value ), 
                                  () => Math.Sqrt( value ) );

            Assert.That( result * result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: Range(int.MinValue, 0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}
