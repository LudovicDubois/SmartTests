using System;
using System.Linq.Expressions;
using System.Reflection;

using SmartTests.Helpers;



namespace SmartTests.Acts
{
    /// <summary>
    ///     This class represents the Act of invoking a method that does not return anything (<c>void</c>).
    /// </summary>
    /// <remarks>
    ///     <para>DO NOT USE DIRECTLY.</para>
    ///     <para>Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.</para>
    /// </remarks>
    /// <seealso cref="SmartTest" />
    public class InvokeAct: Act
    {
        /// <summary>
        ///     Creates an instance of <see cref="InvokeAct" /> to represent a specific <c>void</c> method invocation in the Act
        ///     part of your test.
        /// </summary>
        /// <param name="invocation"> The invocation expression of the Act part of your test.</param>
        /// <remarks>
        ///     <para>DO NOT USE DIRECTLY.</para>
        ///     <para>
        ///         Prefer using
        ///         <see
        ///             cref="SmartTest.RunTest(Criteria,System.Linq.Expressions.Expression{Action},Assertion[])" />
        ///         or
        ///         <see cref="SmartTest.RunTest(Case,System.Linq.Expressions.Expression{Action},Assertion[])" />
        ///         methods.
        ///     </para>
        /// </remarks>
        /// <seealso cref="SmartTest" />
        public InvokeAct( Expression<Action> invocation )
        {
            FillAct( invocation );

            var action = invocation.Compile();
            _Invocation = ctx => action();
        }


        /// <summary>
        ///     Creates an instance of <see cref="InvokeAct" /> to represent a specific <c>void</c> method invocation in the Act
        ///     part of your test while using the <see cref="ActContext" />.
        /// </summary>
        /// <param name="invocation"> The invocation expression of the Act part of your test.</param>
        /// <remarks>
        ///     <para>DO NOT USE DIRECTLY.</para>
        ///     <para>
        ///         Prefer using
        ///         <see
        ///             cref="SmartTest.RunTest(Criteria,System.Linq.Expressions.Expression{Action{ActContext}},Assertion[])" />
        ///         or
        ///         <see cref="SmartTest.RunTest(Case,System.Linq.Expressions.Expression{Action{ActContext}},Assertion[])" />
        ///         methods.
        ///     </para>
        /// </remarks>
        /// <seealso cref="SmartTest" />
        public InvokeAct( Expression<Action<ActContext>> invocation )
        {
            FillAct( invocation );

            _Invocation = invocation.Compile();
        }


        private void FillAct( LambdaExpression invocation )
        {
            if( invocation.GetMemberContext( out object instance, out MemberInfo member ) )
            {
                Instance = instance;
                Constructor = member as ConstructorInfo;
                Method = member as MethodInfo;
            }

            if( Method == null )
                throw SmartTest.InconclusiveException();
        }


        private readonly Action<ActContext> _Invocation;


        /// <inheritdoc />
        public override void Invoke( ActContext context ) => _Invocation( context );
    }


    /// <summary>
    ///     This class represents the Act of invoking an expression.
    /// </summary>
    /// <remarks>
    ///     <para>DO NOT USE DIRECTLY.</para>
    ///     <para>Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.</para>
    /// </remarks>
    /// <typeparam name="T"> The result <see cref="Type" /> of the invocation involved in the Act part of your test.</typeparam>
    /// <seealso cref="SmartTest" />
    public class InvokeAct<T>: Act<T>
    {
        /// <summary>
        ///     Creates an instance of <see cref="InvokeAct{T}" /> to represent an expression in the Act part of your test.
        /// </summary>
        /// <param name="invocation">The invocation expression of the Act part of your test.</param>
        /// <remarks>
        ///     <para>DO NOT USE DIRECTLY.</para>
        ///     <para>
        ///         Prefer using
        ///         <see cref="SmartTest.RunTest{T}(Criteria,Expression{Func{T}},Assertion[])" />,
        ///         <see cref="SmartTest.RunTest{T}(Case,Expression{Func{T}},Assertion[])" /> methods.
        ///     </para>
        /// </remarks>
        /// <seealso cref="SmartTest" />
        public InvokeAct( Expression<Func<T>> invocation )
        {
            FillAct( invocation );

            var compiled = invocation.Compile();
            _Invocation = ctx => compiled();
        }


        /// <summary>
        ///     Creates an instance of <see cref="InvokeAct{T}" /> to represent an expression in the Act part of your test while
        ///     using the <see cref="ActContext" />.
        /// </summary>
        /// <param name="invocation">The invocation expression of the Act part of your test.</param>
        /// <remarks>
        ///     <para>DO NOT USE DIRECTLY.</para>
        ///     <para>
        ///         Prefer using
        ///         <see cref="SmartTest.RunTest{T}(Criteria,Expression{Func{ActContext,T}},Assertion[])" />,
        ///         <see cref="SmartTest.RunTest{T}(Case,Expression{Func{ActContext,T}},Assertion[])" /> methods.
        ///     </para>
        /// </remarks>
        /// <seealso cref="SmartTest" />
        public InvokeAct( Expression<Func<ActContext, T>> invocation )
        {
            FillAct( invocation );

            _Invocation = invocation.Compile();
        }


        private void FillAct( LambdaExpression invocation )
        {
            if( invocation.GetMemberContext( out object instance, out MemberInfo member ) )
            {
                Instance = instance;

                Constructor = member as ConstructorInfo;
                if( Constructor != null )
                    return;

                Method = member as MethodInfo;
                if( Method == null )
                {
                    Property = member as PropertyInfo;
                    if( Property != null )
                        Method = Property.GetMethod;
                    else
                        Field = member as FieldInfo;
                }
                else if( Method.IsSpecialName )
                    // An indexer
                    foreach( var property in Method.DeclaringType.GetRuntimeProperties() )
                        if( Equals( property.GetMethod, Method ) )
                        {
                            Property = property;
                            break;
                        }
            }

            if( Constructor == null && Method == null && Field == null )
                throw SmartTest.InconclusiveException( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() );
        }


        private readonly Func<ActContext, T> _Invocation;


        /// <inheritdoc />
        public override T Invoke( ActContext context ) => _Invocation( context );
    }
}