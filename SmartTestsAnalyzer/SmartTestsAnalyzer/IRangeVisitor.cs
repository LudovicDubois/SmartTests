using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests.Ranges;



namespace SmartTestsAnalyzer
{
    interface IRangeVisitor
    {
        IType Root { get; }
        bool IsError { get; }

        void VisitInvocationExpression( InvocationExpressionSyntax node );
    }
}