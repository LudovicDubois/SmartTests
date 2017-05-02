using System;
using System.Linq.Expressions;

using JetBrains.Annotations;

using SmartTests.Acts;



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


        public static Act<T> Assign<T>( Expression<Func<T>> property, T value ) => new AssignAct<T>( property, value );


        public static T RunTest<T>( Criteria cases, Expression<Func<T>> act ) => RunTest( Case( cases ), new InvokeAct<T>( act ) );
        public static T RunTest<T>( Case cases, Expression<Func<T>> act ) => RunTest( cases, new InvokeAct<T>( act ) );


        public static T RunTest<T>( Criteria cases, [NotNull] Act<T> act ) => RunTest( Case( cases ), act );


        public static T RunTest<T>( Case cases, [NotNull] Act<T> act )
        {
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            return act.Invoke();
        }


        public static void RunTest( Criteria cases, Expression<Action> act ) => RunTest( Case( cases ), new InvokeAct( act ) );
        public static void RunTest( Case cases, Expression<Action> act ) => RunTest( cases, new InvokeAct( act ) );


        public static void RunTest( Case cases, [NotNull] Act act )
        {
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            act.Invoke();
        }
    }
}