using System;

using JetBrains.Annotations;



namespace SmartTests
{
    /// <summary>
    /// Indicates that a criterion is for testing an error.
    /// </summary>
    /// <seealso cref="Criteria"/>
    /// <remarks>
    /// <para>To create a new criteria, you have to create a subclass of <see cref="Criteria"/> and defined static fields of the new type for each of its criterion.</para>
    /// <para>If any criterion is an error, you have to mark it as an error with this attribute.</para>
    /// <para>Errors are not combined when searching for missing tests.</para>
    /// </remarks>
    [PublicAPI]
    [AttributeUsage( AttributeTargets.Field )]
    public class ErrorAttribute: Attribute
    {}
}