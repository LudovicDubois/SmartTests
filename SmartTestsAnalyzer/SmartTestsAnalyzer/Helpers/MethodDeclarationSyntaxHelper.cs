using System;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer.Helpers
{
    static class SyntaxNodeHelper
    {
        public static string GetFullName( this MemberDeclarationSyntax @this, bool includeNamespace = true )
        {
            var result = @this.Parent is MemberDeclarationSyntax parent &&
                         ( includeNamespace || !( parent is NamespaceDeclarationSyntax ) )
                             ? parent.GetFullName( includeNamespace ) + '.'
                             : "";

            switch( @this.Kind() )
            {
                case SyntaxKind.MethodDeclaration:
                    return result + ( (MethodDeclarationSyntax)@this ).Identifier.Text;

                case SyntaxKind.ClassDeclaration:
                    return result + ( (ClassDeclarationSyntax)@this ).Identifier.Text;

                case SyntaxKind.NamespaceDeclaration:
                    return result + ( (NamespaceDeclarationSyntax)@this ).Name;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}