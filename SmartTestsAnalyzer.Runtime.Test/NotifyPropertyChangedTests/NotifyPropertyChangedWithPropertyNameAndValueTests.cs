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
    public class NotifyPropertyChangedWithPropertyNameAndValueTests
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
                     SmartAssert.Raised_PropertyChanged( mc, nameof(MyClass.MyProperty), 10 ) );
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
                                                                           SmartAssert.Raised_PropertyChanged( mc, "MyProperty", 10 ) );
                                                              }
                                                            );

            Assert.AreEqual( "Event 'PropertyChanged' was expected", exception.Message );
        }


        [Test]
        public void HasSubscriberOtherValue_BadPropertyChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( "OtherProperty" );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartAssert.Raised_PropertyChanged( mc, "MyProperty", 10 ) );
                                                              }
                                                            );

            Assert.AreEqual( "Unexpected property name 'OtherProperty' when PropertyChanged event was raised", exception.Message );
        }


        [Test]
        public void HasSubscriberOtherValue_BadValueChanged()
        {
            var exception = Assert.Catch<SmartTestException>( () =>
                                                              {
                                                                  var mc = new MyClass( "MyProperty" );
                                                                  Assert.AreNotEqual( 10, mc.MyProperty );

                                                                  RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                                                                           Assign( () => mc.MyProperty, 10 ),
                                                                           SmartAssert.Raised_PropertyChanged( mc, "MyProperty", 11 ) );
                                                              }
                                                            );

            Assert.AreEqual( "Change is wrong. Expected 11, but was 10", exception.Message );
        }
    }
}