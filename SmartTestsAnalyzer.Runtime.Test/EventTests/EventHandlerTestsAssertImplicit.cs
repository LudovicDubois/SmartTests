using System;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using SmartTestsAnalyzer.Runtime.Test.Annotations;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.EventTests
{
    [TestFixture]
    public class EventHandlerTestsAssertImplicit
    {
        public class MyClass
        {
            public MyClass( bool raiseEvent )
            {
                _RaiseEvent = raiseEvent;
            }


            private readonly bool _RaiseEvent;


            public void Method()
            {
                if( _RaiseEvent )
                    RaiseMyevent();
            }


            public event EventHandler MyEvent;


            [NotifyPropertyChangedInvocator]
            protected virtual void RaiseMyevent() => MyEvent?.Invoke( this, EventArgs.Empty );
        }


        [Test]
        public void Raised()
        {
            var mc = new MyClass( true );

            var raised = false;
            RunTest( AnyValue.IsValid,
                     () => mc.Method(),
                     SmartAssert.Raised<EventArgs>( "MyEvent", ( sender, args ) => raised = true ) );
            Assert.IsTrue( raised );
        }


        [Test]
        public void NotRaised()
        {
            var raised = false;
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false );

                                                                  RunTest( AnyValue.IsValid,
                                                                           () => mc.Method(),
                                                                           SmartAssert.Raised<EventArgs>( "MyEvent", ( sender, args ) => raised = true ) );
                                                              } );

            Assert.AreEqual( "Event 'EventHandlerTestsAssertImplicit+MyClass.MyEvent' was expected", exception.Message );
            Assert.IsFalse( raised );
        }
    }
}