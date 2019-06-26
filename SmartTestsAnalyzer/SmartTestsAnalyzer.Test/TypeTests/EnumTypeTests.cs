using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class EnumTypeTests: CodeFixVerifier
    {
        #region In One

        [Test]
        public void OneValue()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

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
            var result = RunTest( EnumRange.Value( DayOfWeek.Monday ), 
                                  () => Class1.Same( DayOfWeek.Monday ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

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
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Monday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
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
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Monday, DayOfWeek.Wednesday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
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
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Value(DayOfWeek.Wednesday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 )
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
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday ), 
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
        public void OneValue_OneValue()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

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
            var result = RunTest( EnumRange.Value( DayOfWeek.Monday ), 
                                  () => Class1.Same( DayOfWeek.Monday ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( EnumRange.Value( DayOfWeek.Tuesday ), 
                                  () => Class1.Same( DayOfWeek.Tuesday ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneValue_OneValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

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
            var result = RunTest( EnumRange.Value( DayOfWeek.Monday ), 
                                  () => Class1.Same( DayOfWeek.Monday ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Tuesday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OneValueTwoValues()
        {
            var test = @"
using System;
using NUnit.Framework;
using static SmartTests.SmartTest;

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
            var result = RunTest( EnumRange.Value( DayOfWeek.Monday ), 
                                  () => Class1.Same( DayOfWeek.Monday ) );

            Assert.That( result, Is.EqualTo(DayOfWeek.Monday) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Friday, DayOfWeek.Wednesday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Values(DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 )
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
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Friday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.Class1.Same(System.DayOfWeek)' has some missing Test Cases: EnumRange.Value(DayOfWeek.Wednesday)",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 19, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 )
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
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday ), 
                                  () => Class1.Same( value ) );

            Assert.That( result, Is.EqualTo(value) );
        }


        [Test]
        public void TestMethod2()
        {
            var result = RunTest( EnumRange.Values( out var value, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday ), 
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