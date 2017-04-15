using System;

using JetBrains.Annotations;



namespace SmartTests
{
    [PublicAPI]
    [AttributeUsage( AttributeTargets.Field )]
    public class ErrorAttribute: Attribute
    {}
}