using System.Collections.Generic;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer.Criterias
{
    public abstract class CriteriaAnalysis
    {
        public abstract void AddValues( Dictionary<string, CriteriaValues> values, INamedTypeSymbol errorType );

        public abstract string ToDisplayString( SymbolDisplayFormat displayFormat );
    }
}