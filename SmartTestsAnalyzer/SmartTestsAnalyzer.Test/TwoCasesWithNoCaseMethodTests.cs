﻿using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class TwoCasesWithNoCaseMethodTests: CodeFixVerifier
    {
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
            var result = RunTest( ValidValue.IsValid, () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }


        [Test]
        public void TestMethod2()
        {
            Assert.Throws<IndexOutOfRangeException>( () => RunTest( ValidValue.IsInvalid, () => Math.Sqrt(-4) ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void MissingCase()
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
            var result = RunTest( ValidValue.IsValid, () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
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


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}