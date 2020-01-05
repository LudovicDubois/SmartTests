using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer
{
    interface IRangeVisitor
    {
        IType Root { get; }
        // ReSharper disable once InconsistentNaming
        bool IsError { get; }

        void VisitInvocationExpression( InvocationExpressionSyntax node );
    }
}