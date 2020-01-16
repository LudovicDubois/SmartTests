using System;
using System.Linq;

// ReSharper disable UnusedMember.Global


namespace SmartTests.Assertions
{
    /// <summary>
    ///     Helper class to test Exception throw.
    /// </summary>
    /// <seealso cref="SmartTest.SmartAssert" />
    /// <seealso cref="SmartTest" />
    public static class ThrowAssertions
    {
        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure the provided exception type is thrown.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="verify">Code to add verifications on the exception.</param>
        /// <typeparam name="T">The expected exception type</typeparam>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <remarks>Should be the first assertion.</remarks>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Throw<T>( this SmartAssertPlaceHolder _, Action<T> verify = null )
            where T: Exception
            => new ThrowAssertion<T>( verify );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure the provided exception type is thrown for the
        ///     <paramref name="parameterName" /> parameter.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="parameterName">The name of the parameter for which the exception is thrown.</param>
        /// <param name="verify">Code to add verifications on the exception.</param>
        /// <typeparam name="T">The expected exception type</typeparam>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <remarks>Should be the first assertion.</remarks>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Throw<T>( this SmartAssertPlaceHolder _, string parameterName, Action<T> verify = null )
            where T: ArgumentException
            => new ThrowArgumentAssertion<T>( parameterName, null, verify );


        /// <summary>
        ///     Creates an <see cref="Assertion" /> that ensure the provided exception type is thrown for the
        ///     <paramref name="parameterName" /> parameter.
        /// </summary>
        /// <param name="_">The dummy place holder for all Smart Assertions.</param>
        /// <param name="parameterName">The name of the parameter for which the exception is thrown.</param>
        /// <param name="message">
        ///     The expectedMessage of the exception, without the last line (ParameterName: XXX) because it depends on
        ///     the current culture.
        /// </param>
        /// <param name="verify">Code to add verifications on the exception.</param>
        /// <typeparam name="T">The expected exception type</typeparam>
        /// <returns>The newly created <see cref="Assertion" />.</returns>
        /// <remarks>Should be the first assertion.</remarks>
        // ReSharper disable once UnusedParameter.Global
        public static Assertion Throw<T>( this SmartAssertPlaceHolder _, string parameterName, string message, Action<T> verify = null )
            where T: ArgumentException
            => new ThrowArgumentAssertion<T>( parameterName, message, verify );


        private class ThrowAssertion<T>: Assertion
            where T: Exception
        {
            public ThrowAssertion( Action<T> verify )
            {
                _Verify = verify;
            }


            private readonly Action<T> _Verify;


            public override void BeforeAct( ActBase act )
            { }


            public override void AfterAct( ActBase act )
            {
                if( act.Exception == null )
                    throw new SmartTestException( string.Format( Resource.ThrowNoException, typeof(T).FullName ) );
                if( act.Exception.GetType() != typeof(T) )
                {
                    act.Exception = new SmartTestException( string.Format( Resource.ThrowWrongException, typeof(T).FullName, act.Exception.GetType().FullName ) );
                    throw act.Exception;
                }

                var exception = (T)act.Exception;
                act.Exception = null;
                _Verify?.Invoke( exception );
            }
        }


        private class ThrowArgumentAssertion<T>: ThrowAssertion<T>
            where T: ArgumentException
        {
            public ThrowArgumentAssertion( string expectedParameterName, string expectedMessage, Action<T> verify )
                : base( verify )
            {
                _ExpectedParameterName = expectedParameterName ?? throw new ArgumentNullException( nameof(expectedParameterName) );
                _ExpectedMessage = expectedMessage;
            }


            private readonly string _ExpectedParameterName;
            private readonly string _ExpectedMessage;


            public override void AfterAct( ActBase act )
            {
                var argumentException = (ArgumentException)act.Exception;

                base.AfterAct( act );

                // Exception was expected => No error anymore!
                act.Exception = null;
                if( argumentException.ParamName != _ExpectedParameterName )
                    throw new SmartTestException( string.Format( Resource.ThrowBadParameterName, _ExpectedParameterName, argumentException.ParamName ) );

                if( _ExpectedMessage != null &&
                    argumentException.Message.EndsWith( argumentException.ParamName ) &&
                    ExpectedMessage() != _ExpectedMessage )
                    throw new SmartTestException( string.Format( Resource.ThrowBadMessage, _ExpectedMessage, ExpectedMessage() ) );


                string ExpectedMessage()
                {
                    var strings = argumentException.Message.Split( new[] { Environment.NewLine }, StringSplitOptions.None ).ToList();
                    strings.RemoveAt( strings.Count - 1 );
                    return string.Join( Environment.NewLine, strings );
                }
            }
        }
    }
}