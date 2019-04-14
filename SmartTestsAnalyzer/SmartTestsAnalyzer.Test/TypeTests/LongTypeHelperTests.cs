﻿using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class LongTypeHelperTests: CodeFixVerifier
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
        public static long Same(long i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, long.MaxValue ), out var value ), 
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
        public static long Same(long i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, 10 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (long i) => i.Range( 11, long.MaxValue ), out var value ), 
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
        public static long Div(long i, long b)
        {
            if( b == 0 )
                return 0;
            return (long)(i / b);
        }
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( 0, long.MaxValue ), out var value1 ) &
                                  ErrorCase( (long b) => b.Value( 0 ), out var value2 ), 
                                  () => Class1.Div( value1, value2 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Div(long, long)' has some missing Test Cases: b:Long.Below(0).Above(0)",
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
        public static long Same(Class2 i) => i.Data;
    }

    class Class2
    {
        public long Data { get; }
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (Class2 i) => i.Data.Range( long.MinValue, long.MaxValue ), out var value ), 
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
        public static long Same(long i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue + 1, long.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(long)' has some missing Test Cases: i:Long.Value(long.MinValue)",
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
        public static long Same(long i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, long.MaxValue - 1 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(long)' has some missing Test Cases: i:Long.Value(long.MaxValue)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( 1, long.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Below(1)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( 1, long.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, -1 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Value(0)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, -1 ).Range( 1, long.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Value(0)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, -1 ).Range( 1, 10 ).Range( 11, long.MaxValue ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Value(0)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Case( (long i) => i.Range( one, long.MaxValue ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 62 )
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, one ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 77 )
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, -1 ).Range( one, long.MaxValue ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 89 )
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = 1;
            var result = RunTest( Case( (long i) => i.Range( long.MinValue, -1 ).Range( 1, one ), out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 92 )
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( 1, 10 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Below(1).Above(10)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Range( -10, -1 ).Range( 1, 10 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Below(-10).Value(0).Above(10)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Above( 0 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.BelowOrEqual(0)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.AboveOrEqual( 1 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Below(1)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.Below( 0 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.AboveOrEqual(0)",
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
        public static double Inverse(long i) => 1 / i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (long i) => i.BelowOrEqual( -1 ), out var value ), 
                                  () => Class1.Inverse( value ) );

            Assert.That( 1 / result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Inverse(long)' has some missing Test Cases: i:Long.Above(-1)",
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