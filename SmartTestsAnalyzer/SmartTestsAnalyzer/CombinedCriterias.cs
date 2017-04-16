using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    class CombinedCriterias
    {
        private CombinedCriterias()
        {}


        public CombinedCriterias( IMethodSymbol testMethod, IFieldSymbol criteria, bool hasError )
        {
            if( testMethod != null )
                TestMethods.Add( testMethod );
            Criterias.Add( criteria );
            HasError = hasError;
        }


        public List<IFieldSymbol> Criterias { get; } = new List<IFieldSymbol>();
        public List<IMethodSymbol> TestMethods { get; } = new List<IMethodSymbol>();
        public bool HasError { get; private set; }


        public CombinedCriterias CombineAnd( CombinedCriterias otherCriterias )
        {
            var result = new CombinedCriterias();
            result.Criterias.AddRange( Criterias );
            result.Criterias.AddRange( otherCriterias.Criterias );
            result.HasError = HasError || otherCriterias.HasError;
            return result;
        }


        public void FillWithCriteriaTypes( HashSet<ITypeSymbol> criteriaTypes )
        {
            foreach( var criteria in Criterias )
                criteriaTypes.Add( criteria.ContainingType );
        }


        public override bool Equals( object other )
        {
            var otherCriterias = other as CombinedCriterias;
            if( otherCriterias == null )
                return false;

            return otherCriterias.Criterias.Count == Criterias.Count &&
                   otherCriterias.Criterias.All( otherCriteria => Criterias.Contains( otherCriteria ) );
        }


        public override int GetHashCode() => Criterias.Aggregate( 0,
                                                                  ( current, fieldSymbol ) => current ^ fieldSymbol.GetHashCode() );
    }
}