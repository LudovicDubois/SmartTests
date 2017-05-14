using Microsoft.CodeAnalysis.CodeFixes;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class NotASmartTestTests: CodeFixVerifier
    {
        [Test]
        public void NotATestClass()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    //[TestFixture]
    public class MyTestClass
    {
        [Test]
        public void MyTest()
        {
            var result = RunTest( Case( ValidValue.Valid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";

            VerifyCSharpDiagnostic( test, 0 );
        }


        [Test]
        public void NotATestMethod()
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
        //[Test]
        public void MyTest()
        {
            var result = RunTest( Case( ValidValue.Valid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";

            VerifyCSharpDiagnostic( test, 0 );
        }


        [Test]
        public void NotASmartTest()
        {
            var test = @"
using System;
using NUnit.Framework;

namespace TestingProject
{
    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void MyTest()
        {
            // Arrange
            // Act
            var result = Math.Sqrt(4);

            // Assert
            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";

            VerifyCSharpDiagnostic( test, 0 );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}