using System;
using System.Text;



namespace SmartTests
{
    public class SmartTestException: Exception
    {
        public SmartTestException()
        { }


        public SmartTestException( string message )
            : base( message )
        { }


        public SmartTestException( string message, params object[] args )
            : this( string.Format( message, args ) )
        { }


        public SmartTestException( StringBuilder message, params object[] args )
            : this( string.Format( message.ToString(), args ) )
        { }


        public SmartTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }


    public class BadTestException: SmartTestException
    {
        private string name;
        private string v;


        public BadTestException()
        { }


        public BadTestException( string message )
            : base( message )
        { }


        public BadTestException( string message, params object[] args )
            : this( string.Format( message, args ) )
        { }


        public BadTestException( StringBuilder message, params object[] args )
            : this( string.Format( message.ToString(), args ) )
        { }


        public BadTestException( string message, Exception innerException )
            : base( message, innerException )
        { }
    }
}