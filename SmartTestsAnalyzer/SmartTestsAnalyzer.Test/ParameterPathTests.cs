using Microsoft.CodeAnalysis;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class ParameterPathTests: CodeFixVerifier
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
            var result = RunTest( Case( (double d) => d, AnyValue.IsValid ), 
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
            var result = RunTest( Case( (double d) => d, ValidValue.IsValid ), 
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
                    var result = RunTest( Case( (double value) => value, AnyValue.IsValid ), 
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
                                                   new DiagnosticResultLocation( "Test0.cs", 15, 43 )
                                               }
                               };
            var expectedWrong = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_WrongParameterName",
                                    Message = "Test for 'System.Math.Sqrt(double)' has some invalid parameter 'value'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 15, 49 )
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
                [Test]
                public void MyTest1()
                {
                    var result = RunTest( Case( (int d) => d, AnyValue.IsValid ), 
                                          () => Math.Sqrt(4) );

                    Assert.That( result, Is.EqualTo(2) );
                }
            }
        }";

            var expectedWrongType = new DiagnosticResult
                                    {
                                        Id = "SmartTestsAnalyzer_WrongParameterType",
                                        Message = "Test for 'System.Math.Sqrt(double)' has some invalid parameter type 'int' for parameter 'd'.",
                                        Severity = DiagnosticSeverity.Error,
                                        Locations = new[]
                                                    {
                                                        new DiagnosticResultLocation( "Test0.cs", 15, 49 )
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
                        [Test]
                        public void MyTest()
                        {
                            var reminder = default(int);
                            var result = RunTest( Case( (int a) => a, AnyValue.IsValid ),
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
                                                          new DiagnosticResultLocation( "Test0.cs", 16, 51 )
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
                        [Test]
                        public void MyTest1()
                        {
                            var reminder = default(int);
                            var result = RunTest( Case( (int a) => a, AnyValue.IsValid ) &
                                                  Case( (int b) => b, ValidValue.IsValid ),
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
                            var result = RunTest( Case( (int a) => a, AnyValue.IsValid ) &
                                                  Case( (int b) => b, ValidValue.IsValid ),
                                                  () => Math.DivRem( 7, 3, out reminder ) );

                            Assert.That( result, Is.EqualTo( 2 ) );
                            Assert.That( reminder, Is.EqualTo( 1 ) );
                        }

                        [Test]
                        public void Mytest2()
                        {
                            int reminder;
                            Assert.Throws<DivideByZeroException>( () => RunTest( Case( (int a) => a, AnyValue.IsValid ) &
                                                                                 Case( (int b) => b, ValidValue.IsInvalid ),
                                                                                 () => Math.DivRem( 7, 0, out reminder ) ) );
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
                            RunTest( Case( (int value) => value, AnyValue.IsValid ),
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
                                               new DiagnosticResultLocation( "Test0.cs", 17, 44 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}