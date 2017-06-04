using System;
using System.Linq.Expressions;

using JetBrains.Annotations;

using SmartTests.Acts;
using SmartTests.Helpers;



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
        static SmartTest()
        { }


        #region Case

        public static Case Case( Criteria criteria ) => new Case( criteria );
        public static Case Case( string parameterName, Criteria criteria ) => new Case( parameterName, criteria );

        #endregion


        #region Assign

        public static Act<T> Assign<T>( Expression<Func<T>> property, T value ) => new AssignAct<T>( property, value );

        #endregion


        #region RunTest

        public static T RunTest<T>( Criteria cases, Expression<Func<T>> act, params Assertion[] assertions ) => RunTest( Case( cases ), new InvokeAct<T>( act ), assertions );
        public static T RunTest<T>( Case cases, Expression<Func<T>> act, params Assertion[] assertions ) => RunTest( cases, new InvokeAct<T>( act ), assertions );


        public static T RunTest<T>( Criteria cases, [NotNull] Act<T> act, params Assertion[] assertions ) => RunTest( Case( cases ), act, assertions );


        public static T RunTest<T>( Case cases, [NotNull] Act<T> act, params Assertion[] assertions )
        {
            if( act == null )
                throw new ArgumentNullException( nameof(act) );


            BeforeAct( assertions );
            try
            {
                var result = act.Invoke();
                AfterAct( assertions, null );
                return result;
            }
            catch( Exception e )
            {
                e = e.NoInvocation();
                AfterAct( assertions, e );
                throw e;
            }
        }


        private static void BeforeAct( Assertion[] assertions )
        {
            foreach( var assertion in assertions )
                assertion.BeforeAct();
        }


        private static void AfterAct( Assertion[] assertions, Exception exception )
        {
            foreach( var assertion in assertions )
                assertion.AfterAct( exception );
        }


        public static void RunTest( Criteria cases, Expression<Action> act, params Assertion[] assertions ) => RunTest( Case( cases ), new InvokeAct( act ), assertions );
        public static void RunTest( Case cases, Expression<Action> act, params Assertion[] assertions ) => RunTest( cases, new InvokeAct( act ), assertions );


        public static void RunTest( Case cases, [NotNull] Act act, params Assertion[] assertions )
        {
            if( act == null )
                throw new ArgumentNullException( nameof(act) );

            BeforeAct( assertions );
            try
            {
                act.Invoke();
                AfterAct( assertions, null );
            }
            catch( Exception e )
            {
                AfterAct( assertions, e.NoInvocation() );
                throw;
            }
        }

        #endregion


        #region SmartAssert Extensions

        public static SmartAssert SmartAssert { get; }

        #endregion
    }
}