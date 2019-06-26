using System;
using System.Linq;
using System.Linq.Expressions;

using SmartTests.Acts;
using SmartTests.Assertions;
using SmartTests.Helpers;
using SmartTests.Ranges;

// ReSharper disable UnusedMember.Global


namespace SmartTests
{
    /// <summary>
    ///     This is the main class for SmartTests.
    /// </summary>
    /// <remarks>
    ///     It is recommended to use it with <c>using static SmartTests.SmartTest</c>.
    /// </remarks>
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
        /// <seealso cref="Case{T}" />
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
        /// <seealso cref="Case{T}" />
        /// <seealso cref="SmartTests.Case" />
        public static Case Case( string parameterName, Criteria criteria ) => new Case( parameterName, criteria );


        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a specific parameter path <see cref="Criteria" />.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="path">An Expression of the path for the created <see cref="SmartTests.Case" />.</param>
        /// <param name="criteria">The <see cref="Criteria" /> for the created <see cref="SmartTests.Case" />.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <remarks>
        ///     <para>Warning: Not all lambda can be used.</para>
        ///     <para>It has to have 2 constraints:</para>
        ///     <list type="number">
        ///         <item>
        ///             <term>Valid Parameter Name</term>
        ///             <decription>
        ///                 The lambda should have 1 parameter whose name is the name of the parameter of the tested method
        ///                 you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Parameter Type</term>
        ///             <decription>
        ///                 The type of the parameter of the lambda should be the type of the parameter whose name is the
        ///                 name of the parameter of the tested method you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Path</term>
        ///             <decription>
        ///                 <para>
        ///                     The expression in the lambda must be a path starting with the lambda parameter and having has
        ///                     many properties/fields access.
        ///                 </para>
        ///                 <para>
        ///                     The idea is to specify properties (or sub-properties....) of the parameter that have an effect
        ///                     on the context of the test.
        ///                 </para>
        ///             </decription>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <code>
        ///  static class DateTimeHelper
        ///  {
        ///      public static bool IsWeekEnd(DateTime date)
        ///      {
        ///          return date.DayOfWeek == DayOfWeek.Saturday ||
        ///                 date.DayOfWeek == DayOfWeek.Sunday;
        ///      }
        ///  }
        /// 
        ///  ...
        /// 
        ///  private static DateTime GenerateDateOnWeekDay( DayOfWeek day )
        ///  {
        ///      var result = DateTime.Now;
        ///      return result.AddDays( day - result.DayOfWeek );
        ///  }
        /// 
        ///  [Test]
        ///  public void WeekEndTest()
        ///  {
        ///      // You will have a warning because you do not test other days of the week
        ///      var result = RunTest( Case( (DateTime date) => date.DayOfWeek,
        ///                                  SmartTest.Enum.Values( out value, DayOfWeek.Saturday, DayOfWeek.Sunday ) ),
        ///                            () => DateTimeHelper.IsWeekEnd( GenerateDateOnWeekDay( value ) ) );
        ///    
        ///      Assert.IsTrue( result );
        ///  }
        ///  </code>
        /// </example>
        /// <seealso cref="Case(SmartTests.Criteria)" />
        /// <seealso cref="Case(string,SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case Case<T>( Expression<Func<T, object>> path, Criteria criteria ) => new Case( path.Body.ToString(), criteria );


        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a specific parameter path.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <typeparam name="TParam">The type of the parameter of the method to test.</typeparam>
        /// <param name="path">An Expression of the path and the criteria for the parameter of the method to test.</param>
        /// <param name="value">One random value from the provided values in the equivalence class.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <remarks>
        ///     <para>Warning: Not all lambda can be used.</para>
        ///     <para>It has to have 2 constraints:</para>
        ///     <list type="number">
        ///         <item>
        ///             <term>Valid Parameter Name</term>
        ///             <decription>
        ///                 The lambda should have 1 parameter whose name is the name of the parameter of the tested method
        ///                 you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Parameter Type</term>
        ///             <decription>
        ///                 The type of the parameter of the lambda should be the type of the parameter whose name is the
        ///                 name of the parameter of the tested method you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Path</term>
        ///             <decription>
        ///                 <para>
        ///                     The expression in the lambda must be a path starting with the lambda parameter and having has
        ///                     many properties/fields access.
        ///                 </para>
        ///                 <para>
        ///                     The idea is to specify properties (or sub-properties....) of the parameter that have an effect
        ///                     on the context of the test.
        ///                 </para>
        ///             </decription>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <code>
        ///  static class IntHelper
        ///  {
        ///      public static double Inverse(int n) => 1 / n;
        ///  }
        /// 
        ///  ...
        /// 
        ///  [Test]
        ///  public void IntTest()
        ///  {
        ///      var result = RunTest( Case( (int n) => n.Below( 0 ).Above( 0 ), out var value ),
        ///                            () => IntHelper.Inverse( value ) );
        ///    
        ///      Assert.AreEqual( 1 / value, result );
        ///  }
        ///  </code>
        /// </example>
        /// <seealso
        ///     cref="ErrorCase{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.INumericType{T}}},out T)" />
        /// <seealso
        ///     cref="Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.EnumTypeHelper.PlaceHolder{T}}},out T)" />
        /// <seealso cref="Case{T}" />
        /// <seealso cref="Case(SmartTests.Criteria)" />
        /// <seealso cref="Case(string,SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case Case<TParam, T>( Expression<Func<TParam, INumericType<T>>> path, out T value )
            where T: IComparable<T>
        {
            var range = ExtractNameAndRange( path.Body, out var name );
            if( range == null )
            {
                value = default(T);
                return null;
            }

            var evaluatedRange = (INumericType<T>)Expression.Lambda( range ).Compile().DynamicInvoke();
            return new Case( name, evaluatedRange.GetValidValue( out value ) );
        }


        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a specific parameter path.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <typeparam name="TParam">The type of the parameter of the method to test.</typeparam>
        /// <param name="path">An Expression of the path and the criteria for the parameter of the method to test.</param>
        /// <param name="value">One random value from the provided values in the equivalence class.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <remarks>
        ///     <para>Warning: Not all lambda can be used.</para>
        ///     <para>It has to have 2 constraints:</para>
        ///     <list type="number">
        ///         <item>
        ///             <term>Valid Parameter Name</term>
        ///             <decription>
        ///                 The lambda should have 1 parameter whose name is the name of the parameter of the tested method
        ///                 you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Parameter Type</term>
        ///             <decription>
        ///                 The type of the parameter of the lambda should be the type of the parameter whose name is the
        ///                 name of the parameter of the tested method you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Path</term>
        ///             <decription>
        ///                 <para>
        ///                     The expression in the lambda must be a path starting with the lambda parameter and having has
        ///                     many properties/fields access.
        ///                 </para>
        ///                 <para>
        ///                     The idea is to specify properties (or sub-properties....) of the parameter that have an effect
        ///                     on the context of the test.
        ///                 </para>
        ///             </decription>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <code>
        ///  static class IntHelper
        ///  {
        ///      public static double Inverse(int n) => 1 / n;
        ///  }
        /// 
        ///  ...
        /// 
        ///  [Test]
        ///  public void IntTest()
        ///  {
        ///      Assert.Throws&lt;DivideByZeroException&gt;, () => RunTest( ErrorCase( (int n) => n.Value( 0 ), out var value ),
        ///                                                                      () => IntHelper.Inverse( value ) ));
        ///  }
        ///  </code>
        /// </example>
        /// <seealso
        ///     cref="Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.INumericType{T}}},out T)" />
        /// <seealso
        ///     cref="Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.EnumTypeHelper.PlaceHolder{T}}},out T)" />
        /// <seealso cref="Case{T}" />
        /// <seealso cref="Case(SmartTests.Criteria)" />
        /// <seealso cref="Case(string,SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case ErrorCase<TParam, T>( Expression<Func<TParam, INumericType<T>>> path, out T value )
            where T: IComparable<T>
        {
            var range = ExtractNameAndRange( path.Body, out var name );
            if( range == null )
            {
                value = default(T);
                return null;
            }

            var evaluatedRange = (INumericType<T>)Expression.Lambda( range ).Compile().DynamicInvoke();
            return new Case( name, evaluatedRange.GetErrorValue( out value ) );
        }


        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a specific parameter path.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <typeparam name="TParam">The type of the parameter of the method to test.</typeparam>
        /// <param name="path">An Expression of the path and the criteria for the parameter of the method to test.</param>
        /// <param name="value">One random value from the provided values in the equivalence class.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <remarks>
        ///     <para>Warning: Not all lambda can be used.</para>
        ///     <para>It has to have 2 constraints:</para>
        ///     <list type="number">
        ///         <item>
        ///             <term>Valid Parameter Name</term>
        ///             <decription>
        ///                 The lambda should have 1 parameter whose name is the name of the parameter of the tested method
        ///                 you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Parameter Type</term>
        ///             <decription>
        ///                 The type of the parameter of the lambda should be the type of the parameter whose name is the
        ///                 name of the parameter of the tested method you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Path</term>
        ///             <decription>
        ///                 <para>
        ///                     The expression in the lambda must be a path starting with the lambda parameter and having has
        ///                     many properties/fields access.
        ///                 </para>
        ///                 <para>
        ///                     The idea is to specify properties (or sub-properties....) of the parameter that have an effect
        ///                     on the context of the test.
        ///                 </para>
        ///             </decription>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <code>
        ///  static class DateTimeHelper
        ///  {
        ///      public static bool IsWeekEnd(DateTime date)
        ///      {
        ///          return date.DayOfWeek == DayOfWeek.Saturday ||
        ///                 date.DayOfWeek == DayOfWeek.Sunday;
        ///      }
        ///  }
        /// 
        ///  ...
        /// 
        ///  private static DateTime GenerateDateOnWeekDay( DayOfWeek day )
        ///  {
        ///      var result = DateTime.Now;
        ///      return result.AddDays( day - result.DayOfWeek );
        ///  }
        /// 
        ///  [Test]
        ///  public void WeekEndTest()
        ///  {
        ///      // You will have a warning because you do not test other days of the week
        ///      var result = RunTest( Case( (DateTime date) => date.DayOfWeek.Values( DayOfWeek.Saturday, DayOfWeek.Sunday ), out var value ),
        ///                            () => DateTimeHelper.IsWeekEnd( GenerateDateOnWeekDay( value ) ) );
        ///    
        ///      Assert.IsTrue( result );
        ///  }
        ///  </code>
        /// </example>
        /// <seealso
        ///     cref="ErrorCase{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.EnumTypeHelper.PlaceHolder{T}}},out T)" />
        /// <seealso
        ///     cref="Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.EnumTypeHelper.PlaceHolder{T}}},out T)" />
        /// <seealso cref="Case{T}" />
        /// <seealso cref="Case(SmartTests.Criteria)" />
        /// <seealso cref="Case(string,SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case Case<TParam, T>( Expression<Func<TParam, EnumTypeHelper.PlaceHolder<T>>> path, out T value )
            where T: struct, IComparable
        {
            var range = ExtractNameAndValues( path.Body, out var name );
            if( range == null )
            {
                value = default(T);
                return null;
            }

            var typedRange = range.Cast<T>().ToArray();
            var enumType = new EnumType();
            return new Case( name, enumType.Values( out value, typedRange[ 0 ], typedRange.Skip( 1 ).ToArray() ) );
        }


        /// <summary>
        ///     Creates an instance of <see cref="SmartTests.Case" /> class for a specific parameter path to manage errors.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <typeparam name="TParam">The type of the parameter of the method to test.</typeparam>
        /// <param name="path">An Expression of the path and the criteria for the parameter of the method to test.</param>
        /// <param name="value">One random value from the provided values in the equivalence class.</param>
        /// <returns>The newly created <see cref="SmartTests.Case" />.</returns>
        /// <remarks>
        ///     <para>Warning: Not all lambda can be used.</para>
        ///     <para>It has to have 2 constraints:</para>
        ///     <list type="number">
        ///         <item>
        ///             <term>Valid Parameter Name</term>
        ///             <decription>
        ///                 The lambda should have 1 parameter whose name is the name of the parameter of the tested method
        ///                 you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Parameter Type</term>
        ///             <decription>
        ///                 The type of the parameter of the lambda should be the type of the parameter whose name is the
        ///                 name of the parameter of the tested method you want to describe the <see cref="Criteria" />.
        ///             </decription>
        ///         </item>
        ///         <item>
        ///             <term>Valid Path</term>
        ///             <decription>
        ///                 <para>
        ///                     The expression in the lambda must be a path starting with the lambda parameter and having has
        ///                     many properties/fields access.
        ///                 </para>
        ///                 <para>
        ///                     The idea is to specify properties (or sub-properties....) of the parameter that have an effect
        ///                     on the context of the test.
        ///                 </para>
        ///             </decription>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <example>
        ///     <code>
        ///  static class DateTimeHelper
        ///  {
        ///      public static bool IsWeekEnd(DateTime date)
        ///      {
        ///          return date.DayOfWeek == DayOfWeek.Saturday ||
        ///                 date.DayOfWeek == DayOfWeek.Sunday;
        ///      }
        ///  }
        /// 
        ///  ...
        /// 
        ///  private static DateTime GenerateDateOnWeekDay( DayOfWeek day )
        ///  {
        ///      var result = DateTime.Now;
        ///      return result.AddDays( day - result.DayOfWeek );
        ///  }
        /// 
        ///  [Test]
        ///  public void WeekEndTest()
        ///  {
        ///      // You will have a warning because you do not test other days of the week
        ///      var result = RunTest( ErrorCase( (DateTime date) => date.DayOfWeek.Values( DayOfWeek.Saturday, DayOfWeek.Sunday ), out var value ),
        ///                            () => DateTimeHelper.IsWeekEnd( GenerateDateOnWeekDay( value ) ) );
        ///    
        ///      Assert.IsTrue( result );
        ///  }
        ///  </code>
        /// </example>
        /// <seealso
        ///     cref="Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.EnumTypeHelper.PlaceHolder{T}}},out T)" />
        /// <seealso
        ///     cref="Case{TParam,T}(System.Linq.Expressions.Expression{System.Func{TParam,SmartTests.Ranges.INumericType{T}}},out T)" />
        /// <seealso cref="Case{T}" />
        /// <seealso cref="Case(SmartTests.Criteria)" />
        /// <seealso cref="Case(string,SmartTests.Criteria)" />
        /// <seealso cref="SmartTests.Case" />
        public static Case ErrorCase<TParam, T>( Expression<Func<TParam, EnumTypeHelper.PlaceHolder<T>>> path, out T value )
            where T: struct, IComparable
        {
            var range = ExtractNameAndValues( path.Body, out var name );
            if( range == null )
            {
                value = default(T);
                return null;
            }

            var typedRange = range.Cast<T>().ToArray();
            var enumType = new EnumType();
            return new Case( name, enumType.ErrorValues( out value, typedRange[ 0 ], typedRange.Skip( 1 ).ToArray() ) );
        }


        private static Expression ExtractNameAndRange( Expression expression, out string name )
        {
            if( expression is MethodCallExpression methodCall )
                return ExtractNameAndRange( methodCall, out name );

            name = null;
            return null;
        }


        private static MethodCallExpression ExtractNameAndRange( MethodCallExpression expression, out string name )
        {
            if( expression.Object != null )
                return expression.Update( ExtractNameAndRange( expression.Object, out name ), expression.Arguments );

            if( expression.Arguments[ 0 ].NodeType == ExpressionType.MemberAccess ||
                expression.Arguments[ 0 ].NodeType == ExpressionType.Parameter )
            {
                // First Argument is "this"
                name = expression.Arguments[ 0 ].ToString();
                var arguments = expression.Arguments.ToList();
                arguments[ 0 ] = Expression.Default( arguments[ 0 ].Type );
                return expression.Update( expression.Object, arguments );
            }

            name = null;
            return null;
        }


        private static object[] ExtractNameAndValues( Expression expression, out string name )
        {
            name = null;
            return expression is MethodCallExpression methodCall
                       ? ExtractNameAndValues( methodCall, out name )
                       : null;
        }


        private static object[] ExtractNameAndValues( MethodCallExpression expression, out string name )
        {
            if( expression.Object != null )
                return ExtractNameAndValues( expression.Object, out name );

            if( expression.Arguments[ 0 ].NodeType == ExpressionType.MemberAccess ||
                expression.Arguments[ 0 ].NodeType == ExpressionType.Parameter )
            {
                // First Argument is "this"
                name = expression.Arguments[ 0 ].ToString();
                if( !( expression.Arguments[ 1 ] is NewArrayExpression arrayInit ) )
                    return null;
                var otherArguments = arrayInit.Expressions;
                if( otherArguments.Any( arg => arg.NodeType != ExpressionType.Constant ) )
                {
                    name = null;
                    return null;
                }

                return otherArguments.Select( arg => ( (ConstantExpression)arg ).Value ).ToArray();
            }

            name = null;
            return null;
        }

        #endregion


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
        public static T RunTest<T>( Criteria cases, Act<T> act, params Assertion[] assertions ) => RunTest( Case( cases ), act, assertions );


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
        public static T RunTest<T>( Case cases, Act<T> act, params Assertion[] assertions )
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
        public static void RunTest( Case cases, Act act, params Assertion[] assertions )
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
        ///     Creates a new range of <c>byte</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="ByteRange" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use ByteRange, so that all ranges are named the same way" )]
        public static INumericType<byte> Byte => new ByteType();
        /// <summary>
        ///     Creates a new range of <c>byte</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<byte> ByteRange => new ByteType();
        /// <summary>
        ///     Creates a new range of <c>sbyte</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="SByteRange" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use SByteRange, so that all ranges are named the same way" )]
        public static INumericType<sbyte> SByte => new SByteType();
        /// <summary>
        ///     Creates a new range of <c>sbyte</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<sbyte> SByteRange => new SByteType();
        /// <summary>
        ///     Creates a new range of <c>short</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="Int16Range" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use Int16Range, so that all ranges are named the same way" )]
        public static INumericType<short> Short => new Int16Type();
        /// <summary>
        ///     Creates a new range of <c>short</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<short> Int16Range => new Int16Type();
        /// <summary>
        ///     Creates a new range of <c>ushort</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="UInt16Range" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use UInt16Range, so that all ranges are named the same way" )]
        public static INumericType<ushort> UShort => new UInt16Type();
        /// <summary>
        ///     Creates a new range of <c>ushort</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<ushort> UInt16Range => new UInt16Type();
        /// <summary>
        ///     Creates a new range of <c>int</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="Int32Range" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use Int32Range, so that all ranges are named the same way" )]
        public static INumericType<int> Int => new Int32Type();
        /// <summary>
        ///     Creates a new range of <c>int</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<int> Int32Range => new Int32Type();
        /// <summary>
        ///     Creates a new range of <c>uint</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="UInt32Range" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use UInt32Range, so that all ranges are named the same way" )]
        public static INumericType<uint> UInt => new UInt32Type();
        /// <summary>
        ///     Creates a new range of <c>uint</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<uint> UInt32Range => new UInt32Type();
        /// <summary>
        ///     Creates a new range of <c>long</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="Int64Range" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use Int64Range, so that all ranges are named the same way" )]
        public static INumericType<long> Long => new Int64Type();
        /// <summary>
        ///     Creates a new range of <c>long</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<long> Int64Range => new Int64Type();
        /// <summary>
        ///     Creates a new range of <c>ulong</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="UInt64Range" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use UInt64Range, so that all ranges are named the same way" )]
        public static INumericType<ulong> ULong => new UInt64Type();
        /// <summary>
        ///     Creates a new range of <c>ulong</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<ulong> UInt64Range => new UInt64Type();
        /// <summary>
        ///     Creates a new range of <c>float</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="SingleRange" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use SingleRange, so that all ranges are named the same way" )]
        public static INumericType<float> Float => new SingleType();
        /// <summary>
        ///     Creates a new range of <c>float</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<float> SingleRange => new SingleType();
        /// <summary>
        ///     Creates a new range of <c>double</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="DoubleRange" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use DoubleRange, so that there is no conflict with System.Double" )]
        public static INumericType<double> Double => new DoubleType();
        /// <summary>
        ///     Creates a new range of <c>double</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<double> DoubleRange => new DoubleType();
        /// <summary>
        ///     Creates a new range of <c>decimal</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<decimal> DecimalRange => new DecimalType();
        /// <summary>
        ///     Creates a new range of <c>DateTime</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static INumericType<DateTime> DateTimeRange => new DateTimeType();
        /// <summary>
        ///     Creates a new range of <c>Enum</c>
        /// </summary>
        /// <remarks>
        ///     It is obsolete now. You should use <see cref="EnumRange" /> instead.
        /// </remarks>
        /// <seealso cref="INumericType{T}" />
        [Obsolete( "Use EnumRange, so that there is no conflict with System.Enum" )]
        public static EnumType Enum => new EnumType();
        /// <summary>
        ///     Creates a new range of <c>Enum</c>
        /// </summary>
        /// <seealso cref="INumericType{T}" />
        public static EnumType EnumRange => new EnumType();


        /// <summary>
        ///     Convert a value as a string
        /// </summary>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The converted value as a string.</returns>
        public static string ToString( object value )
        {
            if( value is byte bt )
                return ToString( bt );
            if( value is DateTime dtTm )
                return ToString( dtTm );
            if( value is decimal dcm )
                return ToString( dcm );
            if( value is double dbl )
                return ToString( dbl );
            if( value is float flt )
                return ToString( flt );
            if( value is int nt )
                return ToString( nt );
            if( value is long lng )
                return ToString( lng );
            if( value is sbyte sbt )
                return ToString( sbt );
            if( value is uint unt )
                return ToString( unt );
            if( value is ulong ulg )
                return ToString( ulg );
            if( value is ushort ush )
                return ToString( value );

            throw new NotSupportedException( $"Type {value.GetType().FullName} not supported yet!" );
        }


        internal static string ToString( byte value ) => value == byte.MaxValue ? "byte.MaxValue" : value.ToString();


        internal static string ToString( decimal value )
        {
            if( value == decimal.MinValue )
                return "decimal.MinValue";
            if( value == decimal.MaxValue )
                return "decimal.MaxValue";

            return value.ToString();
        }


        internal static string ToString( DateTime value )
        {
            if( value == DateTime.MinValue )
                return "DateTime.MinValue";
            if( value == DateTime.MaxValue )
                return "DateTime.MaxValue";
            return "new DateTime(" +
                   ( value == value.Date
                         ? value.ToString( "yyyy, M, d" )
                         : value.Millisecond == 0
                             ? value.ToString( "yyyy, M, d, H, m, s" )
                             : value.ToString( "yyyy, M, d, H, m, s, f" ) ) +
                   ")";
        }


        internal static string ToString( double value )
        {
            if( value == double.MinValue )
                return "double.MinValue";
            if( value == double.MaxValue )
                return "double.MaxValue";

            return value.ToString();
        }


        internal static string ToString( float value )
        {
            if( value == float.MinValue )
                return "float.MinValue";
            if( value == float.MaxValue )
                return "float.MaxValue";

            return value.ToString();
        }


        internal static string ToString( int value )
        {
            if( value == int.MinValue )
                return "int.MinValue";
            if( value == int.MaxValue )
                return "int.MaxValue";
            return value.ToString();
        }


        internal static string ToString( long value )
        {
            if( value == long.MinValue )
                return "long.MinValue";
            if( value == long.MaxValue )
                return "long.MaxValue";
            return value.ToString();
        }


        internal static string ToString( sbyte value )
        {
            if( value == sbyte.MinValue )
                return "sbyte.MinValue";
            if( value == sbyte.MaxValue )
                return "sbyte.MaxValue";
            return value.ToString();
        }


        internal static string ToString( short value )
        {
            if( value == short.MinValue )
                return "short.MinValue";
            if( value == short.MaxValue )
                return "short.MaxValue";
            return value.ToString();
        }


        internal static string ToString( uint value )
        {
            if( value == uint.MaxValue )
                return "uint.MaxValue";
            return value.ToString();
        }


        internal static string ToString( ulong value )
        {
            if( value == ulong.MaxValue )
                return "ulong.MaxValue";
            return value.ToString();
        }


        internal static string ToString( ushort value )
        {
            if( value == ushort.MaxValue )
                return "ushort.MaxValue";
            return value.ToString();
        }

        #endregion
    }
}