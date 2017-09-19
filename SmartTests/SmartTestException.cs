using System;



namespace SmartTests
{
    public class SmartTestException: Exception
    {
        public SmartTestException()
        { }


        public SmartTestException( string message )
            : base( message )
        { }


        public SmartTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }


    public class BadTestException: SmartTestException
    {
        public BadTestException()
        { }


        public BadTestException( string message )
            : base( message )
        { }


        public BadTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }
}