using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using SmartTests.Acts;
using SmartTests.Helpers;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test actual changes of properties/indexers.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest.Assign{T}" />
    /// <seealso cref="SmartTest" />
    // ReSharper disable once UnusedMember.Global
    public static class PropertyChangedAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed to a different value in the Act
        ///     part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     <para>If the instance is <c>null</c> or the property/indexer cannot be inferred from the Act.</para>
        ///     <para>If the value of the property before the Act is the value involved in the Act.</para>
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is not raised.</para>
        ///     <para>If <see cref="PropertyChangedEventArgs.PropertyName" /> is not the involved property in Act.</para>
        ///     <para>If the current value of the property is not the involved value in Act during the Act or after the Act.</para>
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The value of the property/indexer IS NOT the value of the Act; otherwise a
        ///                     <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is raised.
        ///                 </para>
        ///                 <para>
        ///                     The <see cref="PropertyChangedEventArgs.PropertyName" /> is the name of the property of the Act;
        ///                     otherwise a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>
        ///                     The value of the property/indexer in the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///                     handler is the one involved in the Act; otherwise a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>
        ///                     The value of the property/indexer is the one involved in the Act; otherwise a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///         event is raised for the property <c>MyProperty</c> and the value is <c>10</c>.
        ///     </para>
        ///     <para>It also ensures that the value is still <c>10</c> after the Act.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              Assign( () => mc.MyProperty, 10 ),
        ///              SmartAssert.Raised_PropertyChanged() );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Raised_PropertyChanged( this SmartAssertPlaceHolder _ ) => new RaisePropertyChangedAssertion( null, true, null, null );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed to a different value in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the property/indexer should change.</param>
        /// <param name="propertyNames">The names of the property/indexer that should change.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="instance" /> or <paramref name="propertyNames" /> are <c>null</c>.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is not raised.</para>
        ///     If <see cref="PropertyChangedEventArgs.PropertyName" /> is not in <paramref name="propertyNames" /> in the event.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>Nothing special.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is raised.
        ///                 </para>
        ///                 <para>
        ///                     The <see cref="PropertyChangedEventArgs.PropertyName" /> is one of
        ///                     <paramref name="propertyNames" />;
        ///                     otherwise a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>Note that you have to repeat names if the event is raised multiple times for the same property.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>If the event is not raised, a <see cref="SmartTestException" /> is thrown.</para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///     event is raised twice, once for <c>MyProperty</c> and once for <c>OtherProperty</c>, in any order.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised_PropertyChanged( mc, nameof(MyClass.MyProperty), nameof(MyClass.OtherProperty) ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder _, T instance, params string[] propertyNames )
            where T: INotifyPropertyChanged
        {
            if( instance == null )
                throw new ArgumentNullException( nameof(instance) );
            return new RaisePropertyChangedAssertion( instance, true, propertyNames );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed to a different value in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the property/indexer should change.</param>
        /// <param name="propertyName">The name of the property/indexer that should change.</param>
        /// <param name="expectedValue">The expected value of the property/indexer once the Act part is done.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="instance" /> or <paramref name="propertyName" /> are <c>null</c>.
        /// </exception>
        /// <exception cref="BadTestException">
        ///     <para>If the value of the property is <paramref name="expectedValue" /> before the Act.</para>
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is not raised.</para>
        ///     <para>
        ///         If <see cref="PropertyChangedEventArgs.PropertyName" /> is not <paramref name="propertyName" /> in the event
        ///         handler
        ///     </para>
        ///     <para>
        ///         If the current value of the property is not <paramref name="expectedValue" /> in the event handler and after
        ///         the Act.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The value of the property/indexer IS NOT <paramref name="expectedValue" />; otherwise a
        ///                     <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is raised.
        ///                 </para>
        ///                 <para>
        ///                     The <see cref="PropertyChangedEventArgs.PropertyName" /> is <paramref name="propertyName" />;
        ///                     otherwise a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>
        ///                     The value of the property/indexer in the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///                     handler is <paramref name="expectedValue" />; otherwise a <see cref="SmartTestException" /> is
        ///                     thrown
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>
        ///                     The value of the property/indexer is <paramref name="expectedValue" />; otherwise a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///         event is raised for the property <c>MyProperty</c> and the value is <c>10</c>.
        ///     </para>
        ///     <para>It also ensures that the value is still <c>10</c> after the Act.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised_PropertyChanged( mc, nameof(MyClass.MyProperty), 10 ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder _, T instance, string propertyName, object expectedValue )
            where T: INotifyPropertyChanged
        {
            if( instance == null )
                throw new ArgumentNullException( nameof(instance) );
            if( propertyName == null )
                throw new ArgumentNullException( nameof(propertyName) );
            return new RaisePropertyChangedAssertion( instance, true, propertyName, expectedValue );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed to a different value in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="expression">The expression that will change, directly or indirectly, the property/indexer.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     <para>If <paramref name="expression" /> is not a <see cref="MemberExpression" /> with a property.</para>
        ///     <para>
        ///         If the involved instance of <paramref name="expression" /> is not an <see cref="INotifyPropertyChanged" />
        ///         instance.
        ///     </para>
        ///     <para>If the member is not a Property.</para>
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is not raised.</para>
        ///     If <see cref="PropertyChangedEventArgs.PropertyName" /> is not the name of the property/indexer in the Act.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Nothing special.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is raised.
        ///                 </para>
        ///                 <para>
        ///                     The <see cref="PropertyChangedEventArgs.PropertyName" /> is the one in the Act;
        ///                     otherwise a <see cref="SmartTestException" /> is thrown.
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
        ///         In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///         event is raised for the property <c>MyProperty</c>.
        ///     </para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised_PropertyChanged( () => mc.MyProperty ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder _, Expression<Func<T>> expression )
        {
            GetInstanceAndProperty( expression, out var sender, out var property );

            return new RaisePropertyChangedAssertion( sender, true, new[] { property.Name } );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer changed to a different value in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="expression">The expression that will change, directly or indirectly, the property/indexer.</param>
        /// <param name="expectedValue">The expected value of the property/indexer once the Act part is done.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     <para>If <paramref name="expression" /> is not a <see cref="MemberExpression" /> with a property.</para>
        ///     <para>
        ///         If the involved instance of <paramref name="expression" /> is not an <see cref="INotifyPropertyChanged" />
        ///         instance.
        ///     </para>
        ///     <para>If the member is not a Property.</para>
        ///     <para>If the value of the <paramref name="expression" /> is <paramref name="expectedValue" /> before the Act.</para>
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is not raised.</para>
        ///     If <see cref="PropertyChangedEventArgs.PropertyName" /> is not the name of the property/indexer in the Act.
        ///     <para>
        ///         If the current value of the property is not <paramref name="expectedValue" /> in the event handler and after
        ///         the Act.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     The value of the property/indexer IS NOT <paramref name="expectedValue" />; otherwise a
        ///                     <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is raised.
        ///                 </para>
        ///                 <para>
        ///                     The <see cref="PropertyChangedEventArgs.PropertyName" /> is the one in the Act;
        ///                     otherwise a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>
        ///                     The value of the property/indexer in the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///                     handler is <paramref name="expectedValue" />; otherwise a <see cref="SmartTestException" /> is
        ///                     thrown
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If the event was not raised, a <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///                 <para>
        ///                     The value of the property/indexer is <paramref name="expectedValue" />; otherwise a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///         event is raised for the property <c>MyProperty</c> and the value is <c>10</c>.
        ///     </para>
        ///     <para>It also ensures that the value is still <c>10</c> after the Act.</para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.Raised_PropertyChanged( () => mc.MyProperty, 10 ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Raised_PropertyChanged<T>( this SmartAssertPlaceHolder _, Expression<Func<T>> expression, T expectedValue )
        {
            GetInstanceAndProperty( expression, out var sender, out var property );

            return new RaisePropertyChangedAssertion( sender, true, property.Name, expectedValue );
        }


        private static void GetInstanceAndProperty<T>( Expression<Func<T>> expression, out INotifyPropertyChanged sender, out PropertyInfo property )
        {
            if( !expression.GetMemberContext( out var instance, out var member ) )
                throw SmartTest.InconclusiveException( Resource.BadTest_NotPropertyNorIndexer );

            sender = instance as INotifyPropertyChanged;
            if( sender == null )
                throw SmartTest.InconclusiveException( Resource.BadTest_NotINotifyPropertyChanged, instance.GetType().GetFullName() );

            property = member as PropertyInfo;
            if( property == null )
                throw SmartTest.InconclusiveException( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer did not change at all in the Act
        ///     part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If the involved instance in the Act is not an <see cref="INotifyPropertyChanged" /> instance.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is raised for any property.</para>
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>Nothing special.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is not raised for any
        ///                     property.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>If the event is raised, a <see cref="SmartTestException" /> is thrown.</para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///     event is not raised for any property of <c>mc</c>.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotRaised_PropertyChanged() );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotRaised_PropertyChanged( this SmartAssertPlaceHolder _ )
        {
            return new RaisePropertyChangedAssertion( null, false, null );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer did not change at all in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should not change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the property/indexer should change.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="instance" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>If the <see cref="INotifyPropertyChanged" /> event is raised for any property.</para>
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>Nothing special.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is not raised for any
        ///                     property.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>If the event is raised, a <see cref="SmartTestException" /> is thrown.</para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///     event is not raised for any property of <c>mc</c>.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotRaised_PropertyChanged( mc ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssertPlaceHolder _, T instance )
            where T: INotifyPropertyChanged
        {
            if( instance == null )
                throw new ArgumentNullException( nameof(instance) );
            return new RaisePropertyChangedAssertion( instance, false, null );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer did not change in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should not change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the property/indexer should change.</param>
        /// <param name="propertyNames">The names of the property/indexer that should not change.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="instance" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     <para>
        ///         If the <see cref="INotifyPropertyChanged" /> event is raised for the specified
        ///         <paramref name="propertyNames" />.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>Nothing special.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is not raised for any of
        ///                     the specified <paramref name="propertyNames" />.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>If the event is raised, a <see cref="SmartTestException" /> is thrown.</para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///     event is not raised for <c>MyProperty</c> nor <c>OtherProperty</c>, in any order.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotRaised_PropertyChanged( mc, nameof(MyClass.MyProperty), nameof(MyClass.OtherProperty) ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssertPlaceHolder _, T instance, params string[] propertyNames )
            where T: INotifyPropertyChanged
        {
            if( instance == null )
                throw new ArgumentNullException( nameof(instance) );
            return new RaisePropertyChangedAssertion( instance, false, propertyNames );
        }


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure an object property/indexer did not change in the Act
        ///     part of your test.
        /// </summary>
        /// <typeparam name="T">The type of the property/indexer that should not change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="expression">The expression involving an instance an a property/indexer that should not change.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="expression" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If the <see cref="INotifyPropertyChanged" /> event is raised for the property from <paramref name="expression" />.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>Nothing special.</para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>During the Act</term>
        ///             <description>
        ///                 <para>
        ///                     Ensures the <see cref="INotifyPropertyChanged.PropertyChanged" /> event is not raised for
        ///                     the specified property in <paramref name="expression" />.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>If the event is raised, a <see cref="SmartTestException" /> is thrown.</para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that the <see cref="INotifyPropertyChanged.PropertyChanged" />
        ///     event is not raised for <c>MyProperty</c>.
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotRaised_PropertyChanged( () => mc.MyProperty ) );
        /// }</code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotRaised_PropertyChanged<T>( this SmartAssertPlaceHolder _, Expression<Func<T>> expression )
        {
            if( expression == null )
                throw new ArgumentNullException( nameof(expression) );
            GetInstanceAndProperty( expression, out var sender, out var property );

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
                    throw SmartTest.InconclusiveException( Resource.BadTest_NotINotifyPropertyChanged, act.Instance?.GetType().FullName );
                if( _CheckValue && implicitSource )
                {
                    var assignedAct = act as IAssignee;
                    if( assignedAct == null )
                        throw SmartTest.InconclusiveException( Resource.BadTest_NotAssignment );
                    _Value = assignedAct.AssignedValue;
                }


                if( _ExpectedRaised &&
                    _PropertyNames == null )
                {
                    if( act.Property == null )
                        throw SmartTest.InconclusiveException( Resource.BadTest_NotProperty, act.Method.GetFullName() );
                    _PropertyNames = new List<string>
                                     {
                                         act.Property.Name
                                     };
                }

                if( implicitSource &&
                    _PropertyNames != null )
                    _PropertyNameVerifications = true;

                _Instance.PropertyChanged += InstanceOnPropertyChanged;
            }


            public override void AfterAct( ActBase act )
            {
                if( _Instance != null )
                    _Instance.PropertyChanged -= InstanceOnPropertyChanged;

                if( act.Exception != null )
                    return;

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
                var currentValue = _Instance.GetType().GetRuntimeProperty( args.PropertyName ).GetValue( _Instance );
                if( !Equals( currentValue, _Value ) )
                    throw new SmartTestException( string.Format( Resource.ChangeWrongly, _Value, currentValue ) );
            }
        }
    }
}