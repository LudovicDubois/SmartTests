using System;
using System.ComponentModel;

using JetBrains.Annotations;



namespace SmartTests.Assertions
{
    public static class PropertyChangedAssertions
    {
        public static Assertion Raised_PropertyChanged<T>( this SmartAssert @this, [NotNull] T t )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChanged( t, true );
        }


        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssert @this, [NotNull] T t )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChanged( t, false );
        }


        private class RaisePropertyChanged: Assertion
        {
            public RaisePropertyChanged( INotifyPropertyChanged instance, bool expectedRaised )
            {
                _Instance = instance;
                _ExpectedRaised = expectedRaised;
            }


            private readonly INotifyPropertyChanged _Instance;
            private readonly bool _ExpectedRaised;
            private bool _Raised;


            public override void BeforeAct() => _Instance.PropertyChanged += InstanceOnPropertyChanged;


            public override void AfterAct( Exception e )
            {
                if( _Raised != _ExpectedRaised )
                    throw new SmartTestException( string.Format( _ExpectedRaised
                                                                     ? Resource.ExpectedRaisedEvent
                                                                     : Resource.ExpectedNotRaisedEvent,
                                                                 "PropertyChanged"
                                                               )
                                                );

                _Instance.PropertyChanged -= InstanceOnPropertyChanged;
            }


            private void InstanceOnPropertyChanged( object sender, PropertyChangedEventArgs propertyChangedEventArgs ) => _Raised = true;
        }
    }
}