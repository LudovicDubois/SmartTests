using System;
using System.Runtime.Serialization;

using JetBrains.Annotations;



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


        protected SmartTestException( [NotNull] SerializationInfo info, StreamingContext context )
            : base( info, context )
        { }
    }
}