using System;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

using SmartTests.Acts;
using SmartTests.Assertions;
using SmartTests.Helpers;
using SmartTests.Ranges;



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
        /// <example>
        ///     <code>
        /// [Test]
        /// public void Max_ValuesGreaterThanMin()
        /// {
        ///     var remainder;
        ///     var result = RunTest( Case( "a", AnyValue.IsValid ) &amp;
        ///                           Case( "b", ValidValue.IsValid ),
        ///                           () => Math.DivRem( 5, 2, out remainder ) );
        /// 
        ///     Assert.AreEqual( 2, result );
        ///     Assert.AreEqual( 1, remainder );
        /// }
        /// </code>
        /// </example>
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
        ///     Executes the Act part of the test, and its related Smart Assertions, that involves implicit declarations for
        ///     Assertions.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression in the Act part of the test.</typeparam>
        /// <param name="cases">The <see cref="Criteria" /> expression for the tested expression.</param>
        /// <param name="act">A lambda expression representing the tested expression with an <see cref="ActContext" />.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <returns>The value of the expression of the <paramref name="act" />.</returns>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest(SmartTests.Case,Expression{Action{ActContext}},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the
        ///         <see cref="WaitAssertions.WaitContextHandle(SmartTests.SmartAssertPlaceHolder,double)"></see> wait for the
        ///         implicit wait handle set by <see cref="WaitAssertions.SetHandle" />.
        ///     </para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///   var mc = new MyClass( 300 );
        ///   
        ///   RunTest( AnyValue.IsValid,
        ///            ctx => mc.Method( ctx.SetHandle ),
        ///            SmartAssert.Within( 100 ),
        ///            SmartAssert.WaitContextHandle( 1000 ) );
        ///   
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
        /// }</code>
        /// </example>
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action{ActContext}},SmartTests.Assertion[])" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static T RunTest<T>( Criteria cases, Expression<Func<ActContext, T>> act, params Assertion[] assertions ) => RunTest( Case( cases ), new InvokeAct<T>( act ), assertions );


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
        ///     var result = RunTest( Case( "a", AnyValue.IsValid ) &amp;
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
        ///     Executes the Act part of the test, and its related Smart Assertions, that involves implicit declarations for
        ///     Assertions.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the expression in the Act part of the test.</typeparam>
        /// <param name="cases">The <see cref="Criteria" /> expression for the tested expression.</param>
        /// <param name="act">A lambda expression representing the tested expression with an <see cref="ActContext" />.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <returns>The value of the expression of the <paramref name="act" />.</returns>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest{T}(SmartTests.Case,Expression{Func{ActContext,T}},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <para>
        ///         In this example, the
        ///         <see cref="WaitAssertions.WaitContextHandle(SmartTests.SmartAssertPlaceHolder,double)"></see> wait for the
        ///         implicit wait handle set by <see cref="WaitAssertions.SetHandle" />.
        ///     </para>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///   var mc = new MyClass( 300 );
        ///   
        ///   var result = RunTest( AnyValue.IsValid,
        ///                         ctx => mc.Method( ctx.SetHandle ),
        ///                         SmartAssert.Within( 100 ),
        ///                         SmartAssert.WaitContextHandle( 1000 ) );
        ///   
        ///   Assert.IsTrue( result );
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
        /// }</code>
        /// </example>
        /// <seealso cref="RunTest{T}(SmartTests.Case,Expression{Func{ActContext,T}},SmartTests.Assertion[])" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static T RunTest<T>( Case cases, Expression<Func<ActContext, T>> act, params Assertion[] assertions ) => RunTest( cases, new InvokeAct<T>( act ), assertions );


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
        ///     var mc = new MyClass( 10 );
        /// 
        ///     RunTest( Case( "index", MinIncluded.IsMin ) &amp;
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
                result = act.Invoke( act.Context );
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
        /// <param name="cases">The <see cref="Criteria" /> expression for the tested code.</param>
        /// <param name="act">An <see cref="Action" /> representing the tested code with an <see cref="ActContext" />.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <remarks>
        ///     If you need to specify <see cref="Criteria" /> expressions for multiple parameters, you have to use
        ///     <see cref="RunTest(SmartTests.Case,Expression{Action{ActContext}},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void MyMethodTest()
        /// {
        ///   var mc = new MyClass( 300 );
        ///   
        ///   RunTest( AnyValue.IsValid,
        ///            ctx => mc.Method( ctx.SetHandle ),
        ///            SmartAssert.Within( 100 ),
        ///            SmartAssert.WaitContextHandle( 1000 ) );
        ///   
        ///   Assert.IsTrue( result );
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action{ActContext}},SmartTests.Assertion[])" />
        /// <seealso cref="RunTest(SmartTests.Case,Expression{Action},SmartTests.Assertion[])" />
        /// <seealso cref="RunTest(SmartTests.Case,Act,SmartTests.Assertion[])" />
        /// <seealso cref="Criteria" />
        /// <seealso cref="Assertion" />
        /// <exception cref="SmartTestException">In case a Smart <see cref="Assertion" /> fails after the <paramref name="act" />.</exception>
        /// <exception cref="BadTestException">In case a Smart <see cref="Assertion" /> fails before the <paramref name="act" />.</exception>
        public static void RunTest( Criteria cases, Expression<Action<ActContext>> act, params Assertion[] assertions ) => RunTest( Case( cases ), new InvokeAct( act ), assertions );


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
        ///     RunTest( Case( "p1", MinIncluded.IsAboveMin ) &amp;
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
        /// <param name="act">A lambda expression representing the tested expression with an <see cref="ActContext" />.</param>
        /// <param name="assertions">The Smart Assertions for this <paramref name="act" />.</param>
        /// <remarks>
        ///     If you only have one <see cref="Criteria" /> expression, you can use
        ///     <see cref="RunTest(Criteria,Expression{Action{ActContext}},SmartTests.Assertion[])" /> instead.
        /// </remarks>
        /// <example>
        ///     <code>
        /// [Test]
        /// public void ReturnVoidTest()
        /// {
        ///   var mc = new MyClass( 300 );
        /// 
        ///   RunTest( Case( AnyValue.IsValid ) ,
        ///            ctx => mc.Method( ctx.SetHandle ),
        ///            SmartAssert.Within( 100 ),
        ///            SmartAssert.WaitContextHandle( 1000 ) );
        ///   
        ///   Assert.IsTrue( result );
        ///   Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        ///   Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
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
        public static void RunTest( Case cases, Expression<Action<ActContext>> act, params Assertion[] assertions ) => RunTest( cases, new InvokeAct( act ), assertions );


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
        ///     RunTest( Case( "p1", MinIncluded.IsAboveMin ) &amp;
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
                act.Invoke( act.Context );
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


        #region Type Roots

        /// <summary>
        ///     Creates a new range of <c>int</c>
        /// </summary>
        /// <seealso cref="IType{T}" />
        public static IType<int> Int => new IntType();
        public static IType<long> Long => new LongType();

        #endregion


        #region Type Extensions Methods

        /// <summary>
        ///     Creates a range of integer values and select one randomly
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="min">The min value (included) of the range.</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <param name="value">A random value between <paramref name="min" /> and <paramref name="max" />.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        public static Criteria Range<T>( this IType<T> @this, T min, T max, out T value )
            where T: IComparable<T>
            => @this.Range( min, max ).GetValue( out value );


        /// <summary>
        ///     Creates a range of integer values to max integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="min">The min value (included) of the range.</param>
        /// <returns>The created <see cref="IntType" />: [<paramref name="min" />..<c>int.MaxValue</c>].</returns>
        public static IType<T> AboveOrEqual<T>( this IType<T> @this, T min )
            where T: IComparable<T>
            => @this.Range( min, @this.MaxValue );


        /// <summary>
        ///     Creates a range of integer values to max integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="min">The min value (included) of the range.</param>
        /// <param name="value">A random value between <paramref name="min" /> and <c>int.MaxValue</c>.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        public static Criteria AboveOrEqual<T>( this IType<T> @this, T min, out T value )
            where T: IComparable<T>
            => @this.Range( min, @this.MaxValue, out value );


        /// <summary>
        ///     Creates a range of integer values to max integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="min">The min value (included) of the range.</param>
        /// <returns>The created <see cref="IntType" />: [<paramref name="min" /><c>+1</c>..<c>int.MaxValue</c>].</returns>
        public static IType<T> Above<T>( this IType<T> @this, T min )
            where T: IComparable<T>
            => @this.Range( @this.GetNext( min ), @this.MaxValue );


        /// <summary>
        ///     Creates a range of integer values to max integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="min">The min value (included) of the range.</param>
        /// <param name="value">A random value between <paramref name="min" /><c>+1</c> and <c>int.MaxValue</c>.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        public static Criteria Above<T>( this IType<T> @this, T min, out T value )
            where T: IComparable<T>
            => @this.Range( @this.GetNext( min ), @this.MaxValue, out value );


        /// <summary>
        ///     Creates a range of integer values from min integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>The created <see cref="IntType" />: [<c>int.MinValue</c>..<paramref name="max" />].</returns>
        public static IType<T> BelowOrEqual<T>( this IType<T> @this, T max )
            where T: IComparable<T>
            => @this.Range( @this.MinValue, max );


        /// <summary>
        ///     Creates a range of integer values from min integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <param name="value">A random value between <c>int.MinValue</c> and <paramref name="max" />.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        public static Criteria BelowOrEqual<T>( this IType<T> @this, T max, out T value )
            where T: IComparable<T>
            => @this.Range( @this.MinValue, max, out value );


        /// <summary>
        ///     Creates a range of integer values from min integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <returns>The created <see cref="IntType" />: [<c>int.MinValue</c>..<paramref name="max" />-1].</returns>
        public static IType<T> Below<T>( this IType<T> @this, T max )
            where T: IComparable<T>
            => @this.Range( @this.MinValue, @this.GetPrevious( max ) );


        /// <summary>
        ///     Creates a range of integer values from min integer
        /// </summary>
        /// <param name="this">The range to which a new chunk must be added</param>
        /// <param name="max">The max value (included) of the range.</param>
        /// <param name="value">A random value between <c>int.MinValue</c> and <paramref name="max" /><c>-1</c>.</param>
        /// <returns>Any <see cref="Criteria" /> so that it can be used everywhere a criteria is expected.</returns>
        public static Criteria Below<T>( this IType<T> @this, T max, out T value )
            where T: IComparable<T>
            => @this.Range( @this.MinValue, @this.GetPrevious( max ), out value );

        #endregion
    }
}