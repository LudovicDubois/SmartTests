using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class TwoCasesTests: CodeFixVerifier
    {
        [Test]
        public void MissingFirstCase()
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
            var result = RunTest( Case( ValidValue.Valid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'Math.Sqrt' has some missing Test Cases: ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void MissingNoCase()
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
            var result = RunTest( Case( ValidValue.Valid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }


        [Test]
        public void TestMethod2()
        {
            Assert.Throws<IndexOutOfRangeException>( () => RunTest( Case( ValidValue.Invalid ), () => Math.Sqrt(-4) ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}