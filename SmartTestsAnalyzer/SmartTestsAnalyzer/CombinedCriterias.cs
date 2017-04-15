using System.Collections.Generic;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    class CombinedCriterias
    {
        private CombinedCriterias()
        {}


        public CombinedCriterias( IFieldSymbol criteria, bool hasError )
        {
            Criterias.Add( criteria );
            HasError = hasError;
        }


        public List<IFieldSymbol> Criterias { get; } = new List<IFieldSymbol>();
        public HashSet<IMethodSymbol> TestMethods { get; } = new HashSet<IMethodSymbol>();
        public bool HasError { get; private set; }


        public CombinedCriterias CombineAnd( CombinedCriterias otherCriterias )
        {
            var result = new CombinedCriterias();
            result.Criterias.AddRange( Criterias );
            result.Criterias.AddRange( otherCriterias.Criterias );
            result.HasError = HasError || otherCriterias.HasError;
            return result;
        }
    }
}