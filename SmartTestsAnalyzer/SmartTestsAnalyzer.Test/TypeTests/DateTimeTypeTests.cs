using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DateTimeTypeTests: CodeFixVerifier
    {
        [Test]
        public void STFullRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void FullRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void FullRange_SystemDateTime()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

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
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, false, DateTime.MaxValue, true, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Value(DateTime.MinValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AlmostFullRangeMax()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, true, DateTime.MaxValue, false, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Value(DateTime.MaxValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BadRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( new DateTime(2019, 6, 3), new DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var minMax = new DiagnosticResult
                         {
                             Id = "SmartTestsAnalyzer_MinShouldBeLessThanMax",
                             Message = "Min value (new DateTime(2019, 6, 3)) should be less than max value (new DateTime(2019, 1, 1))",
                             Severity = DiagnosticSeverity.Error,
                             Locations = new[]
                                         {
                                             new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                         }
                         };
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Range(DateTime.MinValue, DateTime.MaxValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, minMax, expected );
        }


        [Test]
        public void OneChunkInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( new DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeTypedRoot()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests;
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
            var result = RunTest( SmartTest.DateTimeRange.Range( new DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeFullRoot()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( SmartTests.SmartTest.DateTimeRange.Range( new DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void TwoChunksInTwoRanges()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( new DateTime(2019, 6, 3), false, DateTime.MaxValue, true, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, true, new DateTime(2019, 6, 3), false, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Value(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 28, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void TwoChunksInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, new DateTime(2019, 1, 1) ).Range( new DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Range(new DateTime(2019, 1, 1), false, new DateTime(2019, 6, 3), false)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void ThreeChunksInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, new DateTime(2019, 1, 1) ).Range( new DateTime(2019, 5, 7), new DateTime(2019, 6, 3) ).Range( new DateTime(2019, 6, 3, 12, 0, 0), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Range(new DateTime(2019, 1, 1), false, new DateTime(2019, 5, 7), false).Range(new DateTime(2019, 6, 3), false, new DateTime(2019, 6, 3, 12, 0, 0), false)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkGetValueInOneRange()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( new DateTime(2019, 6, 3), DateTime.MaxValue ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_LocalVariable()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var d = new DateTime(2019, 6, 3);
            var result = RunTest( DateTimeRange.Range( d, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 56 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Field()
        {
            var test = @"
using System;
using NUnit.Framework;
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
        private DateTime d = new DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTimeRange.Range( d, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 56 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Property()
        {
            var test = @"
using System;
using NUnit.Framework;
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
        private DateTime d => new DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTimeRange.Range( d, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 56 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Indexer()
        {
            var test = @"
using System;
using NUnit.Framework;
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
        private DateTime this[int i] => new DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTimeRange.Range( this[0], DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 56 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Method()
        {
            var test = @"
using System;
using NUnit.Framework;
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
        private DateTime d() => new DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTimeRange.Range( d(), DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 56 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMaxNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var one = new DateTime(2019, 6, 3);
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, one, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 75 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeAddMinNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var one = new DateTime(2019, 6, 3);
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, new DateTime(2019, 6, 3) ).Range( one, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 109 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeAddMaxNotAConstant()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var one = new DateTime(2019, 6, 3);;
            var result = RunTest( DateTimeRange.Range( DateTime.MinValue, new DateTime(2019, 6, 3) ).Range( new DateTime(2019, 6, 3), one, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 20, 135 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMax()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( new DateTime(2019, 1, 1), new DateTime(2019, 6, 3), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 1, 1)).Above(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMiddleMax()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Range( new DateTime(2019, 1, 1), new DateTime(2019, 6, 3) ).Range( new DateTime(2019, 6, 14), new DateTime(2019, 12, 31), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 1, 1)).Range(new DateTime(2019, 6, 3), false, new DateTime(2019, 6, 14), false).Above(new DateTime(2019, 12, 31))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Above()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Above( new DateTime( 2019, 1, 1 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.BelowOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Above_GetValue()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Above( new DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.BelowOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AboveOrEqual()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.AboveOrEqual( new DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AboveOrEqual_GetValue()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.AboveOrEqual( new DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Below(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Below()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Below( new DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.AboveOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Below_GetValue()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.Below( new DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.AboveOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BelowOrEqual()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.BelowOrEqual( new DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Above(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BelowOrEqual_GetValue()
        {
            var test = @"
using System;
using NUnit.Framework;
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
            var result = RunTest( DateTimeRange.BelowOrEqual( new DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTimeRange.Above(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void GetErrorValue_Error()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Min(DateTime i, DateTime j) => i < j ? i : j;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ""i"", DateTimeRange.Range( DateTime.MinValue, true, new DateTime(2019, 1, 1), false ).Range( new DateTime(2019, 1, 1), false, DateTime.MaxValue, true, out var valueI ) ) &
                                  Case( ""j"", DateTimeRange.Range( new DateTime(2019, 1, 1), DateTime.MaxValue, out var valueJ ) ),
                                  () => Class1.Min( valueI, valueJ ) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( ""i"", DateTimeRange.Range( new DateTime(2019, 1, 1), new DateTime(2019, 1, 1) ).GetErrorValue( out var value ) ),
                                  () => Class1.Min( value, new DateTime(2019, 6, 3) ) );
        }

    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Min(System.DateTime, System.DateTime)' has some missing Test Cases: i:DateTimeRange.Below(new DateTime(2019, 1, 1)).Above(new DateTime(2019, 1, 1)) & j:DateTimeRange.Below(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void GetErrorValue_NoError()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static DateTime Min(DateTime i, DateTime j) => i < j ? i : j;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ""i"", DateTimeRange.Range( DateTime.MinValue, true, new DateTime(2019, 1, 1), false ).Range( new DateTime(2019, 1, 1), false, DateTime.MaxValue, true, out var valueI ) ) &
                                  Case( ""j"", DateTimeRange.Range( DateTime.MinValue, DateTime.MaxValue, out var valueJ ) ),
                                  () => Class1.Min( valueI, valueJ ) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( ""i"", DateTimeRange.Range( new DateTime(2019, 1, 1), new DateTime(2019, 1, 1) ).GetErrorValue( out var value ) ),
                                  () => Class1.Min( value, new DateTime(2019, 6, 3) ) );

            Assert.That( result, Is.EqualTo(value) );
        }

    }
}";
            VerifyCSharpDiagnostic( test );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}