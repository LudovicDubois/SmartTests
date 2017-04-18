using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class UnitTest: CodeFixVerifier
    {
        //No diagnostics expected to show up
        [Test]
        public void SimpleTest()
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
                               Locations =
                                   new[]
                                   {
                                       new DiagnosticResultLocation( "Test0.cs", 15, 41 )
                                   }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        //Diagnostic and CodeFix both triggered and checked for
        [Test]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer",
                               Message = String.Format( "Type name '{0}' contains lowercase letters", "TypeName" ),
                               Severity = DiagnosticSeverity.Warning,
                               Locations =
                                   new[]
                                   {
                                       new DiagnosticResultLocation( "Test0.cs", 11, 15 )
                                   }
                           };

            VerifyCSharpDiagnostic( test, expected );

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
            VerifyCSharpFix( test, fixtest );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SmartTestsAnalyzerCodeFixProvider();
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SmartTestsAnalyzerAnalyzer();
        }
    }
}