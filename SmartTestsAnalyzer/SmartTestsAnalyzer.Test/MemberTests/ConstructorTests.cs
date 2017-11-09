using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.MemberTests
{
    [TestFixture]
    public class ConstructorTests: CodeFixVerifier
    {
        [Test]
        public void Valid()
        {
            var test = @"
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
        public void MyTest()
        {
            var result = RunTest( AnyValue.IsValid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void MissingCase()
        {
            var test = @"
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
        public void MyTest()
        {
            var result = RunTest( ValidValue.IsValid,
                                  () => new MyClass( 10 ) );

            Assert.That( result.Property, Is.EqualTo( 10 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.ConstructorTests.MyClass.MyClass(int)' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 24, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}