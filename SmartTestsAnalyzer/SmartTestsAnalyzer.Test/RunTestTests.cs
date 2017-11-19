using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class RunTestTests: CodeFixVerifier
    {
        [Test]
        public void Criteria_Expression_FuncT()
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
            var result = RunTest( ValidValue.IsValid, () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }

        [Test]
        public void Criteria_Expression_FuncActContextT()
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
            var result = RunTest( ValidValue.IsValid, ctx => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Case_Expression_FuncT()
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
            var result = RunTest( Case( ValidValue.IsValid ), () => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Case_Expression_FuncActContextT()
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
            var result = RunTest( Case( ValidValue.IsValid ), ctx => Math.Sqrt(4) );

            Assert.That( result, Is.EqualTo(2) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'System.Math.Sqrt(double)' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 15, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Criteria_ActT()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class RunTestTests
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }

            public int Property { get; set; }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.IsValid,
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.RunTestTests.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Case_ActT()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class RunTestTests
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }

            public int Property { get; set; }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( Case( ValidValue.IsValid ),
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.RunTestTests.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Criteria_ExpressionAction()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class RunTestTests
    {
        public class MyClass
        {
            public void DoNothing() {}
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            RunTest( ValidValue.IsValid,
                     () => mc.DoNothing() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.RunTestTests.MyClass.DoNothing()' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Case_ExpressionAction()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class RunTestTests
    {
        public class MyClass
        {
            public void DoNothing() {}
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            RunTest( Case( ValidValue.IsValid ),
                     () => mc.DoNothing() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.RunTestTests.MyClass.DoNothing()' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Criteria_ExpressionActContextAction()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class RunTestTests
    {
        public class MyClass
        {
            public void DoNothing() {}
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            RunTest( ValidValue.IsValid,
                     ctx => mc.DoNothing() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.RunTestTests.MyClass.DoNothing()' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Test]
        public void Case_ExpressionActContextAction()
        {
            var test = @"
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class RunTestTests
    {
        public class MyClass
        {
            public void DoNothing() {}
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            RunTest( Case( ValidValue.IsValid ),
                     ctx => mc.DoNothing() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.RunTestTests.MyClass.DoNothing()' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}