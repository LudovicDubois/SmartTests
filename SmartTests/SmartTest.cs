using System;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;



namespace SmartTests
{
    /// <summary>
    ///     This is the main class for SmartTests.
    /// </summary>
    /// <remarks>
    ///     It is recommended to use it with <c>using static SmartTests.SmartTest</c>
    /// </remarks>
    [PublicAPI]
    public static class SmartTest
    {
        public static Case Case( Criteria criteria ) => new Case( criteria );
        public static Case Case( string parameterName, Criteria criteria ) => new Case( parameterName, criteria );


        public static T RunTest<T>( Case cases, Expression<Func<T>> act ) => RunTest( cases, act.GetMember(), act.Compile() );


        private static T RunTest<T>( Case cases, [NotNull] MemberInfo member, Func<T> act )
        {
            if( member == null )
                throw new ArgumentNullException( nameof(member) );
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            return act.Invoke();
        }


        public static void RunTest( Case cases, Expression<Action> act ) => RunTest( cases, act.GetMember(), act.Compile() );


        private static void RunTest( Case cases, [NotNull] MemberInfo member, Action act )
        {
            if( member == null )
                throw new ArgumentNullException( nameof(member) );
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            act.Invoke();
        }


        private static MemberInfo GetMember( this Expression @this )
        {
            switch( @this.NodeType )
            {
                case ExpressionType.New:
                    return ( (NewExpression)@this ).Constructor;

                case ExpressionType.MemberAccess:
                    return ( (MemberExpression)@this ).Member;

                default:
                    throw new NotImplementedException();
            }
        }


        private static MemberInfo GetMember<T>( this Expression<Func<T>> @this ) => @this.Body.GetMember();
    }
}