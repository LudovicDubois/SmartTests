using System;

using NUnit.Framework;

using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.ThrowTests
{
    [TestFixture]
    public class ThrowTests
    {
        static class MyClass
        {
            public static void Method( string p )
            {
                if( p == null )
                    throw new ArgumentNullException( nameof(p), "p should not be null!" );
                if( string.IsNullOrEmpty( p ) )
                    throw new ArgumentOutOfRangeException( nameof(p) );
                if( p == "throw" )
                    throw new ArithmeticException( "Wrong Computation" );
                Console.WriteLine( p );
            }
        }


        #region Argument Exception Tests

        [Test]
        public void Argument_HappyPath()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.Method( null ),
                     SmartAssert.Throw<ArgumentNullException>( "p" ) );
        }


        [Test]
        public void Argument_HappyPath_Message()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.Method( null ),
                     SmartAssert.Throw<ArgumentNullException>( "p", "p should not be null!" ) );
        }


        [Test]
        public void Argument_HappyPath_Message_Other()
        {
            var run = false;

            RunTest( AnyValue.IsValid,
                     () => MyClass.Method( null ),
                     SmartAssert.Throw<ArgumentNullException>( "p",
                                                               "p should not be null!",
                                                               e =>
                                                               {
                                                                   run = true;
                                                                   Assert.IsTrue( e.Message.StartsWith( "p should not be null!" ) );
                                                               } ) );
            Assert.IsTrue( run );
        }


        [Test]
        public void Argument_NoThrow()
        {
            var exception = Assert.Catch( () => RunTest( AnyValue.IsValid,
                                                         () => MyClass.Method( "PASS!" ),
                                                         SmartAssert.Throw<ArgumentNullException>( "pp" ) )
                                        );

            Assert.AreEqual( "Exception 'System.ArgumentNullException' was expected", exception.Message );
        }


        [Test]
        public void Argument_WrongThrow()
        {
            var exception = Assert.Catch( () => RunTest( AnyValue.IsValid,
                                                         () => MyClass.Method( "" ),
                                                         SmartAssert.Throw<ArgumentNullException>( "pp" ) )
                                        );

            Assert.AreEqual( "Exception 'System.ArgumentNullException' was expected, but was 'System.ArgumentOutOfRangeException'", exception.Message );
        }


        [Test]
        public void Argument_WrongName()
        {
            var exception = Assert.Catch( () => RunTest( AnyValue.IsValid,
                                                         () => MyClass.Method( null ),
                                                         SmartAssert.Throw<ArgumentNullException>( "pp" ) )
                                        );

            Assert.AreEqual( "Exception's ParamName should be 'pp', but was 'p'", exception.Message );
        }


        [Test]
        public void Argument_WrongMessage()
        {
            var exception = Assert.Catch( () =>
                                              RunTest( AnyValue.IsValid,
                                                       () => MyClass.Method( null ),
                                                       SmartAssert.Throw<ArgumentNullException>( "p", "p should not be null!!" ) )
                                        );

            Assert.AreEqual( "Exception's Message should be 'p should not be null!!', but was 'p should not be null!'", exception.Message );
        }

        #endregion


        #region Other Exceptions

        [Test]
        public void HappyPath()
        {
            RunTest( AnyValue.IsValid,
                     () => MyClass.Method( "throw" ),
                     SmartAssert.Throw<ArithmeticException>() );
        }


        [Test]
        public void HappyPath_Message()
        {
            var run = false;
            RunTest( AnyValue.IsValid,
                     () => MyClass.Method( "throw" ),
                     SmartAssert.Throw<ArithmeticException>( e =>
                                                             {
                                                                 run = true;
                                                                 Assert.AreEqual( "Wrong Computation", e.Message );
                                                             } ) );
            Assert.IsTrue( run );
        }


        [Test]
        public void HappyPath_NoException()
        {
            var exception = Assert.Catch( () =>
                                              RunTest( AnyValue.IsValid,
                                                       () => MyClass.Method( "pass" ),
                                                       SmartAssert.Throw<ArithmeticException>() )
                                        );

            Assert.AreEqual( "Exception 'System.ArithmeticException' was expected", exception.Message );
        }


        [Test]
        public void HappyPath_WrongException()
        {
            var exception = Assert.Catch( () =>
                                              RunTest( AnyValue.IsValid,
                                                       () => MyClass.Method( null ),
                                                       SmartAssert.Throw<ArithmeticException>() )
                                        );

            Assert.AreEqual( "Exception 'System.ArithmeticException' was expected, but was 'System.ArgumentNullException'", exception.Message );
        }

        #endregion
    }
}