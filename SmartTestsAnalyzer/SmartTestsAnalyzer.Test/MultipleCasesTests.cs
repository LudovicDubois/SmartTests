using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

using NUnit.Framework;

using TestHelper;



namespace SmartTestsAnalyzer.Test
{
    [TestFixture]
    public class MultipleCasesTests: CodeFixVerifier
    {
        [Test]
        public void Missing5Cases()
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
        public class MyClass
        {
            public int Add( int i, int j ) 
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add(10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Add(int, int)' has some missing Test Cases: i:MinIncluded.IsBelowMin and j:MinIncluded.IsBelowMin and i:MinIncluded.IsMin & j:MinIncluded.IsMin and i:MinIncluded.IsMin & j:MinIncluded.IsAboveMin and i:MinIncluded.IsAboveMin & j:MinIncluded.IsMin",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Missing4Cases()
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
        public class MyClass
        {
            public int Add( int i, int j ) 
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Add(int, int)' has some missing Test Cases: i:MinIncluded.IsBelowMin and j:MinIncluded.IsBelowMin and i:MinIncluded.IsMin & j:MinIncluded.IsMin and i:MinIncluded.IsAboveMin & j:MinIncluded.IsMin",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 41, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Missing3Cases()
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
        public class MyClass
        {
            public int Add( int i, int j ) 
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }

        [Test]
        public void MyTest3()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 10, 0 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Add(int, int)' has some missing Test Cases: i:MinIncluded.IsBelowMin and j:MinIncluded.IsBelowMin and i:MinIncluded.IsMin & j:MinIncluded.IsMin",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 41, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 53, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Missing2Cases()
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
        public class MyClass
        {
            public int Add( int i, int j ) 
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 20 ) );
        }

        [Test]
        public void MyTest3()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 10, 0 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
        }

        [Test]
        public void MyTest4()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 0, 0 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Add(int, int)' has some missing Test Cases: i:MinIncluded.IsBelowMin and j:MinIncluded.IsBelowMin",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 41, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 53, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 65, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void Missing1Case()
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
        public class MyClass
        {
            public int Add( int i, int j ) 
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 20 ) );
        }

        [Test]
        public void MyTest3()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 10, 0 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
        }

        [Test]
        public void MyTest4()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 0, 0 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }

        [Test]
        public void MyTest5()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsBelowMin ),
                                  () => mc.Add( -1, 0 ) );

            Assert.That( result, Is.EqualTo( -1 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Add(int, int)' has some missing Test Cases: j:MinIncluded.IsBelowMin",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 29, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 41, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 53, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 65, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 77, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void MissingNoCase()
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
        public class MyClass
        {
            public int Add( int i, int j ) 
            {
                if( i < 0 )
                    throw new ArgumentOutOfRangeException( nameof(i) );
                if( j < 0 )
                    throw new ArgumentOutOfRangeException( nameof(j) );
                return i + j;
            }
        }

        [Test]
        public void MyTest()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 20 ) );
        }

        [Test]
        public void MyTest3()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsAboveMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 10, 0 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
        }

        [Test]
        public void MyTest4()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsMin ) &
                                  Case( ""j"", MinIncluded.IsMin ),
                                  () => mc.Add( 0, 0 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }

        [Test]
        public void MyTest5()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""i"", MinIncluded.IsBelowMin ),
                                  () => mc.Add( -1, 0 ) );

            Assert.That( result, Is.EqualTo( -1 ) );
        }

        [Test]
        public void MyTest6()
        {
            var mc = new MyClass();

            var result = RunTest( Case( ""j"", MinIncluded.IsBelowMin ),
                                  () => mc.Add( 0, -1 ) );

            Assert.That( result, Is.EqualTo( -1 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


/*
        [Test]
        public void AndMissing3OppositesCases()
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

            var result = RunTest( NotifyPropertyChanged.HasNoSubscriber & ValidValue.IsValid, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsInvalid and NotifyPropertyChanged.HasSubscriberSameValue & ValidValue.IsValid and NotifyPropertyChanged.HasSubscriberOtherValue & ValidValue.IsValid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndMissing2Cases()
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

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasNoSubscriber, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberSameValue, 
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue and ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 40, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndMissing1Case()
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

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasNoSubscriber, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberSameValue, 
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }

        [Test]
        public void MyTest3()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 40, 35 ),
                                               new DiagnosticResultLocation( "Test0.cs", 53, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndMissingNoCase()
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

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasNoSubscriber, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }

        [Test]
        public void MyTest2()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberSameValue, 
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }

        [Test]
        public void MyTest3()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }

        [Test]
        public void MyTest4()
        {
            var mc = new MyClass( 10 );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );

            RunTest( ValidValue.IsInvalid, 
                     Assign( () => mc.Property, -1 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void OrMissing1Case()
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

            var result = RunTest( NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue, 
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: NotifyPropertyChanged.HasSubscriberOtherValue",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 ),
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void OrMissingNoCase()
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

            var result = RunTest( NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue | NotifyPropertyChanged.HasSubscriberOtherValue, 
                                  Assign( () => mc.Property, 10 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
            Assert.That( mc.Property, Is.EqualTo( 10 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void AndOrMissing2Cases()
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

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasNoSubscriber | ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberSameValue, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue and ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndOrMissing1Case()
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

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasNoSubscriber | ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberSameValue | ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndOrMissingNoCase()
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

            var result = RunTest( ValidValue.IsValid & NotifyPropertyChanged.HasNoSubscriber | ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberSameValue | ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue | ValidValue.IsInvalid, 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }


        [Test]
        public void AndOrParenthesisMissing2Cases()
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

            var result = RunTest( ValidValue.IsValid & ( NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue ), 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsValid & NotifyPropertyChanged.HasSubscriberOtherValue and ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndOrParenthesisMissing1Case()
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

            var result = RunTest( ValidValue.IsValid & ( NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue | NotifyPropertyChanged.HasSubscriberOtherValue ), 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            var expected = new DiagnosticResult
                           {
                               Id = "SmartTestsAnalyzer_MissingCases",
                               Message = "Tests for 'TestingProject.MyTestClass.MyClass.Property [set]' has some missing Test Cases: ValidValue.IsInvalid",
                               Severity = DiagnosticSeverity.Warning,
                               Locations = new[]
                                           {
                                               new DiagnosticResultLocation( "Test0.cs", 27, 35 )
                                           }
                           };

            VerifyCSharpDiagnostic( test, expected );
        }


        [Test]
        public void AndOrParenthesisMissingNoCase()
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

            var result = RunTest( ( ValidValue.IsValid | ValidValue.IsInvalid ) & ( NotifyPropertyChanged.HasNoSubscriber | NotifyPropertyChanged.HasSubscriberSameValue | NotifyPropertyChanged.HasSubscriberOtherValue ), 
                                  Assign( () => mc.Property, 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc.Property, Is.EqualTo( 11 ) );
        }
    }
}";

            VerifyCSharpDiagnostic( test );
        }
*/

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new SmartTestsAnalyzerCodeFixProvider();


        protected override SmartTestsAnalyzerAnalyzer GetCSharpDiagnosticAnalyzer() => new SmartTestsAnalyzerAnalyzer();
    }
}