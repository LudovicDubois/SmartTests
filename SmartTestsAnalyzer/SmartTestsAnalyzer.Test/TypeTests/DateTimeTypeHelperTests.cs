using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DateTimeTypeHelperTests: CodeFixVerifier
    {
        [Test]
        public void FullRange()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }


        [Test]
        public void FullRange2()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DateTime i) => i.Range( DateTime.MinValue, new DateTime(2019, 1, 1) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (DateTime i) => i.Range( new DateTime(2019, 1, 1), false, DateTime.MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }


        [Test]
        public void ErrorCase()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Min(DateTime i, DateTime b) => i < b ? i : b;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DateTime i) => i.Range( new DateTime(2019, 1, 1), DateTime.MaxValue ), out var value1 ) &
                                  ErrorCase( (DateTime b) => b.Value( new DateTime(2019, 6, 21) ), out var value2 ), 
                                  () => Class1.Min(value1, value2) );
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Min(System.DateTime, System.DateTime)' has some missing Test Cases: b:DateTimeRange.Below(new DateTime(2019, 6, 21)).Above(new DateTime(2019, 6, 21))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 46 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void AlmostFullRangeMin()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, false, DateTime.MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Value(DateTime.MinValue)",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void AlmostFullRangeMax()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, true, DateTime.MaxValue, false ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Value(DateTime.MaxValue)",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void OneChunkInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( new DateTime(2019, 1, 1), DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Below(new DateTime(2019, 1, 1))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void TwoChunksInTwoRanges()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( new DateTime(2019, 6, 21), DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, new DateTime(2019, 1, 1) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Range(new DateTime(2019, 1, 1), false, new DateTime(2019, 6, 21), false)",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 ),
                                               new DiagnosticResultLocation( "Test0.cs", 29, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void TwoChunksInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, new DateTime(2019, 1, 1) ).Range( new DateTime(2019, 6, 21), DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Range(new DateTime(2019, 1, 1), false, new DateTime(2019, 6, 21), false)",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void ThreeChunksInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, new DateTime(2019, 1, 1) ).Range( new DateTime(2019, 1, 1), false, new DateTime(2019, 6, 21), false ).Range( new DateTime(2019, 6, 21), false, DateTime.MaxValue, true ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Value(new DateTime(2019, 6, 21))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void OneChunk_RangeMinNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            DateTime one = new DateTime(2019, 1, 1);
            var result = RunTest( Case( ( DateTime i ) => i.Range( one, DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_NotADateCreation",
                Message = "A DateTime instantiation with constants is expected",
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 68 )
                                           }
            };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeMinNotAConstant2()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            int one = 1;
            var result = RunTest( Case( ( DateTime i ) => i.Range( new DateTime(2019, 1, one), DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_NotADateCreation",
                Message = "A DateTime instantiation with constants is expected",
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 68 )
                                           }
            };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeMaxNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            DateTime one = new DateTime(2019, 1, 1);
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, one ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_NotADateCreation",
                Message = "A DateTime instantiation with constants is expected",
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 87 )
                                           }
            };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeAddMinNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            DateTime one = new DateTime(2019, 1, 1);
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, new DateTime(2019,1,1) ).Range( one, DateTime.MaxValue ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_NotADateCreation",
                Message = "A DateTime instantiation with constants is expected",
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 119 )
                                           }
            };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunk_RangeAddMaxNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            DateTime one = new DateTime(2019, 1, 1);
            var result = RunTest( Case( ( DateTime i ) => i.Range( DateTime.MinValue, new DateTime(2018, 1, 1) ).Range( new DateTime(2018, 1, 1), one ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_NotADateCreation",
                Message = "A DateTime instantiation with constants is expected",
                Severity = DiagnosticSeverity.Error,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 147 )
                                           }
            };

            VerifyCSharpDiagnostic(test, 0, expected);
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMax()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( new DateTime(2019, 1, 1), new DateTime(2019, 6, 23) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Below(new DateTime(2019, 1, 1)).Above(new DateTime(2019, 6, 23))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMiddleMax()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Range( new DateTime(2019, 1, 1), new DateTime(2019, 6, 23) ).Range( new DateTime(2019, 12, 31), new DateTime(2020, 5, 7) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Below(new DateTime(2019, 1, 1)).Range(new DateTime(2019, 6, 23), false, new DateTime(2019, 12, 31), false).Above(new DateTime(2020, 5, 7))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Above()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Above( new DateTime(2019, 1, 1) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.BelowOrEqual(new DateTime(2019, 1, 1))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void AboveOrEqual()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.AboveOrEqual( new DateTime(2019, 1, 1) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Below(new DateTime(2019, 1, 1))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Below()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.Below( new DateTime(2019, 1, 1) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.AboveOrEqual(new DateTime(2019, 1, 1))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void BelowOrEqual()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Ranges;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Same(DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ( DateTime i ) => i.BelowOrEqual( new DateTime(2019, 1, 1) ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SmartTestsAnalyzer_MissingCases",
                Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: i:DateTimeRange.Above(new DateTime(2019, 1, 1))",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();

    }
}