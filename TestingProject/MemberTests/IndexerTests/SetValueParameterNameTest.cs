﻿using System.Collections.Generic;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.IndexerTests
{
    [TestFixture]
    public class SetValueParameterNameTest
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

            var result = RunTest( Case( "value", AnyValue.IsValid ),
                                  Assign( () => mc[ 0 ], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }


        /*
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
        */
    }
}