using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class ParameterLongPathTests: CodeFixVerifier
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => v.Major, AnyValue.IsValid ), 
                     () => MyTestClass.Method( new Version() ) );
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => v.Major, ValidValue.IsValid ), 
                           () => MyTestClass.Method( new Version() ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.Method(System.Version)' has some missing Test Cases: v.Major:ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 17, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void RightParameterName_Path3_MissingCases()
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
        public class MyData
        {
            public Version Version { get; }
        }

        public static void Method(MyData md) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (MyData md) => md.Version.Major, ValidValue.IsValid ), 
                           () => MyTestClass.Method( new MyData() ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.Method(TestingProject.MyTestClass.MyData)' has some missing Test Cases: md.Version.Major:ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 22, 22 )
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest1()
        {
            RunTest( Case( (Version a) => a.Major, AnyValue.IsValid ), 
                           () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            var expectedCase = new DiagnosticResult
                               {
                                   Id = "SmartTestsAnalyzer_MissingParameterCase",
                                   Message = "Test for 'TestingProject.MyTestClass.Method(System.Version)' has no Case for parameter 'v'.",
                                   Severity = DiagnosticSeverity.Error,
                                   Locations = new[]
                                               {
                                                   new DiagnosticResultLocation( "Test0.cs", 17, 22 )
                                               }
                               };
            var expectedWrong = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_WrongParameterName",
                                    Message = "Test for 'TestingProject.MyTestClass.Method(System.Version)' has some invalid parameter 'a'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 17, 28 )
                                                }
                                };

            VerifyCSharpDiagnostic( test, expectedCase, expectedWrong );
        }


        [Test]
        public void WrongParameterType()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest1()
        {
            RunTest( Case( (DateTime v) => v.Day, AnyValue.IsValid ), 
                           () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            var expectedWrongType = new DiagnosticResult
                                    {
                                        Id = "SmartTestsAnalyzer_WrongParameterType",
                                        Message = "Test for 'TestingProject.MyTestClass.Method(System.Version)' has some invalid parameter type 'System.DateTime' for parameter 'v'.",
                                        Severity = DiagnosticSeverity.Error,
                                        Locations = new[]
                                                    {
                                                        new DiagnosticResultLocation( "Test0.cs", 17, 28 )
                                                    }
                                    };

            VerifyCSharpDiagnostic( test, expectedWrongType );
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
        public static void Method(Version v1, Version v2) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v1) => v1.Major, AnyValue.IsValid ), 
                           () => MyTestClass.Method( new Version(), new Version() ) );
        }
    }
}";

            var expectedMissingCase = new DiagnosticResult
                                      {
                                          Id = "SmartTestsAnalyzer_MissingParameterCase",
                                          Message = "Test for 'TestingProject.MyTestClass.Method(System.Version, System.Version)' has no Case for parameter 'v2'.",
                                          Severity = DiagnosticSeverity.Error,
                                          Locations = new[]
                                                      {
                                                          new DiagnosticResultLocation( "Test0.cs", 17, 22 )
                                                      }
                                      };

            VerifyCSharpDiagnostic( test, expectedMissingCase );
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
        public static void Method(Version v1, Version v2) {}

        [Test]
        public void MyTest1()
        {
            RunTest( Case( (Version v1) => v1.Major, AnyValue.IsValid ) &
                     Case( (Version v2) => v2.Major, ValidValue.IsValid ),
                     () => MyTestClass.Method( new Version(), new Version() ) );
        }

        [Test]
        public void MyTest2()
        {
            RunTest( Case( (Version v2) => v2.Major, ValidValue.IsInvalid ),
                     () => MyTestClass.Method( new Version(), null ) );
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
        public static void Method(Version v1, Version v2) {}

        [Test]
        public void MyTest1()
        {
            RunTest( Case( (Version v1) => v1.Major, AnyValue.IsValid ) &
                     Case( (Version v2) => v2.Major, ValidValue.IsValid ),
                     () => MyTestClass.Method( new Version(), new Version() ) );
        }

        [Test]
        public void MyTest2()
        {
            RunTest( Case( (Version v1) => v1.Major, AnyValue.IsValid ) &
                     Case( (Version v2) => v2.Major, ValidValue.IsInvalid ),
                     () => MyTestClass.Method( new Version(), null ) );
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
        public static void Method() {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => v.Major, AnyValue.IsValid ),
                     () => Method() );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterName",
                               Message = "Test for 'TestingProject.MyTestClass.Method()' has some invalid parameter 'v'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 17, 28 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void DifferentPaths()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest1()
        {
            RunTest( Case( (Version v) => v.Major, ValidValue.IsValid ),
                     () => MyTestClass.Method( new Version() ) );
        }

        [Test]
        public void MyTest2()
        {
            RunTest( Case( (Version v) => v.Minor, ValidValue.IsInvalid ),
                     () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.Method(System.Version)' has some missing Test Cases: v.Major:ValidValue.IsValid & v.Minor:ValidValue.IsValid and v.Major:ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 17, 22 ),
                                               new DiagnosticResultLocation( "Test0.cs", 24, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void BadPath_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            Version v2 = new Version();
            RunTest( Case( (Version v) => v2.Major, ValidValue.IsValid ),
                     () => MyTestClass.Method( new Version() ) );
       }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterPath",
                               Message = "Test for 'TestingProject.MyTestClass.Method(System.Version)' has an invalid path 'v2.Major' for parameter 'v'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 18, 43 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Parenthesized_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => ((v).Major), AnyValue.IsValid ), 
                     () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void Parenthesssssized_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => ((((((v))).Major))), AnyValue.IsValid ), 
                     () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void Casted_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => ((double)((Version)v).Major), AnyValue.IsValid ), 
                     () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void Parenthesized_Casted_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => (((double)((Version)v).Major)), AnyValue.IsValid ), 
                     () => MyTestClass.Method( new Version() ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void NotAPath_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => v.Major + 0, ValidValue.IsValid ),
                     () => MyTestClass.Method( new Version() ) );
       }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterPath",
                               Message = "Test for 'TestingProject.MyTestClass.Method(System.Version)' has an invalid path 'v.Major + 0' for parameter 'v'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 17, 43 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void FieldPath_ParameterCase()
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
        public class MyData
        {
            public int Data;
        }

        public static void Method(MyData md) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (MyData md) => md.Data, ValidValue.IsValid ), 
                           () => MyTestClass.Method( new MyData() ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.Method(TestingProject.MyTestClass.MyData)' has some missing Test Cases: md.Data:ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 21, 22 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void MethodPath_ParameterCase()
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
        public static void Method(Version v) {}

        [Test]
        public void MyTest()
        {
            RunTest( Case( (Version v) => v.ToString(), ValidValue.IsValid ),
                     () => MyTestClass.Method( new Version() ) );
       }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterPath",
                               Message = "Test for 'TestingProject.MyTestClass.Method(System.Version)' has an invalid path 'v.ToString()' for parameter 'v'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 17, 43 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}