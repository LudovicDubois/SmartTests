using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class ULongTypeHelperTests: CodeFixVerifier
    {
        [Test]
        public void FullRange()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static ulong Same(ulong i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( ulong.MinValue, ulong.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void FullRange2()
        {
            const string test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static ulong Same(ulong i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( ulong.MinValue, 10 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 11, ulong.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void ErrorCase()
        {
            const string test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static ulong Div(ulong i, ulong b)
        {
            if( b == 0 )
                return 0;
            return (ulong)(i / b);
        }
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 0, ulong.MaxValue ), out var value1 ) &
                                  ErrorCase( (ulong b) => b.Value( 10 ), out var value2 ), 
                                  () => Class1.Div( value1, value2 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Div(ulong, ulong)' has some missing Test Cases: b:ULong.Below(10).Above(10)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 25, 46 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void LongPath()
        {
            const string test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static ulong Same(Class2 i) => i.Data;
    }

    class Class2
    {
        public ulong Data { get; }
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (Class2 i) => i.Data.Range( ulong.MinValue, ulong.MaxValue ), out var value ), 
                                  () => Class1.Same( new Class2() ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void AlmostFullRangeMin()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static ulong Same(ulong i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( ulong.MinValue + 1, ulong.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(ulong)' has some missing Test Cases: i:ULong.Value(0)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AlmostFullRangeMax()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static ulong Same(ulong i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( ulong.MinValue, ulong.MaxValue - 1 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(ulong)' has some missing Test Cases: i:ULong.Value(ulong.MaxValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRange()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 100, ulong.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Below(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void TwoChunksInTwoRanges()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 100, ulong.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( (ulong i) => i.Range( ulong.MinValue, 50 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Range(50, false, 100, false)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 ),
                                               new DiagnosticResultLocation( "Test0.cs", 28, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void TwoChunksInOneRange()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 0, 50 ).Range( 100, ulong.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Range(50, false, 100, false)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void ThreeChunksInOneRange()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 0, 50 ).Range( 51, 98 ).Range( 100, ulong.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Value(99)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            ulong one = 1;
            var result = RunTest( Case( (ulong i) => i.Range( one, ulong.MaxValue ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 63 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMaxNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            ulong one = 1;
            var result = RunTest( Case( (ulong i) => i.Range( 0, one ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 66 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeAddMinNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            ulong one = 1;
            var result = RunTest( Case( (ulong i) => i.Range( 0, 100 ).Range( one, ulong.MaxValue ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 79 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeAddMaxNotAConstant()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            ulong one = 1;
            var result = RunTest( Case( (ulong i) => i.Range( 0, 100 ).Range( 1, one ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 82 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMax()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 50, 100 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Below(50).Above(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMiddleMax()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Range( 10, 50 ).Range( 80, 100 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Below(10).Range(50, false, 80, false).Above(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Above()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Above( 100 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.BelowOrEqual(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AboveOrEqual()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.AboveOrEqual( 100 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Below(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Below()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.Below( 100 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.AboveOrEqual(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BelowOrEqual()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static double Inverse(ulong i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (ulong i) => i.BelowOrEqual( 100 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(ulong)' has some missing Test Cases: i:ULong.Above(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}