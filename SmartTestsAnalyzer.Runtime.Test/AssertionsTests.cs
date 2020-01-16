using System;

using NUnit.Framework;

using SmartTests;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test
{
    [TestFixture]
    public class AssertionsTests
    {
        public class AssertionTest: Assertion
        {
            public AssertionTest( bool beforeFail, bool afterFail )
            {
                _BeforeFail = beforeFail;
                _AfterFail = afterFail;
            }


            public int Before { get; private set; }
            public int After { get; private set; }

            private readonly bool _BeforeFail;
            private readonly bool _AfterFail;


            public override void BeforeAct( ActBase act )
            {
                if( _BeforeFail )
                    throw new NotImplementedException();
                Before++;
            }


            public override void AfterAct( ActBase act )
            {
                if( _AfterFail )
                    throw new NotImplementedException();
                After++;
            }
        }


        [Test]
        public void THappyPath()
        {
            var assert1 = new AssertionTest( false, false );
            var assert2 = new AssertionTest( false, false );

            RunTest( AnyValue.IsValid,
                     () => Math.Sqrt( 0 ),
                     assert1,
                     assert2 );

            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 1, assert1.Before );
            Assert.AreEqual( 1, assert1.After );
            Assert.AreEqual( 1, assert2.After );
        }


        [Test]
        public void TBefore2Fail()
        {
            var assert1 = new AssertionTest( false, false );
            var assert2 = new AssertionTest( true, false );

            Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                              () => Math.Sqrt( 0 ),
                                                              assert1,
                                                              assert2 ) );

            Assert.AreEqual( 0, assert2.Before );
            Assert.AreEqual( 0, assert1.Before );
            Assert.AreEqual( 0, assert1.After );
            Assert.AreEqual( 0, assert2.After );
        }


        [Test]
        public void TBefore1Fail()
        {
            var assert1 = new AssertionTest( true, false );
            var assert2 = new AssertionTest( false, false );

            Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                              () => Math.Sqrt( 0 ),
                                                              assert1,
                                                              assert2 ) );

            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 0, assert1.Before );
            Assert.AreEqual( 0, assert1.After );
            Assert.AreEqual( 1, assert2.After );
        }


        [Test]
        public void TAfter1Fail()
        {
            var assert1 = new AssertionTest( false, true );
            var assert2 = new AssertionTest( false, false );

            var exception = Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                                              () => Math.Sqrt( 0 ),
                                                                              assert1,
                                                                              assert2 ) );

            Assert.AreEqual( typeof(NotImplementedException), exception.InnerException.GetType() );
            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 1, assert1.Before );
            Assert.AreEqual( 0, assert1.After );
            Assert.AreEqual( 1, assert2.After );
        }


        [Test]
        public void TAfter2Fail()
        {
            var assert1 = new AssertionTest( false, false );
            var assert2 = new AssertionTest( false, true );

            var exception = Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                                              () => Math.Sqrt( 0 ),
                                                                              assert1,
                                                                              assert2 ) );

            Assert.AreEqual( typeof(NotImplementedException), exception.InnerException.GetType() );
            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 1, assert1.Before );
            Assert.AreEqual( 1, assert1.After );
            Assert.AreEqual( 0, assert2.After );
        }


        public void DoNothing()
        { }


        [Test]
        public void HappyPath()
        {
            var assert1 = new AssertionTest( false, false );
            var assert2 = new AssertionTest( false, false );

            RunTest( AnyValue.IsValid,
                     () => DoNothing(),
                     assert1,
                     assert2 );

            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 1, assert1.Before );
            Assert.AreEqual( 1, assert1.After );
            Assert.AreEqual( 1, assert2.After );
        }


        [Test]
        public void Before2Fail()
        {
            var assert1 = new AssertionTest( false, false );
            var assert2 = new AssertionTest( true, false );

            Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                              () => DoNothing(),
                                                              assert1,
                                                              assert2 ) );

            Assert.AreEqual( 0, assert2.Before );
            Assert.AreEqual( 0, assert1.Before );
            Assert.AreEqual( 0, assert1.After );
            Assert.AreEqual( 0, assert2.After );
        }


        [Test]
        public void Before1Fail()
        {
            var assert1 = new AssertionTest( true, false );
            var assert2 = new AssertionTest( false, false );

            Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                              () => DoNothing(),
                                                              assert1,
                                                              assert2 ) );

            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 0, assert1.Before );
            Assert.AreEqual( 0, assert1.After );
            Assert.AreEqual( 1, assert2.After );
        }


        [Test]
        public void After1Fail()
        {
            var assert1 = new AssertionTest( false, true );
            var assert2 = new AssertionTest( false, false );

            var exception = Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                                              () => DoNothing(),
                                                                              assert1,
                                                                              assert2 ) );

            Assert.AreEqual( typeof(NotImplementedException), exception.InnerException.GetType() );
            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 1, assert1.Before );
            Assert.AreEqual( 0, assert1.After );
            Assert.AreEqual( 1, assert2.After );
        }


        [Test]
        public void After2Fail()
        {
            var assert1 = new AssertionTest( false, false );
            var assert2 = new AssertionTest( false, true );

            var exception = Assert.Throws<SmartTestException>( () => RunTest( AnyValue.IsValid,
                                                                              () => DoNothing(),
                                                                              assert1,
                                                                              assert2 ) );

            Assert.AreEqual( typeof(NotImplementedException), exception.InnerException.GetType() );
            Assert.AreEqual( 1, assert2.Before );
            Assert.AreEqual( 1, assert1.Before );
            Assert.AreEqual( 1, assert1.After );
            Assert.AreEqual( 0, assert2.After );
        }
    }
}