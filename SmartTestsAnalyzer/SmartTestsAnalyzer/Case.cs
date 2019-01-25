using System.Collections.Generic;
using System.Text;

#if !EXTENSION
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Newtonsoft.Json;

using SmartTestsAnalyzer.Helpers;
using SmartTestsAnalyzer.Criterias;

#endif



namespace SmartTestsAnalyzer
{
    public class Case
    {
        public static string NoParameter => "";


#if EXTENSION
        public string ParameterName { get; }
        public string ParameterPath { get; }
        public List<string> Expressions { get; } = new List<string>();
        public bool HasError { get; set; }


#else
        public Case( TestedParameter testedParameter )
        {
            Debug.Assert( testedParameter?.Name != null );
            TestedParameter = testedParameter;
        }


        public string ParameterName => TestedParameter.Name; // For serialization to Json for the Extension
        public string ParameterPath => TestedParameter.Path; // For serialization to Json for the Extension
        [JsonIgnore]
        public TestedParameter TestedParameter { get; }
        [JsonIgnore]
        public List<ExpressionSyntax> CaseExpressions { get; } = new List<ExpressionSyntax>();
        public List<string> Expressions => CriteriaAnalysis.Select( criteria => criteria.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) ).ToList();
        [JsonIgnore]
        public List<CriteriaAnalysis> CriteriaAnalysis { get; } = new List<CriteriaAnalysis>();
        public bool HasError { get; private set; }


        public void Add( ExpressionSyntax caseExpression, CriteriaAnalysis criterion, bool hasError )
        {
            if( caseExpression != null )
                CaseExpressions.Add( caseExpression );

            CriteriaAnalysis.Add( criterion );
            if( hasError )
                HasError = true;
        }


        public void FillCriteriaValues( Dictionary<TestedParameter, CriteriaValues> types, INamedTypeSymbol errorType )
        {
            foreach( var analysis in CriteriaAnalysis )
                analysis.AddValues( types, errorType );
        }


        public void FillWith( Case otherCase )
        {
            Debug.Assert( otherCase.TestedParameter.Name == TestedParameter.Name );
            CaseExpressions.AddRange( otherCase.CaseExpressions );
            CriteriaAnalysis.AddRange( otherCase.CriteriaAnalysis );
            if( otherCase.HasError )
                HasError = true;
        }


        public void FillExpressionSyntax( List<ExpressionSyntax> result )
        {
            foreach( var expression in CaseExpressions )
                if( !result.Contains( expression ) )
                    result.Add( expression );
        }


        public override bool Equals( object other ) => Equals( other as Case );


        private bool Equals( Case other ) => TestedParameter.Equals( other.TestedParameter ) &&
                                             // ReSharper disable once PossibleNullReferenceException
                                             CriteriaAnalysis.Equivalent( other.CriteriaAnalysis );


        public override int GetHashCode() => CriteriaAnalysis.Aggregate( TestedParameter.GetHashCode(), ( current, field ) => current ^ field.GetHashCode() );

#endif


        public void ToString( StringBuilder result )
        {
            if( ParameterName != NoParameter )
            {
                result.Append( ParameterPath );
                result.Append( ':' );
            }

            foreach( var field in Expressions )
            {
                result.Append( field );
                result.Append( " & " );
            }

            result.Length -= 3;
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            ToString( result );
            return result.ToString();
        }
    }
}