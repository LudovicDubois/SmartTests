using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.RangeTests
{
    [TestFixture]
    class IntRangeTests: CodeFixVerifier
    {
        [Test]
        public void OneChunkInOneRange()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Range( 1, int.MaxValue, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(int)' has some missing Test Cases: Range(int.MinValue, 0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void TwoChunksInTwoRanges()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Range( 1, int.MaxValue, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Range( int.MinValue, -1, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(int)' has some missing Test Cases: Range(0, 0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void TwoChunksInOneRange()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Range( int.MinValue, -1 ).Add( 1, int.MaxValue, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(int)' has some missing Test Cases: Range(0, 0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void ThreeChunksInOneRange()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Range( int.MinValue, -1 ).Add( 1, 10 ).Add( 11, int.MaxValue, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(int)' has some missing Test Cases: Range(0, 0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void OneChunkGetValueInOneRange()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Range( 1, int.MaxValue ).GetValue( out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(int)' has some missing Test Cases: Range(int.MinValue, 0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void OneChunk_RangeMinNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Range( one, int.MaxValue, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_NotAConstant",
                               Message = "A constant is expected",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 42 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeMaxNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Range( int.MinValue, one, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_NotAConstant",
                               Message = "A constant is expected",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 56 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeAddMinNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Range( int.MinValue, -1 ).Add( one, int.MaxValue, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_NotAConstant",
                               Message = "A constant is expected",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 66 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeAddMaxNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(int i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Range( int.MinValue, -1 ).Add( 1, one, out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_NotAConstant",
                               Message = "A constant is expected",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 69 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}
