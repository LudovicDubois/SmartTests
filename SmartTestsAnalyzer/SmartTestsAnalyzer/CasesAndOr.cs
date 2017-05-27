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
    public class CasesAndOr
    {
        public CasesAndOr()
        { }


        public CasesAndOr( ExpressionSyntax casesExpression, ExpressionSyntax parameterNameExpression, string parameterName, IFieldSymbol criteria, bool hasError )
        {
            CasesAnd.Add( new CasesAnd( parameterNameExpression, parameterName, casesExpression, criteria, hasError ) );
        }


        public ExpressionSyntax CasesExpression => CasesAnd.First().Cases.First().Value.CaseExpressions.First();

        public List<CasesAnd> CasesAnd { get; } = new List<CasesAnd>();


        public void CombineAnd( CasesAndOr cases )
        {
            if( cases == null )
                return;

            if( CasesAnd.Count == 0 )
            {
                // No criteria yet!
                // => Take it as it
                CasesAnd.AddRange( cases.CasesAnd );
                return;
            }

            var currentCriterias = CasesAnd.ToList();
            CasesAnd.Clear();
            var otherErrors = new HashSet<CasesAnd>();
            foreach( var combinedCriterias in currentCriterias )
            {
                if( combinedCriterias.HasError )
                {
                    // Do not combine it
                    CasesAnd.Add( combinedCriterias );
                    continue;
                }

                foreach( var otherCriterias in cases.CasesAnd )
                {
                    if( otherCriterias.HasError )
                    {
                        if( otherErrors.Add( otherCriterias ) )
                            // Do not combine it
                            CasesAnd.Add( otherCriterias );
                        continue;
                    }

                    CasesAnd.Add( combinedCriterias.CombineAnd( otherCriterias ) );
                }
            }
        }


        public void CombineOr( CasesAndOr cases ) => CasesAnd.AddRange( cases.CasesAnd );


        private void CombineOr( IFieldSymbol criteria )
        {
            CasesAnd.Add( new CasesAnd( null, null, null, criteria, false ) ); //Todo: Not false!
        }


        private void Remove( CasesAndOr cases )
        {
            foreach( var criteria in cases.CasesAnd )
                CasesAnd.Remove( criteria );
        }


        public void Validate( TestedMember testedMember, INamedTypeSymbol errorType, Action<Diagnostic> reportError )
        {
            var allCases = ComputeAllCases( errorType );
            allCases.Remove( this );
            if( allCases.CasesAnd.Count == 0 )
                // No missing cases
                return;

            reportError( SmartTestsDiagnostics.CreateMissingCases( testedMember,
                                                                   GetExpressionSyntaxes(),
                                                                   allCases.ToString() ) );
        }


        private IList<ExpressionSyntax> GetExpressionSyntaxes()
        {
            var result = new List<ExpressionSyntax>();
            foreach( var caseAnd in CasesAnd )
                caseAnd.FillWithExpressionSyntaxes( result );
            return result;
        }


        private CasesAndOr ComputeAllCases( INamedTypeSymbol errorType )
        {
            var result = new CasesAndOr();
            foreach( var pair in GetAllCriteriaTypes() )
                result.CombineAnd( ComputeAllCases( errorType, pair.Key, pair.Value ) );
            return result;
        }


        private static CasesAndOr ComputeAllCases( INamedTypeSymbol errorType, string parameterName, IEnumerable<ITypeSymbol> types )
        {
            var result = new CasesAndOr();
            foreach( var typeSymbol in types )
            {
                var typeCases = new CasesAndOr();
                foreach( var criterion in typeSymbol.GetMembers().Where( member => member is IFieldSymbol ).Cast<IFieldSymbol>() )
                    typeCases.CombineOr( new CasesAndOr( null, null, parameterName, criterion, criterion.HasAttribute( errorType ) ) );
                result.CombineAnd( typeCases );
            }
            return result;
        }


        private Dictionary<string, HashSet<ITypeSymbol>> GetAllCriteriaTypes()
        {
            var result = new Dictionary<string, HashSet<ITypeSymbol>>();
            foreach( var criteria in CasesAnd )
                criteria.FillWithCriteriaTypes( result );
            return result;
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            foreach( var criterias in CasesAnd )
            {
                criterias.ToString( result );
                result.Append( " and " );
            }
            result.Length -= 5;
            return result.ToString();
        }
    }
}