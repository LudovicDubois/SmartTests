using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class ParameterNameTests: CodeFixVerifier
    {
        [Test]
        public void RightParameterName()
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
        [Test]
        public void RightParameterName()
        {
            var result = RunTest( Case( ""d"", ValidValue.Valid ), 
                                  () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingParameterCases",
                               Message = "Tests for 'Math.Sqrt' has some missing Test Cases for parameter 'd': ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 46 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void WrongParameterName()
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
        [Test]
        public void WrongParameterName()
        {
            var result = RunTest( Case( ""value"", ValidValue.Valid ), 
                                  () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }

        [Test]
        public void WrongParameterName2()
        {
            Assert.Throws<ArgumentOutOfRangeException>( 
              () => RunTest( Case( ""value"", ValidValue.Invalid ), 
                             () => Math.Sqrt(-2) ) );
        }
    }
}";
            var expectedMissing = new DiagnosticResult
                                  {
                                      Id = "SmartTestsAnalyzer_MissingParameterCase",
                                      Message = "Test for 'Math.Sqrt' has no Case for parameter 'd'.",
                                      Severity = DiagnosticSeverity.Error,
                                      Locations = new[]
                                                  {
                                                      new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                                  }
                                  };
            var expectedWrong = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_WrongParameterName",
                                    Message = "Test for 'Math.Sqrt' has some invalid parameter 'value'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 15, 41 )
                                                }
                                };

            VerifyCSharpDiagnostic( test, expectedMissing, expectedWrong );
        }


        [Test]
        public void Missing1ParameterCase()
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
        [Test]
        public void TestMethod()
        {
            var reminder = default(int);
            var result = RunTest( Case( ""a"", AnyValue.Valid ),
                                  () => Math.DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }
    }
}";

            var expectedMissingCase = new DiagnosticResult
                                      {
                                          Id = "SmartTestsAnalyzer_MissingParameterCase",
                                          Message = "Test for 'Math.DivRem' has no Case for parameter 'b'.",
                                          Severity = DiagnosticSeverity.Error,
                                          Locations = new[]
                                                      {
                                                          new DiagnosticResultLocation( "Test0.cs", 16, 35 )
                                                      }
                                      };

            VerifyCSharpDiagnostic( test, expectedMissingCase );
        }


        [Test]
        public void Missing2ParameterCases()
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
        [Test]
        public void TestMethod()
        {
            var reminder = default(int);
            var result = RunTest( Case( AnyValue.Valid ),
                                  () => Math.DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }
    }
}";

            var expectedMissingCaseA = new DiagnosticResult
                                       {
                                           Id = "SmartTestsAnalyzer_MissingParameterCase",
                                           Message = "Test for 'Math.DivRem' has no Case for parameter 'a'.",
                                           Severity = DiagnosticSeverity.Error,
                                           Locations = new[]
                                                       {
                                                           new DiagnosticResultLocation( "Test0.cs", 16, 35 )
                                                       }
                                       };
            var expectedMissingCaseB = new DiagnosticResult
                                       {
                                           Id = "SmartTestsAnalyzer_MissingParameterCase",
                                           Message = "Test for 'Math.DivRem' has no Case for parameter 'b'.",
                                           Severity = DiagnosticSeverity.Error,
                                           Locations = new[]
                                                       {
                                                           new DiagnosticResultLocation( "Test0.cs", 16, 35 )
                                                       }
                                       };

            VerifyCSharpDiagnostic( test, expectedMissingCaseA, expectedMissingCaseB );
        }


        [Test]
        public void MissingNoParameterCases()
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
        [Test]
        public void TestMethod()
        {
            var reminder = default(int);
            var result = RunTest( Case( ""a"", AnyValue.Valid ) &
                                  Case( ""b"", ValidValue.Valid ),
                                  () => Math.DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }

        [Test]
        public void TestMethod2()
        {
            var reminder;
            Assert.Throws<DivideByZeroException>( () => RunTest( Case( ""a"", AnyValue.Valid ) &
                                                                 Case( ""b"", ValidValue.Invalid ),
                                                                 () => Math.DivRem( 7, 0, out reminder ) ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void MissingNoParameterCases2()
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
        [Test]
        public void TestMethod()
        {
            var reminder = default(int);
            var result = RunTest( Case( ""a"", AnyValue.Valid ) &
                                  Case( ""b"", ValidValue.Valid ),
                                  () => Console.WriteLine() );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }

        [Test]
        public void TestMethod2()
        {
            var reminder;
            Assert.Throws<DivideByZeroException>( () => RunTest( Case( ""a"", AnyValue.Valid ) &
                                                                 Case(""b"", ValidValue.Invalid),
                                                                 () => DivRem2(7, 0, out reminder) ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void NoParameterNeeded_NoCase()
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
        private static void NoParameter()
        { }

        [Test]
        public void NoCase()
        {
            RunTest( AnyValue.Valid,
                     () => NoParameter() );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void NoParameterNeeded_Case()
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
        private static void NoParameter()
        { }

        [Test]
        public void CaseNoParameter()
        {
            RunTest( Case( AnyValue.Valid ),
                     () => NoParameter() );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void NoParameterNeeded_NullCase()
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
        private static void NoParameter()
        { }

        [Test]
        public void NullCase()
        {
            RunTest( Case( null, AnyValue.Valid ),
                     () => NoParameter() );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void NoParameterNeeded_ParameterCase()
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
        private static void NoParameter()
        { }

        [Test]
        public void ParameterCase()
        {
            RunTest( Case( ""value"", AnyValue.Valid ),
                     () => NoParameter() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterName",
                               Message = "Test for 'MyTestClass.NoParameter' has some invalid parameter 'value'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 28 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}