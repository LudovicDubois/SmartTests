using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using SmartTests.Helpers;



namespace SmartTests.Assertions
{
    /// <summary>
    ///     Specifies flags that control what is tested for non-changes.
    /// </summary>
    [Flags]
    public enum NotChangedKind
    {
        /// <summary>
        ///     Specifies that all public properties have to be checked for modification.
        /// </summary>
        PublicProperties = Visibility.Public,
        /// <summary>
        ///     Specifies that all protected properties have to be checked for modification.
        /// </summary>
        ProtectedProperties = Visibility.Protected,
        /// <summary>
        ///     Specifies that all internal properties have to be checked for modification.
        /// </summary>
        InternalProperties = Visibility.Internal,
        /// <summary>
        ///     Specifies that all private properties have to be checked for modification.
        /// </summary>
        PrivateProperties = Visibility.Private,
        /// <summary>
        ///     Specifies that all non-public properties (i.e. protected, internal and private) have to be checked for
        ///     modification.
        /// </summary>
        NonPublicProperties = ProtectedProperties | InternalProperties | PrivateProperties,
        /// <summary>
        ///     Specifies that all visible properties (i.e. public and protected) have to be checked for modification.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        VisibleProperties = PublicProperties | ProtectedProperties,
        /// <summary>
        ///     Specifies that all properties (i.e. public, protected, internal and private) have to be checked for modification.
        /// </summary>
        AllProperties = PublicProperties | ProtectedProperties | InternalProperties | PrivateProperties,
        /// <summary>
        ///     Specifies that all public fields have to be checked for modification.
        /// </summary>
        PublicFields = Visibility.Public << 4,
        /// <summary>
        ///     Specifies that all protected fields have to be checked for modification.
        /// </summary>
        ProtectedFields = Visibility.Protected << 4,
        /// <summary>
        ///     Specifies that all internal fields have to be checked for modification.
        /// </summary>
        InternalFields = Visibility.Internal << 4,
        /// <summary>
        ///     Specifies that all private fields have to be checked for modification.
        /// </summary>
        PrivateFields = Visibility.Private << 4,
        /// <summary>
        ///     Specifies that all non-public fields (i.e. protected, internal and private) have to be checked for modification.
        /// </summary>
        NonPublicFields = ProtectedFields | InternalFields | PrivateFields,
        /// <summary>
        ///     Specifies that all visible fields (i.e. public and protected) have to be checked for modification.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        VisibleFields = PublicFields | ProtectedFields,
        /// <summary>
        ///     Specifies that all fields (i.e. public, protected, internal and private) have to be checked for modification.
        /// </summary>
        AllFields = PublicFields | ProtectedFields | InternalFields | PrivateFields,
        /// <summary>
        ///     Specifies that all properties and all fields (i.e. public, protected, internal and private) have to be checked for
        ///     modification.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        All = AllProperties | AllFields
    }


    /// <summary>
    ///     Helper class to test properties/fields do not change in the Act of your test.
    /// </summary>
    public static class NotChangedAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object properties/fields did not change in the Act part of your
        ///     test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="kind">
        ///     The kind of tested members and their visibility. Default value is
        ///     <see cref="NotChangedKind.PublicProperties" />
        /// </param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If any member, of the instance involved in the Act, described by <paramref name="kind" /> has changed.
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
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If any of the specified members (using <paramref name="kind" />) has changed, a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no property nor field of <c>mc</c> changed while invoking
        ///     <c>MyMethod</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotChanged( NotChangedKind.All ) );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChanged( this SmartAssertPlaceHolder _, NotChangedKind kind = NotChangedKind.PublicProperties ) => new NotChangedAssertion( null, kind, null );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object properties/fields did not change in the Act part of your
        ///     test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which the properties/fields should not change.</param>
        /// <param name="kind">
        ///     The kind of tested members and their visibility. Default value is
        ///     <see cref="NotChangedKind.PublicProperties" />
        /// </param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If any member, of <paramref name="instance" />, described by <paramref name="kind" /> has changed.
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
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If any of the specified members (using <paramref name="kind" />) has changed, a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no property nor field of <c>other</c> changed while invoking
        ///     <c>CopyFrom</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     var other = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.CopyFrom( other ),
        ///              SmartAssert.NotChanged( other, NotChangedKind.All ) );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChanged( this SmartAssertPlaceHolder _, object instance, NotChangedKind kind = NotChangedKind.PublicProperties ) => new NotChangedAssertion( instance, kind, null );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object public properties did not change, except the one involved in
        ///     the Act part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If the Act does not involve a property nor field.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If any public property, of the instance involved in the Act, except the property implied in the Act, have changed.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If there is no public property involved in the Act, a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a public property changed during the Act (except the one involved in the Act itself), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no public property of <c>mc</c>, except <c>MyProperty</c>
        ///     changed while assigning <c>MyProperty</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              Assign( () => mc.MyProperty, 10 ),
        ///              SmartAssert.NotChangedExceptAct() );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChangedExceptAct( this SmartAssertPlaceHolder _ ) => new NotChangedAssertion( true );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object public properties did not change, except the one involved in
        ///     the Act part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If the Act does not involve a property nor field.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If any public property, of the instance involved in the Act, except the property implied in the Act, have changed.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If there is no public property involved in the Act, a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a public property changed during the Act (except the one involved in the Act itself), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no public property of <c>mc</c>, except <c>MyProperty</c>
        ///     changed while assigning <c>MyProperty</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              Assign( () => mc.MyProperty, 10 ),
        ///              SmartAssert.NotChangedExceptAct() );
        /// }
        /// </code>
        /// </example>
        [Obsolete( "Use NotChangedExceptAct instead" )]
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder _ ) => new NotChangedAssertion( true );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object public properties did not change, except the specified ones,
        ///     in
        ///     the Act part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="exceptions">The names of the public properties that can change during the Act.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If <paramref name="exceptions" /> are not public properties of the instance involve in the Act.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If any public property, of the instance involved in the Act, except the <paramref name="exceptions" />
        ///     have changed.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If names in <paramref name="exceptions" /> are not public properties of the instance involved in
        ///                     the Act, a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a public property changed during the Act (except <paramref name="exceptions" />), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no public property of <c>mc</c>, except <c>MyProperty</c> and
        ///     <c>OtherProperty</c>
        ///     changed while calling <c>MyMethod</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotChangedExcept("MyProperty", "OtherProperty") );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder _, params string[] exceptions ) => new NotChangedAssertion( null, NotChangedKind.PublicProperties, exceptions );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object properties/fields did not change, except the specified ones,
        ///     in the Act part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="kind">The kind of members and visibility to check for changes.</param>
        /// <param name="exceptions">The names of the properties/fields that can change during the Act.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If <paramref name="exceptions" /> are not properties/fields of the instance involve in the Act.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If any property/field checked (see <paramref name="kind" />), of the instance involved in the Act, except the
        ///     <paramref name="exceptions" />
        ///     have changed.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If names in <paramref name="exceptions" /> are not properties/fields of the instance involved in
        ///                     the Act, a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a property/field changed during the Act (except <paramref name="exceptions" />), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no property nor field of <c>mc</c>, except <c>MyProperty</c> and
        ///     <c>OtherProperty</c>
        ///     changed while calling <c>MyMethod</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.MyMethod(),
        ///              SmartAssert.NotChangedExcept( NotChangedKind.All, "MyProperty", "OtherProperty" ) );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder _, NotChangedKind kind, params string[] exceptions ) => new NotChangedAssertion( null, kind, exceptions );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object public properties of the specified instance did not change,
        ///     except the specified ones,
        ///     in the Act part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which to test public property changes.</param>
        /// <param name="exceptions">The names of the public properties that can change during the Act.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If <paramref name="exceptions" /> are not public properties of <paramref name="instance" />.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If any public property of <paramref name="instance" />, except the <paramref name="exceptions" /> have changed.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If names in <paramref name="exceptions" /> are not public properties of
        ///                     <paramref name="instance" />, a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a public property of <paramref name="instance" /> changed during the Act (except
        ///                     <paramref name="exceptions" />), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no public property of <c>other</c>, except <c>CopyCount</c>,
        ///     changed while calling <c>CopyPropertiesFrom</c>.
        ///     <code>
        /// [Test]
        /// public void CopyPropertiesFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     var other = new MyClass();
        /// 
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.CopyPropertiesFrom( other ),
        ///              SmartAssert.NotChangedExcept( other, "CopyCount" ) );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder _, object instance, params string[] exceptions ) => new NotChangedAssertion( instance, NotChangedKind.PublicProperties, exceptions );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object properties/fields of the specified instance did not change,
        ///     except the specified ones,
        ///     in the Act part of your test.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="instance">The instance for which to test public property changes.</param>
        /// <param name="kind">The kind of members and visibility to check for changes.</param>
        /// <param name="exceptions">The names of the properties/fields that can change during the Act.</param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="BadTestException">
        ///     If <paramref name="exceptions" /> are not properties/fields of <paramref name="instance" />.
        /// </exception>
        /// <exception cref="SmartTestException">
        ///     If any property/field checked (see <paramref name="kind" />) of <paramref name="instance" />, except the
        ///     <paramref name="exceptions" /> have changed.
        /// </exception>
        /// <remarks>
        ///     This <see cref="Assertion" /> ensures that:
        ///     <list type="number">
        ///         <item>
        ///             <term>Before the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If names in <paramref name="exceptions" /> are not properties/fields of
        ///                     <paramref name="instance" />, a <see cref="BadTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a property/field of <paramref name="instance" /> changed during the Act (except
        ///                     <paramref name="exceptions" />), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no property nor field of <c>other</c>, except <c>CopyCount</c>,
        ///     changed while calling <c>CopyPropertiesFrom</c>.
        ///     <code>
        /// [Test]
        /// public void CopyFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     var other = new MyClass();
        /// 
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.CopyPropertiesFrom( other ),
        ///              SmartAssert.NotChangedExcept( other, NotChangedKind.All, "CopyCount" ) );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder _, object instance, NotChangedKind kind, params string[] exceptions ) => new NotChangedAssertion( instance, kind, exceptions );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure object properties/fields of the specified instance did not change,
        ///     except the specified ones,
        ///     in the Act part of your test.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression that can change.</typeparam>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="expression">The expression of an instance and a property/field.</param>
        /// <param name="kind">
        ///     The kind of members and visibility to check for changes. Default value is
        ///     <see cref="NotChangedKind.PublicProperties" />.
        /// </param>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <exception cref="SmartTestException">
        ///     If any property/field checked (see <paramref name="kind" />) of the instance in <paramref name="expression" />,
        ///     except the
        ///     property/field in <paramref name="expression" /> has changed.
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
        ///             <term>After the Act</term>
        ///             <description>
        ///                 <para>
        ///                     If a property/field of the instance in <paramref name="expression" /> changed during the Act
        ///                     (except the property/field in <paramref name="expression" />), a
        ///                     <see cref="SmartTestException" /> is thrown.
        ///                 </para>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     In this example, the Smart Assertion verifies that no property nor field of <c>other</c>, except <c>CopyCount</c>,
        ///     changed while calling <c>CopyPropertiesFrom</c>.
        ///     <code>
        /// [Test]
        /// public void CopyPropertiesFromTest()
        /// {
        ///     var mc = new MyClass();
        ///     var other = new MyClass();
        /// 
        ///     RunTest( ValidValue.IsValid,
        ///              () => mc.CopyPropertiesFrom( other ),
        ///              SmartAssert.NotChangedExcept( () => other.CopyCount, NotChangedKind.All ) );
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion NotChangedExcept<T>( this SmartAssertPlaceHolder _, Expression<Func<T>> expression, NotChangedKind kind = NotChangedKind.PublicProperties )
        {
            GetInstanceAndProperty( expression, out var instance, out var property );
            return new NotChangedAssertion( instance, kind, new[] { property.Name } );
        }


        private static void GetInstanceAndProperty<T>( Expression<Func<T>> expression, out object instance, out PropertyInfo property )
        {
            if( !expression.GetMemberContext( out instance, out var member ) )
                throw new BadTestException( Resource.BadTest_NotPropertyNorIndexer );

            property = member as PropertyInfo;
            if( property == null )
                throw new BadTestException( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() );
        }


        private class NotChangedAssertion: Assertion
        {
            public NotChangedAssertion( object instance, NotChangedKind kind, string[] exceptions )
            {
                _Instance = instance;
                _Kind = kind;
                _Exceptions = exceptions;
            }


            public NotChangedAssertion( bool isImplicit )
            {
                _IsImplicit = isImplicit;
                _Instance = null;
                _Kind = NotChangedKind.PublicProperties;
                _Exceptions = null;
            }


            private readonly bool _IsImplicit;
            private object _Instance;
            private readonly NotChangedKind _Kind;
            private string[] _Exceptions;
            private PropertyInfo[] _Properties;
            private FieldInfo[] _Fields;
            private readonly Dictionary<PropertyInfo, object> _PropertyValues = new Dictionary<PropertyInfo, object>();
            private readonly Dictionary<FieldInfo, object> _FieldValues = new Dictionary<FieldInfo, object>();


            public override void BeforeAct( ActBase act )
            {
                if( _Instance == null )
                {
                    _Instance = act.Instance;
                    if( _IsImplicit )
                    {
                        if( act.Property == null )
                            throw new BadTestException( Resource.BadTest_NotPropertyNorField, act.Member.Name, _Instance.GetType().GetFullName() );
                        _Exceptions = new[] { act.Property.Name };
                    }
                }

                var propertiesVisibility = (Visibility)( _Kind & NotChangedKind.AllProperties );
                var instanceType = _Instance.GetType();
                _Properties = propertiesVisibility != 0
                                  ? instanceType.GetProperties( propertiesVisibility )
                                  : new PropertyInfo[0];

                var fieldsVisibility = (Visibility)( (int)( _Kind & NotChangedKind.AllFields ) >> 4 );
                _Fields = fieldsVisibility != 0
                              ? instanceType.GetFields( fieldsVisibility )
                              : new FieldInfo[0];

                if( _Exceptions != null )
                {
                    CheckExceptions();
                    _Properties = _Properties.Where( prop => !_Exceptions.Contains( prop.Name ) ).ToArray();
                    _Fields = _Fields.Where( field => !_Exceptions.Contains( field.Name ) ).ToArray();
                }

                foreach( var property in _Properties )
                    if( property.CanRead &&
                        property.GetMethod.GetParameters().Length == 0 )
                        _PropertyValues[ property ] = property.GetValue( _Instance );
                foreach( var field in _Fields )
                    _FieldValues[ field ] = field.GetValue( _Instance );
            }


            private void CheckExceptions()
            {
                var names = _Properties.Select( prop => prop.Name ).ToList();
                names.AddRange( _Fields.Select( field => field.Name ) );

                var message = new StringBuilder();
                foreach( var exception in _Exceptions )
                {
                    if( !names.Contains( exception ) )
                    {
                        message.AppendLine();
                        message.AppendFormat( Resource.BadTest_NotPropertyNorField, exception, _Instance.GetType().GetFullName() );
                    }
                }

                if( message.Length > 0 )
                    throw new BadTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
            }


            public override void AfterAct( ActBase act )
            {
                if( act.Exception != null )
                    return;

                var message = new StringBuilder();
                foreach( var pair in _PropertyValues )
                    if( !Equals( _PropertyValues[ pair.Key ], pair.Key.GetValue( _Instance ) ) )
                    {
                        message.AppendLine();
                        message.Append( string.Format( Resource.PropertyChanged, pair.Key.GetFullName() ) );
                    }

                foreach( var pair in _FieldValues )
                    if( !Equals( _FieldValues[ pair.Key ], pair.Key.GetValue( _Instance ) ) )
                    {
                        message.AppendLine();
                        message.AppendFormat( Resource.FieldChanged, pair.Key.GetFullName() );
                    }

                if( message.Length > 0 )
                    throw new SmartTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
            }
        }
    }
}