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
    ///     'Case( Criteria1.Good1 | Criteria1.Good2 )': Good1 field of Criteria1 is one CriteriaAnd and Good2 field of
    ///     Criteria1 is another one
    ///     'Case( Criteria1.Good1 & Criteria2.GoodA )': Good1 field of Criteria1 and GoodB field of Criteria2 is one
    ///     CriteriaAnd
    ///     'Case( Criteria1.Good1 > Criteria2.GoodA )': Good1 field of Criteria1 and GoodB field of Criteria2 is one
    ///     CriteriaAnd
    /// </example>
    public class CriteriasAndOr
    {
        private CriteriasAndOr()
        { }


        public CriteriasAndOr( ExpressionSyntax caseExpression, ExpressionSyntax parameterNameExpression, IFieldSymbol criteria, bool hasError )
        {
            CaseExpression = caseExpression;
            ParameterNameExpression = parameterNameExpression;
            Criterias.Add( new CriteriasAnd( caseExpression, criteria, hasError ) );
        }


        public ExpressionSyntax CaseExpression { get; }
        public ExpressionSyntax ParameterNameExpression { get; }

        public List<CriteriasAnd> Criterias { get; private set; } = new List<CriteriasAnd>();


        public void Add( CriteriasAndOr criterias )
        {
            foreach( var combinedCriterias in criterias.Criterias )
                Criterias.Add( combinedCriterias );
        }


        public void CombineAnd( CriteriasAndOr criterias )
        {
            if( Criterias.Count == 0 )
            {
                // No criteria yet!
                // => Take it as it
                Criterias = criterias.Criterias;
                return;
            }

            var currentCriterias = Criterias;
            Criterias = new List<CriteriasAnd>();
            var otherErrors = new HashSet<CriteriasAnd>();
            foreach( var combinedCriterias in currentCriterias )
            {
                if( combinedCriterias.HasError )
                {
                    // Do not combine it
                    Criterias.Add( combinedCriterias );
                    continue;
                    ;
                }

                foreach( var otherCriterias in criterias.Criterias )
                {
                    if( otherCriterias.HasError )
                    {
                        if( otherErrors.Add( otherCriterias ) )
                            // Do not combine it
                            Criterias.Add( otherCriterias );
                        continue;
                    }

                    Criterias.Add( combinedCriterias.CombineAnd( otherCriterias ) );
                }
            }
        }


        public void CombineOr( CriteriasAndOr criterias ) => Criterias.AddRange( criterias.Criterias );


        private void CombineOr( IFieldSymbol criteria )
        {
            Criterias.Add( new CriteriasAnd( null, criteria, false ) ); //Todo: Not false!
        }


        private void Remove( CriteriasAndOr criterias )
        {
            foreach( var criteria in criterias.Criterias )
                Criterias.Remove( criteria );
        }


        public void Validate( TestedMember testedMember, string parameterName, INamedTypeSymbol errorType, Action<Diagnostic> reportError )
        {
            var allCriterias = ComputeAllCriteriaCombinations( errorType );
            allCriterias.Remove( this );
            if( allCriterias.Criterias.Count == 0 )
                // No missing cases
                return;

            reportError( SmartTestsDiagnostics.CreateMissingCase( testedMember,
                                                                  parameterName,
                                                                  GetExpressionSyntaxes(),
                                                                  CreateMessage( allCriterias ) ) );
        }


        private static string CreateMessage( CriteriasAndOr allCriterias )
        {
            var result = new StringBuilder();
            foreach( var criterias in allCriterias.Criterias )
            {
                foreach( var criteria in criterias.Criterias )
                {
                    result.Append( criteria.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) );
                    result.Append( " & " );
                }
                result.Length -= 3;
                result.Append( " and " );
            }
            result.Length -= 5;
            return result.ToString();
        }


        private IList<ExpressionSyntax> GetExpressionSyntaxes()
        {
            var result = new List<ExpressionSyntax>();
            foreach( var combinedCriteriase in Criterias )
            foreach( var criteriaExpression in combinedCriteriase.CriteriaExpressions )
                if( !result.Contains( criteriaExpression ) )
                    result.Add( criteriaExpression );
            return result;
        }


        private CriteriasAndOr ComputeAllCriteriaCombinations( INamedTypeSymbol errorType )
        {
            var result = new CriteriasAndOr();
            foreach( var criteriaType in GetAllCriteriaTypes() )
            {
                var typeCriterias = new CriteriasAndOr();
                foreach( var criteria in criteriaType.GetMembers().Where( member => member is IFieldSymbol ).Cast<IFieldSymbol>() )
                    typeCriterias.CombineOr( new CriteriasAndOr( null, null, criteria, criteria.HasAttribute( errorType ) ) );
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