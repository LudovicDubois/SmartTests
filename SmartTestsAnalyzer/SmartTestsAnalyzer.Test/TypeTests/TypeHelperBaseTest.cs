using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    abstract class TypeHelperBaseTest: CodeFixVerifier
    {
        protected TypeHelperBaseTest( string type, string smartType )
        {
            _Type = type;
            _SmartType = smartType;
            var isUnsigned = _Type[ 0 ] == 'u' || _Type == "byte";
            _Min = isUnsigned
                       ? "0"
                       : _Type + ".MinValue";
        }


        private readonly string _Type;
        private readonly string _SmartType;
        private readonly string _Min;


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
        public static " + _Type + @" Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( " + _Type + @".MinValue, " + _Type + @".MaxValue ), out var value ), 
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
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static " + _Type + @" Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( " + _Type + @".MinValue, 10 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( 10, false, " + _Type + @".MaxValue, true ), out var value ), 
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
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static " + _Type + @" Min(" + _Type + @" a, " + _Type + @" b) => a < b ? a : b;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( 0, " + _Type + @".MaxValue ), out var value1 ) &
                                  ErrorCase( (" + _Type + @" b) => b.Value( 10 ), out var value2 ), 
                                  () => Class1.Min( value1, value2 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Min({_Type}, {_Type})' has some missing Test Cases: b:{_SmartType}.Below(10).Above(10)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 46 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
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
        public static " + _Type + @" Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( " + _Type + @".MinValue, false, " + _Type + @".MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Value({_Min})",
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
        public static " + _Type + @" Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( " + _Type + @".MinValue, true, " + _Type + @".MaxValue, false ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Value({_Type}.MaxValue)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 100, " + _Type + @".MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Below(100)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 100, " + _Type + @".MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( " + _Type + @".MinValue, 50 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Range(50, false, 100, false)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( " + _Type + @".MinValue, 50 ).Range( 100, " + _Type + @".MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Range(50, false, 100, false)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( " + _Type + @".MinValue, 50 ).Range( 50, false, 99, false ).Range( 99, false, " + _Type + @".MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Value(99)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            " + _Type + @" one = 1;
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( one, " + _Type + @".MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 60 + _Type.Length )
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            " + _Type + @" one = 1;
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 0, one ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 63 + _Type.Length )
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            " + _Type + @" one = 1;
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 0, 100 ).Range( one, " + _Type + @".MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 76 + _Type.Length )
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            " + _Type + @" one = 1;
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 0, 100 ).Range( 1, one ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 79 + _Type.Length )
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 50, 100 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Below(50).Above(100)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Range( 10, 50 ).Range( 80, 100 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Below(10).Range(50, false, 80, false).Above(100)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Above( 100 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.BelowOrEqual(100)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.AboveOrEqual( 100 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Below(100)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.Below( 100 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.AboveOrEqual(100)",
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
        public static " + _Type + " Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( " + _Type + @" i ) => i.BelowOrEqual( 100 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.Class1.Same({_Type})' has some missing Test Cases: i:{_SmartType}.Above(100)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        #region AvoidedValues


        [Test]
        public void PropertySet_Avoided()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Assertions;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class MyClass
    {
        public " + _Type + @" MyProperty { get; set; }
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void MyPropertyTest_Set()
        {
            var mc = new MyClass();

            RunTest( Case( (" + _Type + @" value) => value.Above(10), out var val, mc.MyProperty),
                     Assign( () => mc.MyProperty, val),
                     SmartAssert.ChangedTo() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = $"Tests for 'TestingProject.MyClass.MyProperty [set]' has some missing Test Cases: value:{_SmartType}.BelowOrEqual(10)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 22, 28 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void FullRange2_Avoided1()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static " + _Type + @" Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( " + _Type + @".MinValue, 10 ), out var value, (" + _Type + @")1 ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( 10, false, " + _Type + @".MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }


        [Test]
        public void ErrorCase_Avoided1()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static " + _Type + @" Min(" + _Type + @" a, " + _Type + @" b) => a < b ? a : b;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( 0, " + _Type + @".MaxValue ), out var value1, (" + _Type + @")1 ) &
                                  ErrorCase( (" + _Type + @" b) => b.Value( 10 ), out var value2 ), 
                                  () => Class1.Min( value1, value2 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = $"Tests for 'TestingProject.Class1.Min({_Type}, {_Type})' has some missing Test Cases: b:{_SmartType}.Below(10).Above(10)",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 46 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void FullRange2_Avoided2()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static " + _Type + @" Same(" + _Type + @" i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( " + _Type + @".MinValue, 10 ), out var value, (" + _Type + @")1, (" + _Type + @")2 ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( 10, false, " + _Type + @".MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }


        [Test]
        public void ErrorCase_Avoided2()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static " + _Type + @" Min(" + _Type + @" a, " + _Type + @" b) => a < b ? a : b;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (" + _Type + @" i) => i.Range( 0, " + _Type + @".MaxValue ), out var value1, (" + _Type + @")1, (" + _Type + @")2 ) &
                                  ErrorCase( (" + _Type + @" b) => b.Value( 10 ), out var value2 ), 
                                  () => Class1.Min( value1, value2 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = $"Tests for 'TestingProject.Class1.Min({_Type}, {_Type})' has some missing Test Cases: b:{_SmartType}.Below(10).Above(10)",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 46 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        #endregion


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}