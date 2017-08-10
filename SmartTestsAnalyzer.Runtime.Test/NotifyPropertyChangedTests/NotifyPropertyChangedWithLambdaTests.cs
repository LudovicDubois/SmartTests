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
    public class NotifyPropertyChangedWithLambdaTests
    {
        public class MyClass: INotifyPropertyChanged
        {
            public MyClass( params string[] propertyNames )
            {
                _RaiseEventPropertyNames = propertyNames;
            }


            private readonly string[] _RaiseEventPropertyNames;


            private int _MyProperty;
            public int MyProperty
            {
                get { return _MyProperty; }
                set
                {
                    _MyProperty = value;
                    if( _RaiseEventPropertyNames != null )
                        foreach( var propertyName in _RaiseEventPropertyNames )
                            RaisePropertyChanged( propertyName );
                }
            }


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
            var mc = new MyClass( "MyProperty" );
            Assert.AreNotEqual( 10, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                     Assign( () => mc.MyProperty, 10 ),
                     SmartAssert.Raised_PropertyChanged( () => mc.MyProperty ) );

            Assert.AreEqual( 10, mc.MyProperty );
        }


        [Test]
        public void HasSubscriberOtherValue_NoPropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( null );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartAssert.Raised_PropertyChanged( () => mc.MyProperty ) );
                                                              }
                                                            );

            Assert.AreEqual( "Event 'PropertyChanged' was expected", exception.Message );
        }


        [Test]
        public void HasSubscriberOtherValue_1BadPropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( "OtherProperty" );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartAssert.Raised_PropertyChanged( () => mc.MyProperty ) );
                                                              }
                                                            );

            Assert.AreEqual( "Unexpected property name 'OtherProperty' when PropertyChanged event was raised", exception.Message );
        }


        [Test]
        public void HasSubscriberOtherValue_2BadPropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( "OtherProperty", "NextProperty" );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartAssert.Raised_PropertyChanged( () => mc.MyProperty ) );
                                                              }
                                                            );

            Assert.AreEqual( "Unexpected property name 'OtherProperty' when PropertyChanged event was raised", exception.Message );
        }


        [Test]
        public void HasSubscriberOtherValue_TwicePropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( "MyProperty", "MyProperty" );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartAssert.Raised_PropertyChanged( mc, "MyProperty" ) );
                                                              }
                                                            );

            Assert.AreEqual( "Unexpected property name 'MyProperty' when PropertyChanged event was raised", exception.Message );
        }


        [Test]
        public void HasSubscriberSameValue()
        {
            var mc = new MyClass( null );
            Assert.AreEqual( 0, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberSameValue,
                     Assign( () => mc.MyProperty, 0 ),
                     SmartAssert.NotRaised_PropertyChanged( () => mc.MyProperty ) );

            Assert.AreEqual( 0, mc.MyProperty );
        }


        [Test]
        public void HasSubscriberSameValue_RaiseOtherProperty()
        {
            var mc = new MyClass( "OtherProperty" );
            Assert.AreEqual( 0, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberSameValue,
                     Assign( () => mc.MyProperty, 0 ),
                     SmartAssert.NotRaised_PropertyChanged( () => mc.MyProperty ) );

            Assert.AreEqual( 0, mc.MyProperty );
        }
    }
}