using System.ComponentModel;
using System.Runtime.CompilerServices;

using NUnit.Framework;

using SmartTests;
using SmartTests.Assertions;
using SmartTests.Criterias;

using SmartTestsAnalyzer.Runtime.Test.Annotations;

using static SmartTests.SmartTest;



namespace SmartTestsAnalyzer.Runtime.Test.NotifyPropertyChangedTests
{
    [TestFixture]
    public class NotifyPropertyChangedTests
    {
        public class MyClass: INotifyPropertyChanged
        {
            public MyClass( bool raiseEvent )
            {
                _RaiseEvent = raiseEvent;
            }


            private readonly bool _RaiseEvent;


            private int _MyProperty;
            public int MyProperty
            {
                get { return _MyProperty; }
                set
                {
                    _MyProperty = value;
                    if( _RaiseEvent )
                        RaisePropertyChanged();
                }
            }

            public int ReadonlyProperty => 10;


            public int Method() => 10;


            public event PropertyChangedEventHandler PropertyChanged;


            [NotifyPropertyChangedInvocator]
            protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = null )
            {
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }


        [Test]
        public void HasSubscriberOtherValue()
        {
            var mc = new MyClass( true );
            Assert.AreNotEqual( 10, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                     Assign( () => mc.MyProperty, 10 ),
                     SmartTest.SmartAssert.Raised_PropertyChanged( mc ) );

            Assert.AreEqual( 10, mc.MyProperty );
        }


        [Test]
        public void HasSubscriberOtherValue_NoPropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( false );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartTest.SmartAssert.Raised_PropertyChanged( mc ) );
                                                              }
                                                            );
            Assert.AreEqual( "Event PropertyChanged was expected", exception.Message );
        }


        [Test]
        public void HasSubscriberSameValue()
        {
            var mc = new MyClass( false );
            Assert.AreEqual( 0, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberSameValue,
                     Assign( () => mc.MyProperty, 0 ),
                     SmartTest.SmartAssert.NotRaised_PropertyChanged( mc ) );

            Assert.AreEqual( 0, mc.MyProperty );
        }


        [Test]
        public void HasSubscriberSameValue_PropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true );
                                                                  Assert.AreEqual( 0, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberSameValue,
                                                                           Assign( () => mc.MyProperty, 0 ),
                                                                           SmartTest.SmartAssert.NotRaised_PropertyChanged( mc ) );
                                                              } );

            Assert.AreEqual( "Event PropertyChanged was unexpected", exception.Message );
        }


        [Test]
        public void HasNoSubscriber()
        {
            var mc = new MyClass( false );
            Assert.AreNotEqual( 20, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasNoSubscriber,
                     Assign( () => mc.MyProperty, 20 ) );

            Assert.AreEqual( 20, mc.MyProperty );
        }


        [Test]
        public void HasNoSubscriber_PropertyChanged()
        {
            var mc = new MyClass( true );
            Assert.AreNotEqual( 20, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasNoSubscriber,
                     Assign( () => mc.MyProperty, 20 ) );
        }


        [Test]
        public void ReadonlyProperty()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true );
                                                                  Assert.AreNotEqual( 20, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasNoSubscriber,
                                                                           Assign( () => mc.Method(), 20 ) );
                                                              } );

            Assert.AreEqual( "BAD TEST: 'SmartTestsAnalyzer.Runtime.Test.NotifyPropertyChangedTests.NotifyPropertyChangedTests+MyClass.Method' is not a writable property", exception.Message );
        }


        [Test]
        public void NotAProperty()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( true );
                                                                  Assert.AreNotEqual( 20, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasNoSubscriber,
                                                                           Assign( () => mc.Method(), 20 ) );
                                                              } );

            Assert.AreEqual( "BAD TEST: 'SmartTestsAnalyzer.Runtime.Test.NotifyPropertyChangedTests.NotifyPropertyChangedTests+MyClass.Method' is not a writable property", exception.Message );
        }
    }
}