using System;
using System.Text;



namespace SmartTests
{
    /// <summary>
    ///     The exception that is thrown when an error occurs in the <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </summary>
    /// <remarks>
    ///     It can be in the Act part or in the <see cref="ActBase.AfterAct" /> of any Smart Assertion.
    /// </remarks>
    /// <seealso cref="BadTestException" />
    public class SmartTestException: Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class.
        /// </summary>
        public SmartTestException()
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified error message.
        /// </summary>
        public SmartTestException( string message )
            : base( message )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified formatted error message.
        /// </summary>
        public SmartTestException( string message, params object[] args )
            : this( string.Format( message, args ) )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified formatted error message.
        /// </summary>
        public SmartTestException( StringBuilder message, params object[] args )
            : this( string.Format( message.ToString(), args ) )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartTestException" /> class with a specified error message and a
        ///     reference to the inner exception that is the cause of this exception.
        /// </summary>
        public SmartTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }


    /// <summary>
    ///     The exception that is thrown when an error occurs in the <see cref="O:SmartTests.SmartTest.RunTest" /> methods.
    /// </summary>
    /// <remarks>
    ///     It can only be in the <see cref="ActBase.BeforeAct" /> of any Smart Assertion (i.e. in the Arrange or Assume parts
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
        public BadTestException( string message )
            : base( message )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="BadTestException" /> class with a specified formatted error message.
        /// </summary>
        public BadTestException( string message, params object[] args )
            : this( string.Format( message, args ) )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="BadTestException" /> class with a specified formatted error message.
        /// </summary>
        public BadTestException( StringBuilder message, params object[] args )
            : this( string.Format( message.ToString(), args ) )
        { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="BadTestException" /> class with a specified error message and a
        ///     reference to the inner exception that is the cause of this exception.
        /// </summary>
        public BadTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }
}