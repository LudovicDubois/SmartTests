using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SmartTests.Acts;
using SmartTests.Assertions;



namespace SmartTests
{
    /// <summary>
    /// Context of the Act part of your test.
    /// </summary>
    /// <remarks>
    /// <para>Do not use it directly. It is only useful for Smart Assertions development.</para>
    /// <para>This context enables you to store information that can be accessible in your Act if needed.</para>
    /// <para>The best is to encapsulate this information using some extensions methods on this class; thus, 
    /// you have meaningful names and right types.</para>
    /// <para>see cref="WaitAssertions.SetHandle"/> for an example.</para>
    /// </remarks>
    public class ActContext
    {
        private readonly Dictionary<string, object> _Context = new Dictionary<string, object>();


        /// <summary>
        /// Gets or sets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified name. 
        /// If the specified name is not found, a get operation throws a <see cref="KeyNotFoundException"/>, 
        /// and a set operation creates a new element with the specified name.
        /// </returns>
        /// <remarks>
        /// <para>Do not use it directly. It is only useful for Smart Assertions development.</para>
        /// <para>The best is to encapsulate this information using some extensions methods on this class; thus, 
        /// you have meaningful names and right types.</para>
        /// <para><see cref="WaitAssertions.SetHandle"/> for an example.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="name"/> does not exist in the dictionary.</exception>
        public object this[ string name ]
        {
            get => _Context[ name ];
            set => _Context[ name ] = value;
        }


        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get.</param>
        /// <returns>
        /// The value associated with the specified name. 
        /// If the specified name is not found, throws a <see cref="KeyNotFoundException"/>.
        /// </returns>
        /// <remarks>
        /// <para>Do not use it directly. It is only useful for Smart Assertions development.</para>
        /// <para>The best is to encapsulate this information using some extensions methods on this class; thus, 
        /// you have meaningful names and right types.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="name"/> does not exist in the dictionary.</exception>
        public T GetValue<T>( string name ) => (T)this[ name ];


        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="value">The value associated with the specified name.
        /// If the specified name is not found, returns <c>false</c> and <paramref name="value"/> is <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the context contains an element with the specified name; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>Do not use it directly. It is only useful for Smart Assertions development.</para>
        /// <para>The best is to encapsulate this information using some extensions methods on this class; thus, 
        /// you have meaningful names and right types.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public bool TryGetValue( string name, out object value ) => _Context.TryGetValue( name, out value );


        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="value">The value associated with the specified name.
        /// If the specified name is not found, returns <c>false</c> and <paramref name="value"/> is <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the context contains an element with the specified name; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>Do not use it directly. It is only useful for Smart Assertions development.</para>
        /// <para>The best is to encapsulate this information using some extensions methods on this class; thus, 
        /// you have meaningful names and right types.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
        public bool TryGetValue<T>( string name, out T value )
        {
            if( !_Context.TryGetValue( name, out object val ) )
            {
                value = default(T);
                return false;
            }
            value = (T)val;
            return true;
        }
    }


    /// <summary>
    ///     The base class of all Act classes.
    /// </summary>
    /// <remarks>
    ///     Do not use directly. Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </remarks>
    /// <seealso cref="SmartTest" />
    public abstract class ActBase
    {
        /// <summary>
        ///     The instance of the Act part of the test, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act is for testing a <c>static</c> declaration.
        /// </remarks>
        public object Instance { get; internal set; }
        /// <summary>
        ///     The <see cref="ConstructorInfo" /> of the Act part of the test, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act is not for testing a constructor.
        /// </remarks>
        public ConstructorInfo Constructor { get; internal set; }

        /// <summary>
        ///     The <see cref="FieldInfo" /> of the Act part of the test, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act is not for testing a field.
        /// </remarks>
        public FieldInfo Field { get; internal set; }
        /// <summary>
        ///     The <see cref="MethodInfo" /> of the Act part of the test, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act is not for testing a method.
        /// </remarks>
        public MethodInfo Method { get; internal set; }
        /// <summary>
        ///     The <see cref="PropertyInfo" /> of the Act part of the test, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act is not for testing a property.
        /// </remarks>
        public PropertyInfo Property { get; internal set; }

        /// <summary>
        ///     The thrown <see cref="Exception" /> when running <see cref="O:SmartTests.SmartTest.RunTest" /> methods, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act nor Smart Assertions threw an exception.
        /// </remarks>
        public Exception Exception { get; internal set; }

        /// <summary>
        ///     The <see cref="MemberInfo" /> of the Act prt of the test, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act is not for testing a member.
        /// </remarks>
        public MemberInfo Member => Constructor ?? Field ?? (MemberInfo)Method ?? Property;

        /// <summary>
        ///     The Smart <see cref="Assertion" /> implied in the Associated <see cref="O:SmartTests.SmartTest.RunTest" /> methods,
        ///     if any.
        /// </summary>
        public Assertion[] Assertions { get; internal set; }
        private readonly List<Assertion> _DoneAssertions = new List<Assertion>();


        internal void BeforeAct()
        {
            var assertions = Assertions.ToList();
            assertions.Reverse();
            foreach( var assertion in assertions )
            {
                assertion.BeforeAct( this );
                _DoneAssertions.Insert( 0, assertion );
            }
        }


        internal void AfterAct()
        {
            foreach( var assertion in _DoneAssertions )
                assertion.AfterAct( this );
        }


        #region Context        

        /// <summary>
        /// Gets the context of this Act.
        /// </summary>
        /// <returns>
        /// The <see cref="ActContext"/> of this Act instance.
        /// </returns>
        /// <remarks>
        /// <para>Do not use it directly. It is only useful for Smart Assertions development.</para>
        /// <para>The best is to encapsulate this information using some extensions methods on this class; thus, 
        /// you have meaningful names and right types.</para>
        /// </remarks>
        /// <para>see cref="WaitAssertions.SetHandle"/> for an example.</para>
        public ActContext Context { get; } = new ActContext();

        #endregion
    }


    /// <summary>
    ///     The base class of all Act classes that are not expressions (such as invoking a void method).
    /// </summary>
    /// <remarks>
    ///     Do not use directly. Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </remarks>
    /// <seealso cref="ActBase" />
    /// <seealso cref="InvokeAct" />
    /// <seealso cref="Act{T}" />
    /// <seealso cref="SmartTest" />
    public abstract class Act: ActBase
    {
        /// <summary>
        ///     Run the Act part of your test.
        /// </summary>
        public abstract void Invoke( ActContext context );
    }


    /// <summary>
    ///     The base class of all Act classes that are expressions.
    /// </summary>
    /// <remarks>
    ///     Do not use directly. Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </remarks>
    /// <typeparam name="T"> The result <see cref="Type" /> of the Act.</typeparam>
    /// <seealso cref="ActBase" />
    /// <seealso cref="AssignAct{T}" />
    /// <seealso cref="InvokeAct{T}" />
    /// <seealso cref="Act" />
    public abstract class Act<T>: ActBase
    {
        /// <summary>
        ///     Run the Act part of your test.
        /// </summary>
        /// <returns>The result of the Act part of your test.</returns>
        public abstract T Invoke( ActContext context );
    }
}