using System;
using System.Text;



namespace SmartTests
{
    /// <summary>
    ///     The exception that is thrown when an error occurs in the <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </summary>
    /// <remarks>
    ///     It can be in the Act part or in the <see cref="Assertion.AfterAct" /> of any Smart Assertion.
    /// </remarks>
    /// <seealso cref="BadTestException" />
    public class SmartTestException: Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class.
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        public SmartTestException()
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SmartTestException( string message )
            : base( message )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified formatted error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="args">The arguments to format the message.</param>
        public SmartTestException( string message, params object[] args )
            : this( string.Format( message, args ) )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified formatted error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="args">The arguments to format the message.</param>
        // ReSharper disable once UnusedMember.Global
        public SmartTestException( StringBuilder message, params object[] args )
            : this( string.Format( message.ToString(), args ) )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified error message and a
        ///     reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a <c>null</c> reference if no
        ///     inner exception is specified.
        /// </param>
        public SmartTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }


    /// <summary>
    ///     The exception that is thrown when an error occurs in the <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </summary>
    /// <remarks>
    ///     It can only be in the <see cref="Assertion.BeforeAct" /> of any Smart Assertion (i.e. in the Arrange or Assume
    ///     parts
    ///     of your tests).
    /// </remarks>
    /// <seealso cref="SmartTestException" />
    public class BadTestException: SmartTestException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BadTestException" /> class.
        /// </summary>
        public BadTestException()
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="BadTestException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BadTestException( string message )
            : base( message )
        { }



        /// <summary>
        ///     Initializes a new instance of the <see cref="BadTestException" /> class with a specified error message and a
        ///     reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a <c>null</c> reference if no
        ///     inner exception is specified.
        /// </param>
        // ReSharper disable once UnusedMember.Global
        public BadTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }
}