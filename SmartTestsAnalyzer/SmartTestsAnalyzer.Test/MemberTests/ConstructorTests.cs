using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.MemberTests
{
    [TestFixture]
    public class ConstructorTests: CodeFixVerifier
    {
        [Test]
        public void Constructor()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }

            public int Property { get; }
        }

        [Test]
        public void Valid()
        {
            var result = RunTest( AnyValue.Valid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void ConstructorMissingCase()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }

            public int Property { get; }
        }

        [Test]
        public void Missing1()
        {
            var result = RunTest( ValidValue.Valid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'ConstructorTests.MyClass..ctor' has some missing Test Cases: ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 25, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}