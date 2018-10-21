using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class EnumTypeTests: CodeFixVerifier
    {
        [Test]
        public void OneValue()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTest = SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( SmartTest.Enum.Value( DayOfWeek.Monday ), 
                                  () => Class1.Same( DayOfWeek.Monday ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: Enum.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}