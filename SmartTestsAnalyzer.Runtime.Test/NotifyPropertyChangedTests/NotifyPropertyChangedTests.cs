using System.ComponentModel;
using System.Runtime.CompilerServices;

using NUnit.Framework;

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
            private int _MyProperty;
            public int MyProperty
            {
                get { return _MyProperty; }
                set
                {
                    if( value == _MyProperty )
                        return;
                    _MyProperty = value;
                    RaisePropertyChanged();
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
            var mc = new MyClass();
            Assert.AreNotEqual( 10, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberOtherValue,
                     Assign( () => mc.MyProperty, 10 ),
                     SmartAssert.Raised_PropertyChanged( mc ) );

            Assert.AreEqual( 10, mc.MyProperty );
        }


        [Test]
        public void HasSubscriberSameValue()
        {
            var mc = new MyClass();
            Assert.AreEqual( 0, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasSubscriberSameValue,
                     Assign( () => mc.MyProperty, 0 ),
                     SmartAssert.NotRaised_PropertyChanged( mc ) );

            Assert.AreEqual( 0, mc.MyProperty );
        }


        [Test]
        public void HasNoSubscriber()
        {
            var mc = new MyClass();
            Assert.AreNotEqual( 20, mc.MyProperty );

            RunTest( NotifyPropertyChanged.HasNoSubscriber,
                     Assign( () => mc.MyProperty, 20 ) );

            Assert.AreEqual( 20, mc.MyProperty );
        }
    }
}