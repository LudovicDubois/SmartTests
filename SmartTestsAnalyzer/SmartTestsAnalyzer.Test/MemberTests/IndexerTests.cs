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
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
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
        public void GetValid()
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
        public void SetValid()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
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
        public void SetValid()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void SetMissingCase()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
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
        public void SetMissingCase()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( ValidValue.Valid,
                                  Assign( () => mc[0], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.ConstructorTests.MyClass.this[int]' has some missing Test Cases: ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 33, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic(test, expected);
        }


        /*
        [Test]
        public void PropertyGetMissingCase()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }

            public int Property { get; }
        }

        [Test]
        public void Missing1()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( ValidValue.Valid,
                                  () => mc.Property );

            Assert.That( result, Is.EqualTo( 10 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'ConstructorTests.MyClass.Property' has some missing Test Cases: ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void PropertyGetCaseParameter()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
    {
        public class MyClass
        {
            public MyClass( int property )
            {
                Property = property;
            }

            public int Property { get; }
        }

        [Test]
        public void WrongParameter()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( ""value"", ValidValue.Valid ),
                                  () => mc.Property );

            Assert.That( result, Is.EqualTo( 10 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterName",
                               Message = "Test for 'ConstructorTests.MyClass.Property' has some invalid parameter 'value'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void PropertySet()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
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
        public void Valid()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( AnyValue.Valid,
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void PropertySetMissingCase()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
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
        public void Missing1()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.Valid,
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'ConstructorTests.MyClass.Property' has some missing Test Cases: ValidValue.Invalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 28, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void PropertySetCaseParameter()
        {
            var test = @"
using System;
using NUnit.Framework;
using SmartTests.Criterias;
using static SmartTests.SmartTest;

namespace TestingProject
{
    [TestFixture]
    public class ConstructorTests
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
        public void WrongParameter()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( Case( ""value"", ValidValue.Valid ),
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";
            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_WrongParameterName",
                               Message = "Test for 'ConstructorTests.MyClass.Property' has some invalid parameter 'value'.",
                               Severity = DiagnosticSeverity.Error,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 28, 41 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }
        */

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}