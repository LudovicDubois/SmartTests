using System;
using System.Diagnostics;
using System.Reflection;

using JetBrains.Annotations;

using SmartTests.Helpers;

// ReSharper disable UnusedParameter.Global


namespace SmartTests.Assertions
{
    public static class RaiseAssertions
    {
        public static Assertion Raised( this SmartAssertPlaceHolder @this, [NotNull] string expectedEventName ) => new RaiseAssertion( null, true, expectedEventName );


        public static Assertion Raised( this SmartAssertPlaceHolder @this, object instance, [NotNull] string expectedEventName ) => new RaiseAssertion( instance, true, expectedEventName );


        public static Assertion NotRaised( this SmartAssertPlaceHolder @this, [NotNull] string eventName ) => new RaiseAssertion( null, false, eventName );
        public static Assertion NotRaised( this SmartAssertPlaceHolder @this, object instance, [NotNull] string eventName ) => new RaiseAssertion( instance, false, eventName );


        private class RaiseAssertion: Assertion
        {
            public RaiseAssertion( object instance, bool expectedRaised, [NotNull] string eventName )
            {
                if( eventName == null )
                    throw new ArgumentNullException( nameof(eventName) );
                if( string.IsNullOrEmpty( eventName ) )
                    throw new ArgumentException( nameof(eventName) );

                _Instance = instance;
                _ExpectedRaised = expectedRaised;
                _EventName = eventName;
                _RealDelegate = new EventHandler( InstanceOnRaised );
            }


            private object _Instance;
            private readonly bool _ExpectedRaised;
            private readonly string _EventName;
            private EventInfo _Event;
            private readonly Delegate _RealDelegate;
            private bool _Raised;


            public override void BeforeAct( ActBase act )
            {
                if( _Instance == null )
                    _Instance = act.Instance;

                var instanceType = _Instance.GetType();
                Debug.Assert( instanceType != null );
                _Event = instanceType.GetEvent( _EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
                if( _Event == null )
                    throw new BadTestException( string.Format( Resource.BadTest_NotEvent, _EventName, instanceType.GetFullName() ) );

                _Event.AddEventHandler( _Instance, _RealDelegate );
            }


            public override void AfterAct( ActBase act )
            {
                _Event.RemoveEventHandler( _Instance, _RealDelegate );

                if( _Raised != _ExpectedRaised )
                    throw new SmartTestException( string.Format( _ExpectedRaised
                                                                     ? Resource.ExpectedRaisedEvent
                                                                     : Resource.ExpectedNotRaisedEvent,
                                                                 _Event.GetFullName()
                                                               )
                                                );
            }


            private void InstanceOnRaised( object sender, EventArgs args )
            {
                _Raised = true;
            }
        }
    }
}