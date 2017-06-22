using System.Collections.Generic;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MemberTests.IndexerTests
{
    [TestFixture]
    public class GetValidTest
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

            var result = RunTest( AnyValue.IsValid,
                                  () => mc[ 0 ] );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        // No Error
    }
}