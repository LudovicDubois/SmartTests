using System.Collections.Generic;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests
{
    [TestFixture]
    public class IndexerTests
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
        public void GetValid()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  () => mc[ 0 ] );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        [Test]
        public void SetValid()
        {
            var mc = new MyClass( 10 );

            var result = RunTest( AnyValue.Valid,
                                  Assign( () => mc[ 0 ], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc[ 0 ], Is.EqualTo( 11 ) );
        }


        public class MyClass2
        {
            public MyClass2( int value )
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
        public void SetMissingCase()
        {
            var mc = new MyClass2( 10 );

            var result = RunTest( ValidValue.Valid,
                                  Assign( () => mc[ 0 ], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
            Assert.That( mc[ 0 ], Is.EqualTo( 11 ) );
        }


        [Test]
        public void SetCaseParameter()
        {
            var mc = new MyClass2( 10 );

            var result = RunTest( Case( "value", ValidValue.Valid ),
                                  Assign( () => mc[ 0 ], 11 ) );

            Assert.That( result, Is.EqualTo( 11 ) );
        }
    }
}