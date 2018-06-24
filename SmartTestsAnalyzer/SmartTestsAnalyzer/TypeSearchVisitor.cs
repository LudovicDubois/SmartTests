using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests;
using SmartTests.Ranges;
using SmartTestsAnalyzer.Helpers;


namespace SmartTestsAnalyzer
{
    class TypeSearchVisitor : CSharpSyntaxVisitor<IType>
    {

        public TypeSearchVisitor(SemanticModel model, Action<Diagnostic> reportDiagnostics,
            InvocationExpressionSyntax node)
        {
            _Model = model;
            var smartTestType = model.Compilation.GetTypeByMetadataName("SmartTests.SmartTest");

            AddType(smartTestType, "Int", () => node.Accept(new RangeVisitor<int>(_Model, SmartTest.Int, reportDiagnostics)) );
        }

        private readonly SemanticModel _Model;
        private readonly Dictionary<IPropertySymbol, Func<IType>> _TypeProperties =
            new Dictionary<IPropertySymbol, Func<IType>>();


        private void AddType(ITypeSymbol smartTestType, string propertyName, Func<IType> iTypeCreator)
        {
            var rootProperty = (IPropertySymbol) smartTestType.GetMembers(propertyName)[0];
            Debug.Assert(rootProperty != null, $"Problem with SmartTest.{propertyName} property");
            _TypeProperties[rootProperty] = iTypeCreator;
        }


        // Visit Methods

        private IType GetRoot(SyntaxNode node)
        {
            var member = _Model.GetSymbol(node);
            if (member is IPropertySymbol type)
                if (_TypeProperties.TryGetValue(type, out var func))
                    return func();
            return null;
        }


        public override IType VisitIdentifierName(IdentifierNameSyntax node) => GetRoot( node ) ?? base.VisitIdentifierName(node);


        public override IType VisitMemberAccessExpression(MemberAccessExpressionSyntax node) => GetRoot(node) ?? node.Expression.Accept( this );


        public override IType VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) => node.Expression.Accept(this);


        public override IType VisitInvocationExpression(InvocationExpressionSyntax node) => node.Expression.Accept(this);
    }
}