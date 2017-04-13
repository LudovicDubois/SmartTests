using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Helpers
{
    class FullSymbolVisitor: SymbolVisitor
    {
        public override void VisitAssembly( IAssemblySymbol symbol )
        {
            foreach( var module in symbol.Modules )
                module.Accept( this );
        }


        public override void VisitModule( IModuleSymbol symbol )
        {
            foreach( var namespaceOrTypeSymbol in symbol.GlobalNamespace.GetMembers() )
                namespaceOrTypeSymbol.Accept( this );
        }


        public override void VisitNamespace( INamespaceSymbol symbol )
        {
            foreach( var namespaceMember in symbol.GetMembers() )
                namespaceMember.Accept( this );
        }


        public override void VisitNamedType( INamedTypeSymbol symbol )
        {
            foreach( var member in symbol.GetMembers() )
                member.Accept( this );
        }
    }
}