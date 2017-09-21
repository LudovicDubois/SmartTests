using System;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

using SmartTests.Acts;
using SmartTests.Helpers;



namespace SmartTests
{
    /// <summary>
    ///     This is the main class for SmartTests.
    /// </summary>
    /// <remarks>
    ///     It is recommended to use it with <c>using static SmartTests.SmartTest</c>.
    /// </remarks>
    [PublicAPI]
    public static class SmartTest
    {
        #region Case

        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a global <see cref="Criteria" /> (not specific to a
        ///     parameter).
        /// </summary>
        /// <param name="criteria">The <see cref="Criteria" /> expression for the created <see cref="SmartTests.Case" />.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <example>
        ///     <code>
        ///  [Test]
        ///  public void TestMethod()
        ///  {
        ///      var result = RunTest( Case( MinIncluded.IsAboveMin ), 
        ///                            () => Math.Sqrt( 4 ) );
        ///      Assert.That( result, Is.EqualTo( 2 ) );
        ///  }
        ///  </code>
        /// </example>
        /// <seealso cref="Case(string,SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case Case( Criteria criteria ) => new Case( criteria );


        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a specific parameter <see cref="Criteria" />.
        /// </summary>
        /// <param name="parameterName">The name of the parameter for which this <see cref="SmartTests.Case" /> belongs to.</param>
        /// <param name="criteria">The <see cref="Criteria" /> for the created <see cref="SmartTests.Case" />.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <code>
        /// [Test]
        /// public void Max_ValuesGreaterThanMin()
        /// {
        ///     var remainder;
        ///     var result = RunTest( Case( "a", AnyValue.IsValid ) &
        ///                           Case( "b", ValidValue.IsValid ),
        ///                           () => Math.DivRem( 5, 2, out remainder ) );
        /// 
        ///     Assert.AreEqual( 2, result );
        ///     Assert.AreEqual( 1, remainder );
        /// }
        /// </code>
        /// <seealso cref="Case(SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case Case( string parameterName, Criteria criteria ) => new Case( parameterName, criteria );

        #endregion


        [ThreadStatic]
        private static object _Instance;
        [ThreadStatic]
        private static MethodBase _Method;


        internal static void SetInfo( object instance, MethodBase method )
        {
            _Instance = instance;
            _Method = method;
        }


        #region Assign

        /// <summary>
        ///     Creates an <see cref="AssignAct{T}" />, i.e. represents an Assignment of a
        ///     <paramref name="property" />
        ///     for the Act part of the test.
        /// </summary>
        /// <typeparam name="T">The type of the property to set.</typeparam>
        /// <param name="property">A lambda expression of the property to set in the Act part.</param>
        /// <param name="value">
        ///     The value to assign to the <paramref name="property" />.
        /// </param>
        /// <returns>The newly created <see cref="AssignAct{T}" />.</returns>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void MyPropertyTest()
        /// {
        ///     var mc = new MyClass( 10 );
        /// 
        ///     var result = RunTest( AnyValue.IsValid,
        ///                           () => mc.Value );
        /// 
        ///     Assert.AreEqual( 10, result );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="AssignAct{T}" />
        public static Act<T> Assign<T>( Expression<Func<T>> property, T value ) => new AssignAct<T>( property, value );

        #endregion


        #region RunTest

        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression in the Act part of the test.</typeparam>
        /// <param name="cases">The <see cref="Criteria" /> expression for the tested expression.</param>
        /// <param name="act">A lambda expression representing the tested expression.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <returns>The value of the expression of the <paramref name="act" />.</returns>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void MyPropertyTest()
        /// {
        ///     var mc = new MyClass( 10 );
        /// 
        ///     var result = RunTest( AnyValue.IsValid,
        ///                           () => mc.Value );
        /// 
        ///     Assert.AreEqual( 10, result );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static T RunTest<T>( Criteria cases, Expression<Func<T>> act, params Assertion[] assertions ) => RunTest( Case( cases ), new InvokeAct<T>( act ), assertions );


        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression in the Act part of the test.</typeparam>
        /// <param name="cases">The <see cref="SmartTests.Case" /> expression for the tested expression.</param>
        /// <param name="act">A lambda expression representing the tested expression.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <returns>The value of the expression of the <paramref name="act" />.</returns>
        /// <remarks>
        ///     If you only have one <see cref="Criteria" /> expression, you can use
        ///     <see cref="RunTest{T}(Criteria,Expression{Func{T}},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void Max_ValuesGreaterThanMin()
        /// {
        ///     var remainder;
        ///     var result = RunTest( Case( "a", AnyValue.IsValid ) &
        ///                           Case( "b", ValidValue.IsValid ),
        ///                           () => Math.DivRem( 5, 2, out remainder ) );
        /// 
        ///     Assert.AreEqual( 2, result );
        ///     Assert.AreEqual( 1, remainder );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" />
        /// <seealso cref="Case(Criteria)" />
        /// <seealso cref="Case(string,Criteria)" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static T RunTest<T>( Case cases, Expression<Func<T>> act, params Assertion[] assertions ) => RunTest( cases, new InvokeAct<T>( act ), assertions );


        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression in the Act part of the test.</typeparam>
        /// <param name="cases">The <see cref="Criteria" /> expression for the tested expression.</param>
        /// <param name="act">A <see cref="Act{T}" /> expression representing the tested expression.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <returns>The value of the expression of the <paramref name="act" />.</returns>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest{T}(SmartTests.Case,Act{T},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void MyPropertyTest()
        /// {
        ///     var mc = new MyClass( 10 );
        /// 
        ///     var result = RunTest( AnyValue.IsValid,
        ///                           Assign( () => mc.Value, 11 ) );
        /// 
        ///     Assert.AreEqual( 11, result );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest{T}(SmartTests.Case,Act{T},SmartTests.Assertion[])" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static T RunTest<T>( Criteria cases, [NotNull] Act<T> act, params Assertion[] assertions ) => RunTest( Case( cases ), act, assertions );


        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression in the Act part of the test.</typeparam>
        /// <param name="cases">The <see cref="SmartTests.Case" /> expression for the tested expression.</param>
        /// <param name="act">A <see cref="Act{T}" /> expression representing the tested expression.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <returns>The value of the expression of the <paramref name="act" />.</returns>
        /// <remarks>
        ///     If you only have one <see cref="Criteria" /> expression, you can use
        ///     <see cref="RunTest{T}(Criteria,Act{T},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void MyPropertyTest()
        /// {
        ///     var mc = new MyClass(10);
        /// 
        ///     RunTest( Case( "index", MinIncluded.IsMin ) &
        ///              Case( MinIncluded.IsAboveMin ),
        ///              Assign( () => mc.Values[ 0 ], 2 ) );
        /// 
        ///     Assert.AreEqual( 2, mc.Values[ 0 ] );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest{T}(SmartTests.Case,Act{T},SmartTests.Assertion[])" />
        /// <seealso cref="Case(Criteria)" />
        /// <seealso cref="Case(string,Criteria)" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static T RunTest<T>( Case cases, [NotNull] Act<T> act, params Assertion[] assertions )
        {
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            act.Assertions = assertions;
            T result;
            try
            {
                act.BeforeAct();
                result = act.Invoke();
            }
            catch( Exception e )
            {
                e = e.NoInvocation();
                act.Exception = e;
                act.AfterAct();
                if( e is SmartTestException )
                    // ReSharper disable once PossibleIntendedRethrow
                    throw e;
                throw new SmartTestException( "Unexpected error occurred!", e );
            }

            act.AfterAct();
            return result;
        }


        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <param name="cases">The <see cref="Criteria" /> expression for the tested code.</param>
        /// <param name="act">An <see cref="Action" /> representing the tested code.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void ReturnVoidTest()
        /// {
        ///     var mc = new MyClass( 10 );
        /// 
        ///     RunTest( MinIncluded.IsAboveMin,
        ///              () => mc.SetValue( 2 ) );
        /// 
        ///     Assert.AreEqual( 2, mc.Value );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" />
        /// <seealso cref="RunTest(SmartTests.Case,Act,SmartTests.Assertion[])" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static void RunTest( Criteria cases, Expression<Action> act, params Assertion[] assertions ) => RunTest( Case( cases ), new InvokeAct( act ), assertions );


        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <param name="cases">The <see cref="SmartTests.Case" /> expression for the tested code.</param>
        /// <param name="act">An <see cref="Action" /> expression representing the tested code.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <remarks>
        ///     If you only have one <see cref="Criteria" /> expression, you can use
        ///     <see cref="RunTest(Criteria,Expression{Action},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void ReturnVoidTest()
        /// {
        ///     var mc = new MyClass( 10 );
        /// 
        ///     RunTest( Case( "p1", MinIncluded.IsAboveMin ) &
        ///              Case( "p2", MinIncluded.IsAboveMin ),
        ///              () => mc.SetValues( 2, 3 ) );
        /// 
        ///     Assert.AreEqual( 2, mc.Value1 );
        ///     Assert.AreEqual( 3, mc.Value2 );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest{T}(SmartTests.Case,Act{T},SmartTests.Assertion[])" />
        /// <seealso cref="RunTest(SmartTests.Case,Act,SmartTests.Assertion[])" />
        /// <seealso cref="Case(Criteria)" />
        /// <seealso cref="Case(string,Criteria)" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static void RunTest( Case cases, Expression<Action> act, params Assertion[] assertions ) => RunTest( cases, new InvokeAct( act ), assertions );


        /// <summary>
        ///     Executes the Act part of the test and its related Smart Assertions.
        /// </summary>
        /// <param name="cases">The <see cref="SmartTests.Case" /> expression for the tested code.</param>
        /// <param name="act">An <see cref="Act" /> instance representing the tested code.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest(SmartTests.Case,Act,SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void ReturnVoidTest()
        /// {
        ///     var mc = new MyClass( 10 );
        /// 
        ///     RunTest( Case( "p1", MinIncluded.IsAboveMin ) &
        ///              Case( "p2", MinIncluded.IsAboveMin ),
        ///              () => mc.SetValues( 2, 3 ) );
        /// 
        ///     Assert.AreEqual( 2, mc.Value1 );
        ///     Assert.AreEqual( 3, mc.Value2 );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest(Criteria,Expression{Action},SmartTests.Assertion[])" />
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" />
        /// <seealso cref="Case(Criteria)" />
        /// <seealso cref="Case(string,Criteria)" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static void RunTest( Case cases, [NotNull] Act act, params Assertion[] assertions )
        {
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            act.Assertions = assertions;
            try
            {
                act.BeforeAct();
                act.Invoke();
            }
            catch( Exception e )
            {
                e = e.NoInvocation();
                act.Exception = e;
                act.AfterAct();
                if( e is SmartTestException )
                    // ReSharper disable once PossibleIntendedRethrow
                    throw e;
                throw new SmartTestException( "Unexpected error occurred!", e );
            }
            act.AfterAct();
        }

        #endregion


        #region SmartAssert Extensions

        /// <summary>
        ///     The simplest way to access Smart Assertions.
        /// </summary>
        /// <remarks>
        ///     Smart Assertions are instances of <see cref="Assertion" /> sub-classes, created from method factories as
        ///     extension methods.
        /// </remarks>
        /// <seealso cref="Assertion" />
        public static SmartAssertPlaceHolder SmartAssert { get; }

        #endregion
    }
}