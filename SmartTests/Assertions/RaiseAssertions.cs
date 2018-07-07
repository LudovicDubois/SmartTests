using System;
using System.Diagnostics;
using System.Reflection;

using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedParameter.Global


namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test standard event (<see cref="EventHandler" /> compatible) raise.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest" />
    // ReSharper disable once UnusedMember.Global
    public static class RaiseAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an standard event is raised in the Act part of your test.
        /// </summary>
        /// <param name="this">The dummy place holder for all Smart Assertions.</param>
        /// <param name="expectedEventName">The name of the event that should raise.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="expectedEventName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="expectedEventName" /> is empty.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     If the <paramref name="expectedEventName" /> is not a valid event of the type of the instance involved in the Act.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="expectedEventName" /> event is not raised.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The <paramref name="expectedEventName" /> exists in the type of instance involved in tha Act type;
        ///                     otherwise a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <c>MyEvent</c> event is raised for <c>mc</c>.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised( "MyEvent" ) );
        /// }
        /// </code>
        /// </example>
        public static Assertion Raised( this SmartAssertPlaceHolder @this, string expectedEventName ) => new RaiseAssertion( null, true, expectedEventName );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an standard event is raised in the Act part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="EventHandler{TEventArgs}" /> of the event that should raise.</typeparam>
        /// <param name="this">The dummy place holder for all Smart Assertions.</param>
        /// <param name="expectedEventName">The name of the event that should raise.</param>
        /// <param name="assert">The handler that must be run in Act part to do specific assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="expectedEventName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="expectedEventName" /> is empty.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     If the <paramref name="expectedEventName" /> is not a valid event of the type of the instance involved in the Act.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="expectedEventName" /> event is not raised.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The <paramref name="expectedEventName" /> exists in the type of the instance involved in the Act
        ///                     type;
        ///                     otherwise a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Invoke the <paramref name="assert" /> handler, to test specific assertions.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the Smart Assertion verifies that the <c>MyEvent</c> event is raised for <c>mc</c>.
        ///     </para>
        ///     <para>It also ensure anything you want in the specified handler.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised( "MyEvent",
        ///                                  ( sender, args ) => {
        ///                                                          // Check anything you want when MyEvent is raised.
        ///                                                      }));
        /// }
        /// </code>
        /// </example>
        public static Assertion Raised<T>( this SmartAssertPlaceHolder @this, string expectedEventName, EventHandler<T> assert )
            where T: EventArgs
            => assert != null
                   ? new RaiseAssertion<T>( null, true, expectedEventName, assert )
                   : new RaiseAssertion( null, true, expectedEventName );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an standard event is raised in the Act part of your test.
        /// </summary>
        /// <param name="this">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the event should raise.</param>
        /// <param name="expectedEventName">The name of the event that should raise.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="expectedEventName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="expectedEventName" /> is empty.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     If the <paramref name="expectedEventName" /> is not a valid event of the type of the <paramref name="instance" />.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="expectedEventName" /> event is not raised.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The <paramref name="expectedEventName" /> exists in the type of <paramref name="instance" /> type;
        ///                     otherwise a
        ///                     <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <c>MyEvent</c> event is raised for <c>mc.InnerObject</c>
        ///     inner instance.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised( mc.InnerObject, "MyEvent" ) );
        /// }
        /// </code>
        /// </example>
        public static Assertion Raised( this SmartAssertPlaceHolder @this, object instance, string expectedEventName ) => new RaiseAssertion( instance, true, expectedEventName );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an standard event is raised in the Act part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="EventHandler{TEventArgs}" /> of the event that should raise.</typeparam>
        /// <param name="this">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the event should raise.</param>
        /// <param name="expectedEventName">The name of the event that should raise.</param>
        /// <param name="assert">The handler that must be run in Act part to do specific assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="expectedEventName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="expectedEventName" /> is empty.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     If the <paramref name="expectedEventName" /> is not a valid event of the type of the <paramref name="instance" />.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="expectedEventName" /> event is not raised.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The <paramref name="expectedEventName" /> exists in the type of <paramref name="instance" /> type;
        ///                     otherwise a
        ///                     <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Invoke the <paramref name="assert" /> handler, to test specific assertions.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the Smart Assertion verifies that the <c>MyEvent</c> event is raised for <c>mc.InnerObject</c>
        ///         inner instance.
        ///     </para>
        ///     <para>It also ensure anything you want in the specified handler.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised( mc.InnerObject,
        ///                                  "MyEvent",
        ///                                  ( sender, args ) => {
        ///                                                          // Check anything you want when MyEvent is raised.
        ///                                                      }));
        /// }
        /// </code>
        /// </example>
        public static Assertion Raised<T>( this SmartAssertPlaceHolder @this, object instance, string expectedEventName, EventHandler<T> assert )
            where T: EventArgs
            => assert != null
                   ? new RaiseAssertion<T>( instance, true, expectedEventName, assert )
                   : new RaiseAssertion( instance, true, expectedEventName );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an standard event is not raised in the Act part of your test.
        /// </summary>
        /// <param name="this">The dummy place holder for all Smart Assertions.</param>
        /// <param name="eventName">The name of the event that should not raise.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="eventName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="eventName" /> is empty.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     If the <paramref name="eventName" /> is not a valid event of the type of the instance involved in the Act.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="eventName" /> event is raised.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The <paramref name="eventName" /> exists in the type of the involved instance of the Act;
        ///                     otherwise a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <c>MyEvent</c> event is not raised for <c>mc</c>.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotRaised( "MyEvent" ) );
        /// }
        /// </code>
        /// </example>
        public static Assertion NotRaised( this SmartAssertPlaceHolder @this, string eventName ) => new RaiseAssertion( null, false, eventName );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an standard event is not raised in the Act part of your test.
        /// </summary>
        /// <param name="this">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the event should not raise.</param>
        /// <param name="eventName">The name of the event that should not raise.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="eventName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="eventName" /> is empty.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     If the <paramref name="eventName" /> is not a valid event of the type of the <paramref name="instance" />.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <paramref name="eventName" /> event is raised.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The <paramref name="eventName" /> exists in the type of <paramref name="instance" /> type;
        ///                     otherwise a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <c>MyEvent</c> event is not raised for <c>mc</c>.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotRaised( mc.InnerObject, "MyEvent" ) );
        /// }
        /// </code>
        /// </example>
        public static Assertion NotRaised( this SmartAssertPlaceHolder @this, object instance, string eventName ) => new RaiseAssertion( instance, false, eventName );


        private class RaiseAssertion: Assertion
        {
            public RaiseAssertion( object instance, bool expectedRaised, string eventName )
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
                _Event = instanceType.GetRuntimeEvent( _EventName );
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
                Assert( sender, args );
            }


            protected virtual void Assert( object sender, EventArgs args )
            { }
        }


        private class RaiseAssertion<T>: RaiseAssertion
            where T: EventArgs
        {
            public RaiseAssertion( object instance, bool expectedRaised, string eventName, EventHandler<T> assert )
                : base( instance, expectedRaised, eventName )
            {
                _Assert = assert;
            }


            private readonly EventHandler<T> _Assert;


            protected override void Assert( object sender, EventArgs args )
            {
                _Assert.Invoke( sender, (T)args );
            }
        }
    }
}