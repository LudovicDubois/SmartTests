using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

using SmartTests.Acts;
using SmartTests.Helpers;

// ReSharper disable UnusedParameter.Global


namespace SmartTests.Assertions
{
    public static class PropertyChangedAssertions
    {
        public static Assertion Raised_PropertyChanged( this SmartAssertPlaceHolder @this ) => new RaisePropertyChangedAssertion( null, true, null, null );


        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, [NotNull] T t, params string[] expectedPropertyNames )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChangedAssertion( t, true, expectedPropertyNames );
        }


        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, [NotNull] T t, [NotNull] string expectedPropertyName, object value )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            if( expectedPropertyName == null )
                throw new ArgumentNullException( nameof(expectedPropertyName) );
            return new RaisePropertyChangedAssertion( t, true, expectedPropertyName, value );
        }


        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> expression )
        {
            INotifyPropertyChanged sender;
            PropertyInfo property;
            GetInstanceAndProperty( expression, out sender, out property );

            return new RaisePropertyChangedAssertion( sender, true, new[] { property.Name } );
        }


        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> expression, T value )
        {
            INotifyPropertyChanged sender;
            PropertyInfo property;
            GetInstanceAndProperty( expression, out sender, out property );

            return new RaisePropertyChangedAssertion( sender, true, property.Name, value );
        }


        private static void GetInstanceAndProperty<T>( Expression<Func<T>> expression, out INotifyPropertyChanged sender, out PropertyInfo property )
        {
            MemberInfo member;
            object instance;
            if( !expression.GetMemberContext( out instance, out member ) )
                throw new BadTestException( string.Format( Resource.BadTest_NotPropertyNorIndexer ) );

            sender = instance as INotifyPropertyChanged;
            if( sender == null )
                throw new BadTestException( string.Format( Resource.BadTest_NotINotifyPropertyChanged, instance.GetType().GetFullName() ) );

            property = member as PropertyInfo;
            if( property == null )
                throw new BadTestException( string.Format( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() ) );
        }


        public static Assertion NotRaised_PropertyChanged( this SmartAssertPlaceHolder @this )
        {
            return new RaisePropertyChangedAssertion( null, false, null );
        }


        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, [NotNull] T t )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChangedAssertion( t, false, null );
        }


        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, [NotNull] T t, params string[] propertyNames )
            where T: INotifyPropertyChanged
        {
            if( t == null )
                throw new ArgumentNullException( nameof(t) );
            return new RaisePropertyChangedAssertion( t, false, propertyNames );
        }


        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> expression )
        {
            if( expression == null )
                throw new ArgumentNullException( nameof(expression) );
            INotifyPropertyChanged sender;
            PropertyInfo property;
            GetInstanceAndProperty( expression, out sender, out property );

            return new RaisePropertyChangedAssertion( sender, false, new[] { property.Name } );
        }


        private class RaisePropertyChangedAssertion: Assertion
        {
            public RaisePropertyChangedAssertion( INotifyPropertyChanged instance, bool expectedRaised, string[] propertyNames )
            {
                _Instance = instance;
                _ExpectedRaised = expectedRaised;
                _PropertyNames = propertyNames?.ToList();
                _PropertyNameVerifications = _PropertyNames?.Count > 0;
            }


            public RaisePropertyChangedAssertion( INotifyPropertyChanged instance, bool expectedRaised, string expectedPropertyName, object value )
            {
                _Instance = instance;
                _ExpectedRaised = expectedRaised;
                _PropertyNames = expectedPropertyName != null ? new List<string> { expectedPropertyName } : null;
                _PropertyNameVerifications = true;
                _Value = value;
                _CheckValue = true;
            }


            private INotifyPropertyChanged _Instance;
            private readonly bool _ExpectedRaised;
            private List<string> _PropertyNames;
            private bool _PropertyNameVerifications;
            private bool _Raised;
            private object _Value;
            private readonly bool _CheckValue;


            public override void BeforeAct( ActBase act )
            {
                var implicitSource = _Instance == null && _PropertyNames == null;

                if( _Instance == null )
                    _Instance = act.Instance as INotifyPropertyChanged;
                if( _Instance == null )
                    throw new BadTestException( string.Format( Resource.BadTest_NotINotifyPropertyChanged, act.Instance?.GetType().FullName ) );
                if( _CheckValue && implicitSource )
                {
                    var assignedAct = act as IAssignee;
                    if( assignedAct == null )
                        throw new BadTestException( Resource.BadTest_NotAssignment );
                    _Value = assignedAct.AssignedValue;
                }


                if( _PropertyNames == null )
                {
                    if( act.Property == null )
                        throw new BadTestException( string.Format( Resource.BadTest_NotProperty, act.Method.GetFullName() ) );
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
                if( _Instance != null )
                    _Instance.PropertyChanged -= InstanceOnPropertyChanged;

                if( _Raised != _ExpectedRaised )
                    throw new SmartTestException( string.Format( _ExpectedRaised
                                                                     ? Resource.ExpectedRaisedEvent
                                                                     : Resource.ExpectedNotRaisedEvent,
                                                                 "PropertyChanged"
                                                               )
                                                );
            }


            private void InstanceOnPropertyChanged( object sender, PropertyChangedEventArgs args )
            {
                if( !_ExpectedRaised )
                {
                    if( _PropertyNameVerifications )
                    {
                        if( _PropertyNames.Contains( args.PropertyName ) )
                            _Raised = true;
                        return;
                    }
                    _Raised = true;
                    return;
                }

                // Expected Raise
                _Raised = true;
                if( !_PropertyNameVerifications )
                    // No property name expected
                    return;

                // Ensure this property changed is expected
                if( !_PropertyNames.Remove( args.PropertyName ) )
                    throw new SmartTestException( string.Format( Resource.UnexpectedPropertyNameWhenPropertyChangedRaised, args.PropertyName ) );

                if( !_CheckValue )
                    return;
                // Ensure the value is the expected one
                var currentValue = _Instance.GetType().GetProperty( args.PropertyName ).GetValue( _Instance );
                if( !Equals( currentValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, currentValue ) );
            }
        }
    }
}