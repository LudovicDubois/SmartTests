using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class ParameterNameTests: CodeFixVerifier
    {
        [Test]
        public void RightParameterName_NotMissingCases()
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
        public void MyTest()
        {
            var result = RunTest( Case( ""d"", AnyValue.Valid ), 
                                  () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void RightParameterName_MissingCases()
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
        public void MyTest()
        {
            var result = RunTest( Case( ""d"", ValidValue.IsValid ), 
                                  () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: d:ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
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
        public void MyTest1()
        {
            var result = RunTest( Case( ""value"", AnyValue.Valid ), 
                                  () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";

            var expectedCase = new DiagnosticResult
                               {
                                   Id = "SmartTestsAnalyzer_MissingParameterCase",
                                   Message = "Test for 'System.Math.Sqrt(double)' has no Case for parameter 'd'.",
                                   Severity = DiagnosticSeverity.Error,
                                   Locations = new[]
                                               {
                                                   new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                               }
                               };
            var expectedWrong = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_WrongParameterName",
                                    Message = "Test for 'System.Math.Sqrt(double)' has some invalid parameter 'value'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 15, 41 )
                                                }
                                };

            VerifyCSharpDiagnostic( test, expectedCase, expectedWrong );
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
        public void MyTest()
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
                                          Message = "Test for 'System.Math.DivRem(int, int, out int)' has no Case for parameter 'b'.",
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
        public void MyTest()
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
                                           Message = "Test for 'System.Math.DivRem(int, int, out int)' has no Case for parameter 'a'.",
                                           Severity = DiagnosticSeverity.Error,
                                           Locations = new[]
                                                       {
                                                           new DiagnosticResultLocation( "Test0.cs", 16, 35 )
                                                       }
                                       };
            var expectedMissingCaseB = new DiagnosticResult
                                       {
                                           Id = "SmartTestsAnalyzer_MissingParameterCase",
                                           Message = "Test for 'System.Math.DivRem(int, int, out int)' has no Case for parameter 'b'.",
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
        public void MyTest1()
        {
            var reminder = default(int);
            var result = RunTest( Case( ""a"", AnyValue.Valid ) &
                                  Case( ""b"", ValidValue.IsValid ),
                                  () => Math.DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }

        [Test]
        public void MyTest2()
        {
            int reminder;
            Assert.Throws<DivideByZeroException>( () => RunTest( Case( ""b"", ValidValue.IsInvalid ),
                                                                 () => Math.DivRem( 7, 0, out reminder ) ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void MissingNoParameterCases_ErrorNotAlone()
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
        public void MyTest1()
        {
            var reminder = default(int);
            var result = RunTest( Case( ""a"", AnyValue.Valid ) &
                                  Case( ""b"", ValidValue.IsValid ),
                                  () => Math.DivRem( 7, 3, out reminder ) );

            Assert.That( result, Is.EqualTo( 2 ) );
            Assert.That( reminder, Is.EqualTo( 1 ) );
        }

        [Test]
        public void Mytest2()
        {
            int reminder;
            Assert.Throws<DivideByZeroException>( () => RunTest( Case( ""a"", AnyValue.Valid ) &
                                                                 Case( ""b"", ValidValue.IsInvalid ),
                                                                 () => Math.DivRem( 7, 0, out reminder ) ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void NoParameterNeeded_NoCase()
        {
            var test = @"
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
        public void MyTest()
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
        public void MyTest()
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
        public void MyTest()
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
        public void MyTest()
        {
            RunTest( Case( ""value"", AnyValue.Valid ),
                     () => NoParameter() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterName",
                               Message = "Test for 'TestingProject.MyTestClass.NoParameter()' has some invalid parameter 'value'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 17, 28 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}