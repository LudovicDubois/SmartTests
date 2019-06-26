using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class EnumTypeHelperTests: CodeFixVerifier
    {
        #region In One

        [Test]
        public void OneValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Monday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: dow:EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void TwoValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Monday, DayOfWeek.Wednesday ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: dow:EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AllValuesExceptOne()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: dow:EnumRange.Value(DayOfWeek.Wednesday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AllValuesInOne()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }

        #endregion


        #region In Two

        [Test]
        public void OneValues_OneValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Monday ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Tuesday ), out var value ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: dow:EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 ),
                                               new DiagnosticResultLocation( "Test0.cs", 30, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneValuesTwoValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Monday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Friday, DayOfWeek.Wednesday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: dow:EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 ),
                                               new DiagnosticResultLocation( "Test0.cs", 30, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AllValuesExceptOneInTwo()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Friday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: dow:EnumRange.Value(DayOfWeek.Wednesday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 20, 41 ),
                                               new DiagnosticResultLocation( "Test0.cs", 30, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AllValuesInTwo()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;
using SmartTests.Ranges;

namespace TestingProject
{
    class Class1
    {
        public static DayOfWeek Same(DayOfWeek dow) => dow;
    }

    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( Case( (DayOfWeek dow) => dow.Values( DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday ), out var value ),
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }

        #endregion


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}