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
            _Invocation = invocation;

            object instance;
            MemberInfo member;
            if( invocation.GetMemberContext( out instance, out member ) )
            {
                Instance = instance;
                Constructor = member as ConstructorInfo;
                Method = member as MethodInfo;
            }
            if( Method == null )
                throw new SmartTestException();
        }


        private readonly Expression<Action> _Invocation;


        /// <inheritdoc />
        public override void Invoke() => _Invocation.Compile().Invoke();
    }


    /// <summary>
    ///     This class represents the Act of invoking an expression.
    /// </summary>
    /// <remarks>
    ///     <para>DO NOT USE DIRECTLY.</para>
    ///     <para>Prefer using <see cref="O:SmartTests.SmartTest.RunTest" /> methods.</para>
    /// </remarks>
    /// <typeparam name="T"> The result <see cref="Type"/> of the invocation involved in the Act part of your test.</typeparam>
    /// <seealso cref="SmartTest" />
    public class InvokeAct<T>: Act<T>
    {
        /// <summary>
        ///     Creates an instance of <see cref="InvokeAct{T}" /> to represent an expression  in the Act part of your test.
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
            _Invocation = invocation;

            object instance;
            MemberInfo member;
            if( invocation.GetMemberContext( out instance, out member ) )
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
                        if( property.GetMethod == Method )
                        {
                            Property = property;
                            break;
                        }
            }
            if( Constructor == null && Method == null && Field == null )
                throw new BadTestException( string.Format( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() ) );
        }


        private readonly Expression<Func<T>> _Invocation;


        /// <inheritdoc />
        public override T Invoke() => _Invocation.Compile().Invoke();
    }
}