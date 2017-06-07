using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using JetBrains.Annotations;

// ReSharper disable UnusedParameter.Global


namespace SmartTests.Assertions
{
    public static class PropertyChangedAssertions
    {
        public static Assertion Raised_PropertyChanged( this SmartAssert @this ) => new RaisePropertyChanged( null, true, null );


        public static Assertion Raised_PropertyChanged<T>( this SmartAssert @this, [NotNull] T t, params string[] expectedPropertyNames )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChanged( t, true, expectedPropertyNames );
        }


        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssert @this, [NotNull] T t )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChanged( t, false, null );
        }


        private class RaisePropertyChanged: Assertion
        {
            public RaisePropertyChanged( INotifyPropertyChanged instance, bool expectedRaised, string[] propertyNames )
            {
                _Instance = instance;
                _ExpectedRaised = expectedRaised;
                _PropertyNames = propertyNames?.ToList();
                _PropertyNameVerifications = _PropertyNames?.Count > 0;
            }


            private INotifyPropertyChanged _Instance;
            private readonly bool _ExpectedRaised;
            private List<string> _PropertyNames;
            private bool _PropertyNameVerifications;
            private bool _Raised;


            public override void BeforeAct( ActBase act )
            {
                var implicitSource = _Instance == null && _PropertyNames == null;

                if( _Instance == null )
                    _Instance = act.Instance as INotifyPropertyChanged;
                if( _Instance == null )
                    throw new SmartTestException( string.Format( Resource.BadTest_NotINotifyPropertyChanged, act.Instance?.GetType().FullName ) );

                if( _PropertyNames == null )
                {
                    if( act.Property == null )
                        throw new SmartTestException( string.Format( Resource.BadTest_NotProperty, act.Method.DeclaringType?.FullName + "." + act.Method.Name ) );
                    _PropertyNames = new List<string>
                                     {
                                         act.Property.Name
                                     };
                }

                if( implicitSource )
                    _PropertyNameVerifications = true;

                _Instance.PropertyChanged += InstanceOnPropertyChanged;
            }


            public override void AfterAct( ActBase act )
            {
                if( _Raised != _ExpectedRaised )
                    throw new SmartTestException( string.Format( _ExpectedRaised
                                                                     ? Resource.ExpectedRaisedEvent
                                                                     : Resource.ExpectedNotRaisedEvent,
                                                                 "PropertyChanged"
                                                               )
                                                );

                if( _Instance != null )
                    _Instance.PropertyChanged -= InstanceOnPropertyChanged;
            }


            private void InstanceOnPropertyChanged( object sender, PropertyChangedEventArgs args )
            {
                _Raised = true;

                if( !_PropertyNameVerifications )
                    // No property name expected
                    return;

                // Ensure this property changed is expected
                if( !_PropertyNames.Remove( args.PropertyName ) )
                    throw new SmartTestException( string.Format( Resource.UnexpectedPropertyNameWhenPropertyChangedRaised, args.PropertyName ) );
            }
        }
    }
}