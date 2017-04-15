using System.Collections.Generic;

using Microsoft.CodeAnalysis;



namespace SmartTestsAnalyzer
{
    /// <summary>
    ///     A list of test cases (IFieldSymbol of Criteria subclasses) combined with logical AND to represents the criteria of
    ///     a parameter of a tested member.
    /// </summary>
    /// <remarks>
    ///     It is used for user test cases and for expected test cases (when computing all expected test cases)
    /// </remarks>
    /// <example>
    ///     'Case( Criteria1.Good1 )': Good1 field of Criteria1
    ///     'Case( Criteria1.Good1 | Criteria1.Good2 )': Good1 field of Criteria1 is one CombinedCriterias and Good2 field of
    ///     Criteria1 is another one
    ///     'Case( Criteria1.Good1 & Criteria2.GoodA )': Good1 field of Criteria1 and GoodB field of Criteria2 is one
    ///     CombinedCriterias
    ///     'Case( Criteria1.Good1 > Criteria2.GoodA )': Good1 field of Criteria1 and GoodB field of Criteria2 is one
    ///     CombinedCriterias
    /// </example>
    class CombinedCriteriasCollection
    {
        public CombinedCriteriasCollection( IFieldSymbol criteria, bool hasError )
        {
            Criterias.Add( new CombinedCriterias( criteria, hasError ) );
        }


        public List<CombinedCriterias> Criterias { get; private set; } = new List<CombinedCriterias>();


        public void Add( CombinedCriteriasCollection criterias )
        {
            foreach( var combinedCriterias in criterias.Criterias )
                Criterias.Add( combinedCriterias );
        }


        public void CombineAnd( CombinedCriteriasCollection criterias )
        {
            var currentCriterias = Criterias;
            Criterias = new List<CombinedCriterias>();
            foreach( var combinedCriterias in currentCriterias )
            foreach( var otherCriterias in criterias.Criterias )
                Criterias.Add( combinedCriterias.CombineAnd( otherCriterias ) );
        }


        public void Validate()
        {
            
        }
    }
}