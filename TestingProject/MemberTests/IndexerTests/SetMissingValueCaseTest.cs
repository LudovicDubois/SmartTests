﻿using System.Collections.Generic;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.IndexerTests
{
    [TestFixture]
    public class SetMissingValueCaseTest
    {
        public class MyClass
        {
            public MyClass( int value )
            {
                _List.Add( value );
            }


            private readonly List<int> _List = new List<int>();

            public int this[ int index ]
            {
                get { return _List[ index ]; }
                set { _List[ index ] = value; }
            }
        }


        [Test]
        public void MyTest()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( Case( "index", AnyValue.IsValid ) &
                                  Case( "value", ValidValue.IsValid ),
                                  Assign( () => mc[ 0 ], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }


        /*
        var expected = new DiagnosticResult
                        {
                            Id = "SmartTestsAnalyzer_MissingParameterCases",
                            Message = "Tests for 'TestingProject.IndexerTests.MyClass.this[int] [set]' has some missing Test Cases for parameter 'value': ValidValue.IsInvalid",
                            Severity = DiagnosticSeverity.Warning,
                            Locations = new[]
                                        {
                                            new DiagnosticResultLocation( "Test0.cs", 34, 35 )
                                        }
                        };
        */
    }
}