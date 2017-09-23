using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SmartTests.Acts;



namespace SmartTests
{
    /// <summary>
    ///     The base class of all Act classes.
    /// </summary>
    /// <remarks>
    ///     Do not use directly. Prefer using <see cref="SmartTest.RunTest" /> methods.
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
        ///     The thrown <see cref="Exception" /> when running <see cref="SmartTest.RunTest" /> methods, if any.
        /// </summary>
        /// <remarks>
        ///     This property is <c>null</c> if the Act nor Smart Assertions threw an exception.
        /// </remarks>
        public Exception Exception { get; internal set; }

        /// <summary>
        ///     The Smart <see cref="Assertion" /> implied in the Associated <see cref="SmartTest.RunTest" /> methods, if any.
        /// </summary>
        public Assertion[] Assertions { get; internal set; }
        private readonly List<Assertion> _DoneAssertions = new List<Assertion>();


        internal virtual void BeforeAct()
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
    }


    /// <summary>
    ///     The base class of all Act classes that are not expressions (such as invoking a void method).
    /// </summary>
    /// <remarks>
    ///     Do not use directly. Prefer using <see cref="SmartTest.RunTest" /> methods.
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
        public abstract void Invoke();
    }


    /// <summary>
    ///     The base class of all Act classes that are expressions.
    /// </summary>
    /// <remarks>
    ///     Do not use directly. Prefer using <see cref="SmartTest.RunTest" /> methods.
    /// </remarks>
    /// <seealso cref="ActBase" />
    /// <seealso cref="AssignAct{T}" />
    /// <seealso cref="InvokeAct{T}" />
    /// <seealso cref="Act" />
    public abstract class Act<T>: ActBase
    {
        /// <summary>
        ///     Run the Act part of your test.
        /// </summary>
        /// <returns></returns>
        public abstract T Invoke();
    }
}