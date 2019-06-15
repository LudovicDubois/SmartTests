using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class DateTimeTypeTests: CodeFixVerifier
    {
        [Test]
        public void FullRange()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( DateTime.MinValue, DateTime.MaxValue, out var value ), 
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
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( System.DateTime.MinValue, System.DateTime.MaxValue, out var value ), 
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
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( System.DateTime.MinValue, false, System.DateTime.MaxValue, true, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Value(DateTime.MinValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AlmostFullRangeMax()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( System.DateTime.MinValue, true, System.DateTime.MaxValue, false, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Value(DateTime.MaxValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BadRange()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( new System.DateTime(2019, 6, 3), new System.DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var minMax = new DiagnosticResult
                         {
                             Id = "SmartTestsAnalyzer_MinShouldBeLessThanMax",
                             Message = "Min value (2019-06-03 00:00:00) should be less than max value (2019-01-01 00:00:00)",
                             Severity = DiagnosticSeverity.Error,
                             Locations = new[]
                                         {
                                             new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                         }
                         };
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Range(DateTime.MinValue, DateTime.MaxValue)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, minMax, expected );
        }


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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( new System.DateTime(2019, 6, 3), System.DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeTypedRoot()
        {
            var test = @"
using NUnit.Framework;
using SmartTests;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( SmartTest.DateTime.Range( new System.DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeFullRoot()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( SmartTests.SmartTest.DateTime.Range( new System.DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( new System.DateTime(2019, 6, 3), false, DateTime.MaxValue, true, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( DateTime.Range( DateTime.MinValue, true, new System.DateTime(2019, 6, 3), false, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Value(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( DateTime.MinValue, new System.DateTime(2019, 1, 1) ).Range( new System.DateTime(2019, 6, 3), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Range(new DateTime(2019, 1, 1), false, new DateTime(2019, 6, 3), false)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( DateTime.MinValue, new System.DateTime(2019, 1, 1) ).Range( new System.DateTime(2019, 5, 7), new System.DateTime(2019, 6, 3) ).Range( new System.DateTime(2019, 6, 3, 12, 0, 0), DateTime.MaxValue, out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Range(new DateTime(2019, 1, 1), false, new DateTime(2019, 5, 7), false).Range(new DateTime(2019, 6, 3), false, new DateTime(2019, 6, 3, 12, 0, 0), false)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( new System.DateTime(2019, 6, 3), DateTime.MaxValue ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_LocalVariable()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var d = new System.DateTime(2019, 6, 3);
            var result = RunTest( DateTime.Range( d, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 51 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Field()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        private System.DateTime d = new System.DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( d, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 51 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Property()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        private System.DateTime d => new System.DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( d, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 51 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Indexer()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        private System.DateTime this[int i] => new System.DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( this[0], DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 51 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunk_RangeMinNotADateTimeCreation_Method()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        private System.DateTime d() => new System.DateTime(2019, 6, 3);
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( d(), DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 51 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = new System.DateTime(2019, 6, 3);
            var result = RunTest( DateTime.Range( DateTime.MinValue, one, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 70 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = new System.DateTime(2019, 6, 3);
            var result = RunTest( DateTime.Range( DateTime.MinValue, new System.DateTime(2019, 6, 3) ).Range( one, DateTime.MaxValue, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 111 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
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
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var one = new System.DateTime(2019, 6, 3);;
            var result = RunTest( DateTime.Range( DateTime.MinValue, new System.DateTime(2019, 6, 3) ).Range( new System.DateTime(2019, 6, 3), one, out var value ), 
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
                                               new DiagnosticResultLocation( "Test0.cs", 19, 144 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, 0, expected );
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMax()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( new System.DateTime(2019, 1, 1), new System.DateTime(2019, 6, 3), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 1, 1)).Above(new DateTime(2019, 6, 3))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneChunkInOneRangeMissingMinMiddleMax()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Range( new System.DateTime(2019, 1, 1), new System.DateTime(2019, 6, 3) ).Range( new System.DateTime(2019, 6, 14), new System.DateTime(2019, 12, 31), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 1, 1)).Range(new DateTime(2019, 6, 3), false, new DateTime(2019, 6, 14), false).Above(new DateTime(2019, 12, 31))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Above()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Above( new System.DateTime( 2019, 1, 1 ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.BelowOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Above_GetValue()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Above( new System.DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.BelowOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AboveOrEqual()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.AboveOrEqual( new System.DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AboveOrEqual_GetValue()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.AboveOrEqual( new System.DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Below(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Below()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Below( new System.DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.AboveOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Below_GetValue()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.Below( new System.DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.AboveOrEqual(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BelowOrEqual()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.BelowOrEqual( new System.DateTime(2019, 1, 1), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Above(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BelowOrEqual_GetValue()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Same(System.DateTime i) => i;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( DateTime.BelowOrEqual( new System.DateTime(2019, 1, 1) ).GetValidValue( out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DateTime)' has some missing Test Cases: DateTime.Above(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void GetErrorValue_Error()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Computation(System.DateTime i, System.DateTime j) => i > j ? i : j;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ""i"", DateTime.Range( DateTime.MinValue, true, new System.DateTime(2019, 1, 1), false ).Range( new System.DateTime(2019, 1, 1), false, DateTime.MaxValue, true, out var valueI ) ) &
                                  Case( ""j"", DateTime.Range( new System.DateTime(2019, 1, 1), DateTime.MaxValue, out var valueJ ) ),
                                  () => Class1.Computation( valueI, valueJ ) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( ""i"", DateTime.Range( new System.DateTime(2019, 1, 1), new System.DateTime(2019, 1, 1) ).GetErrorValue( out var value ) ),
                                  () => Class1.Computation( value, new System.DateTime(2019, 6, 3) ) );
        }

    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Computation(System.DateTime, System.DateTime)' has some missing Test Cases: i:DateTime.Below(new DateTime(2019, 1, 1)).Above(new DateTime(2019, 1, 1)) & j:DateTime.Below(new DateTime(2019, 1, 1))",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 26, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void GetErrorValue_NoError()
        {
            var test = @"
using NUnit.Framework;
using static SmartTests.SmartTest;

namespace TestingProject
{
    class Class1
    {
        public static System.DateTime Computation(System.DateTime i, System.DateTime j) => i > j ? i : j;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ""i"", DateTime.Range( DateTime.MinValue, true, new System.DateTime(2019, 1, 1), false ).Range( new System.DateTime(2019, 1, 1), false, DateTime.MaxValue, true, out var valueI ) ) &
                                  Case( ""j"", DateTime.Range( DateTime.MinValue, DateTime.MaxValue, out var valueJ ) ),
                                  () => Class1.Computation( valueI, valueJ ) );
        }

        [Test]
        public void Test2Method()
        {
            var result = RunTest( Case( ""i"", DateTime.Range( new System.DateTime(2019, 1, 1), new System.DateTime(2019, 1, 1) ).GetErrorValue( out var value ) ),
                                  () => Class1.Computation( value, new System.DateTime(2019, 6, 3) ) );

            Assert.That( result, Is.EqualTo(value) );
        }

    }
}";
            VerifyCSharpDiagnostic( test );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}