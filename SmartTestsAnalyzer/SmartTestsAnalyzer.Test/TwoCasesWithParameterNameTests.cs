using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class TwoCasesWithParameterNameTests: CodeFixVerifier
    {
        [Test]
        public void RightParameterName()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ""d"", ValidValue.Valid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingParameterCases",
                Message = "Tests for 'Math.Sqrt' has some missing Test Cases for parameter 'd': ValidValue.Invalid",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 46 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void WrongParameterName()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ""value"", ValidValue.Valid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_WrongParameterName",
                Message = "Test for 'Math.Sqrt' has some invalid parameter name 'value'.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}