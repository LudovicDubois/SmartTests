using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject.MultipleCasesTests
{
    [TestFixture]
    public class MissingNoCase
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

            var result = RunTest( Case( "i", MinIncluded.IsAboveMin ) &
                                  Case( "j", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 10, 20 ) );

            Assert.That( result, Is.EqualTo( 30 ) );
        }


        [Test]
        public void MyTest2()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsMin ) &
                                  Case( "j", MinIncluded.IsAboveMin ),
                                  () => mc.Add( 0, 20 ) );

            Assert.That( result, Is.EqualTo( 20 ) );
        }


        [Test]
        public void MyTest3()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsAboveMin ) &
                                  Case( "j", MinIncluded.IsMin ),
                                  () => mc.Add( 10, 0 ) );

            Assert.That( result, Is.EqualTo( 10 ) );
        }


        [Test]
        public void MyTest4()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsMin ) &
                                  Case( "j", MinIncluded.IsMin ),
                                  () => mc.Add( 0, 0 ) );

            Assert.That( result, Is.EqualTo( 0 ) );
        }


        [Test]
        public void MyTest5()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "i", MinIncluded.IsBelowMin ),
                                  () => mc.Add( -1, 0 ) );

            Assert.That( result, Is.EqualTo( -1 ) );
        }


        [Test]
        public void MyTest6()
        {
            var mc = new MyClass();

            var result = RunTest( Case( "j", MinIncluded.IsBelowMin ),
                                  () => mc.Add( 0, -1 ) );

            Assert.That( result, Is.EqualTo( -1 ) );
        }
    }
}