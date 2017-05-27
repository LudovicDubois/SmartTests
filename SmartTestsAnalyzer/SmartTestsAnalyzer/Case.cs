using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    public class Case
    {
        public Case( ExpressionSyntax parameterNameExpression, string parameterName )
        {
            ParameterName = parameterName ?? MemberTestCases.NoParameter;
            ParameterNameExpression = parameterNameExpression;
        }


        public ExpressionSyntax ParameterNameExpression { get; }
        public string ParameterName { get; }
        public List<ExpressionSyntax> CaseExpressions { get; } = new List<ExpressionSyntax>();
        public List<IFieldSymbol> CriteriaFields { get; } = new List<IFieldSymbol>();
        public bool HasError { get; private set; }


        public void Add( ExpressionSyntax caseExpression, IFieldSymbol criterion, bool hasError )
        {
            if( caseExpression != null )
                CaseExpressions.Add( caseExpression );

            CriteriaFields.Add( criterion );
            if( hasError )
                HasError = true;
        }


        public void FillWith( Case otherCase )
        {
            Debug.Assert( otherCase.ParameterName == ParameterName );
            CaseExpressions.AddRange( otherCase.CaseExpressions );
            CriteriaFields.AddRange( otherCase.CriteriaFields );
            if( otherCase.HasError )
                HasError = true;
        }


        public void FillWithCriteriaTypes( HashSet<ITypeSymbol> types )
        {
            foreach( var field in CriteriaFields )
                types.Add( field.ContainingType );
        }


        public void FillWithExpressionSyntaxes( List<ExpressionSyntax> result )
        {
            foreach( var expression in CaseExpressions )
                if( !result.Contains( expression ) )
                    result.Add( expression );
        }


        public override bool Equals( object other ) => Equals( other as Case );


        private bool Equals( Case other ) => string.Equals( ParameterName, other?.ParameterName ) &&
                                             CriteriaFields.Equivalent( other.CriteriaFields );


        public override int GetHashCode()
        {
            var hashCode = ParameterName.GetHashCode();
            return CriteriaFields.Aggregate( hashCode, ( current, field ) => current ^ field.GetHashCode() );
        }


        public void ToString( StringBuilder result )
        {
            if( ParameterName != MemberTestCases.NoParameter )
            {
                result.Append( ParameterName );
                result.Append( ':' );
            }
            foreach ( var field in CriteriaFields )
            {
                result.Append( field.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) );
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