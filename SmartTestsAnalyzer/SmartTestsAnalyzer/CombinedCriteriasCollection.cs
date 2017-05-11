using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



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
    public class CombinedCriteriasCollection
    {
        private CombinedCriteriasCollection()
        { }


        public CombinedCriteriasCollection( ExpressionSyntax caseExpression, ExpressionSyntax parameterNameExpression, ExpressionSyntax criteriaExpression, IFieldSymbol criteria, bool hasError )
        {
            CaseExpression = caseExpression;
            ParameterNameExpression = parameterNameExpression;
            Criterias.Add( new CombinedCriterias( caseExpression, criteria, hasError ) );
        }


        public ExpressionSyntax CaseExpression { get; }
        public ExpressionSyntax ParameterNameExpression { get; }

        public List<CombinedCriterias> Criterias { get; private set; } = new List<CombinedCriterias>();


        public void Add( CombinedCriteriasCollection criterias )
        {
            foreach( var combinedCriterias in criterias.Criterias )
                Criterias.Add( combinedCriterias );
        }


        public void CombineAnd( CombinedCriteriasCollection criterias )
        {
            if( Criterias.Count == 0 )
            {
                // No criteria yet!
                // => Take it as it
                Criterias = criterias.Criterias;
                return;
            }

            var currentCriterias = Criterias;
            Criterias = new List<CombinedCriterias>();
            foreach( var combinedCriterias in currentCriterias )
            foreach( var otherCriterias in criterias.Criterias )
                Criterias.Add( combinedCriterias.CombineAnd( otherCriterias ) );
        }


        public void CombineOr( CombinedCriteriasCollection criterias ) => Criterias.AddRange( criterias.Criterias );


        private void CombineOr( IFieldSymbol criteria )
        {
            Criterias.Add( new CombinedCriterias( null, criteria, false ) ); //Todo: Not false!
        }


        private void Remove( CombinedCriteriasCollection criterias )
        {
            foreach( var criteria in criterias.Criterias )
                Criterias.Remove( criteria );
        }


        public void Validate( TestedMember testedMember, string parameterName, Action<Diagnostic> reportError )
        {
            var allCriterias = ComputeAllCriteriaCombinations();
            allCriterias.Remove( this );
            if( allCriterias.Criterias.Count == 0 )
                // No missing cases
                return;

            var text = new StringBuilder();
            foreach( var criterias in allCriterias.Criterias )
            {
                foreach( var criteria in criterias.Criterias )
                {
                    text.Append( criteria.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) );
                    text.Append( " & " );
                }
                text.Length -= 3;
                text.Append( " and " );
            }
            text.Length -= 5;
            reportError( SmartTestsDiagnostics.CreateMissingCase( testedMember, parameterName, GetExpressionSyntaxes(), text.ToString() ) );
        }


        private IList<ExpressionSyntax> GetExpressionSyntaxes()
        {
            var result = new List<ExpressionSyntax>();
            foreach( var combinedCriteriase in Criterias )
                result.AddRange( combinedCriteriase.CriteriaExpressions );
            return result;
        }


        private CombinedCriteriasCollection ComputeAllCriteriaCombinations()
        {
            var result = new CombinedCriteriasCollection();
            foreach( var criteriaType in GetAllCriteriaTypes() )
            {
                var typeCriterias = new CombinedCriteriasCollection();
                foreach( var criteria in criteriaType.GetMembers().Where( member => member is IFieldSymbol ).Cast<IFieldSymbol>() )
                    typeCriterias.CombineOr( new CombinedCriteriasCollection( null, null, null, criteria, false ) ); // TODO: Not false!
                result.CombineAnd( typeCriterias );
            }
            return result;
        }


        private HashSet<ITypeSymbol> GetAllCriteriaTypes()
        {
            var result = new HashSet<ITypeSymbol>();
            foreach( var criteria in Criterias )
                criteria.FillWithCriteriaTypes( result );
            return result;
        }
    }
}