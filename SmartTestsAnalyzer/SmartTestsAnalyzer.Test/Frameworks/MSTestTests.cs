using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.Frameworks
{
    [TestFixture]
    public class MSTestTests: CodeFixVerifier
    {
        [Test]
        public void Test()
        {
            var test = @"
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void TestMethod()
        {
            var result = RunTest( ValidValue.IsValid, () => Math.Sqrt(4) );

            Assert.AreEqual( 2, result );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override IEnumerable<Type> GetTestingFramework()
        {
            yield return typeof(TestClassAttribute);
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}