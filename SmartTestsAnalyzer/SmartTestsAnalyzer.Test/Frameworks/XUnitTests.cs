using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.Frameworks
{
    [TestFixture]
    public class XUnitTests: CodeFixVerifier
    {
        [Test]
        public void Fact()
        {
            var test = @"
using System;
using Xunit;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    public class MyTestClass
    {
        [Fact]
        public void TestMethod()
        {
            var result = RunTest( ValidValue.IsValid, () => Math.Sqrt(4) );

            Assert.Equal( 2, result );
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
                                               new DiagnosticResultLocation( "Test0.cs", 14, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Test()
        {
            var test = @"
using System;
using Xunit;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    public class MyTestClass
    {
        [Theory]
        [InlineData(4,2)]
        [InlineData(16,4)]
        public void TestMethod( int data, int expected )
        {
            var result = RunTest( ValidValue.IsValid, () => Math.Sqrt(data) );

            Assert.Equal( expected, result );
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
                                               new DiagnosticResultLocation( "Test0.cs", 16, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override IEnumerable<Type> GetTestingFramework()
        {
            yield return typeof(Xunit.FactAttribute);
            yield return typeof(Xunit.Assert);
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}