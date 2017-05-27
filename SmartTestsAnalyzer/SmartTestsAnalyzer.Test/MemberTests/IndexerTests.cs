using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test.MemberTests
{
    [TestFixture]
    public class IndexerTests: CodeFixVerifier
    {
        [Test]
        public void GetValid()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }

     
        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  () => mc[ 0 ] );

            Assert.That( result, Is.EqualTo( 10 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void GetMissingCase()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( ValidValue.Valid,
                                  () => mc[ 0 ] );

            Assert.That( result, Is.EqualTo( 10 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.IndexerTests.MyClass.this[int] [get]' has some missing Test Cases: ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 32, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void GetWrongCaseParameter()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }

     
        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""value"", AnyValue.Valid ),
                                  () => mc[ 0 ] );

            Assert.That( result, Is.EqualTo( 10 ) );
        }
    }
}";

            var expectedCase = new DiagnosticResult
                               {
                                   Id = "SmartTestsAnalyzer_MissingParameterCase",
                                   Message = "Test for 'TestingProject.IndexerTests.MyClass.this[int] [get]' has no Case for parameter 'index'.",
                                   Severity = DiagnosticSeverity.Error,
                                   Locations = new[]
                                               {
                                                   new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                               }
                               };
            var expectedWrong = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_WrongParameterName",
                                    Message = "Test for 'TestingProject.IndexerTests.MyClass.this[int] [get]' has some invalid parameter 'value'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 33, 41 )
                                                }
                                };

            VerifyCSharpDiagnostic( test, expectedCase, expectedWrong );
        }


        [Test]
        [Ignore( "Multiple parameters not treated yet" )]
        public void SetValid()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""index"", AnyValue.Valid ) &
                                  Case( ""value"", AnyValue.Valid ),
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test, 0 );
        }


        [Test]
        public void SetIndexParameterName()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""index"", AnyValue.Valid ),
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";
            var expectedValue = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_MissingParameterCase",
                                    Message = "Test for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has no Case for parameter 'value'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                                }
                                };

            VerifyCSharpDiagnostic( test, expectedValue );
        }


        [Test]
        public void SetValueParameterName()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""value"", AnyValue.Valid ),
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingParameterCase",
                               Message = "Test for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has no Case for parameter 'index'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void SetNoParameterName()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            var expectedIndex = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_MissingParameterCase",
                                    Message = "Test for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has no Case for parameter 'index'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                                }
                                };
            var expectedValue = new DiagnosticResult
                                {
                                    Id = "SmartTestsAnalyzer_MissingParameterCase",
                                    Message = "Test for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has no Case for parameter 'value'.",
                                    Severity = DiagnosticSeverity.Error,
                                    Locations = new[]
                                                {
                                                    new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                                }
                                };

            VerifyCSharpDiagnostic( test, expectedIndex, expectedValue );
        }


        [Test]
        [Ignore( "Code doesn't match Name!" )]
        public void SetMissingIndexCase()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""index"", ValidValue.Valid ) &
                                  Case( ""value"", AnyValue.Valid ),
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has some missing Test Cases: index:ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void SetMissingValueCase()
        {
            var test = @"
using System.Collections.Generic;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class IndexerTests
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add(value);
            }

            private readonly List<int> _List = new List<int>();

            public int this[ int index ] {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""index"", AnyValue.Valid ) &
                                  Case( ""value"", ValidValue.Valid ),
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has some missing Test Cases: value:ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}