using System.Collections.Generic;
using System.Text;

using JetBrains.Annotations;

#if !EXTENSION

using System.Diagnostics;
using System.Linq;


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Newtonsoft.Json;

using SmartTestsAnalyzer.Helpers;

#endif



namespace SmartTestsAnalyzer
{
    [UsedImplicitly( ImplicitUseTargetFlags.WithMembers )]
    public class Case
    {
        public static string NoParameter => "";


#if EXTENSION

        public string ParameterName { get; }
        public List<string> Expressions { get; } = new List<string>();
        public bool HasError { get; set; }


#else
        public Case( ExpressionSyntax parameterNameExpression, string parameterName )
        {
            Debug.Assert( parameterName != null );
            ParameterNameExpression = parameterNameExpression;
            ParameterName = parameterName;
        }


        [JsonIgnore]
        public ExpressionSyntax ParameterNameExpression { get; }
        public string ParameterName { get; }
        [JsonIgnore]
        public List<ExpressionSyntax> CaseExpressions { get; } = new List<ExpressionSyntax>();
        public List<string> Expressions => CriteriaFields.Select( criteria => criteria.ToDisplayString( SymbolDisplayFormat.CSharpShortErrorMessageFormat ) ).ToList();
        [JsonIgnore]
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


        public void FillCriteriaTypes( HashSet<ITypeSymbol> types )
        {
            foreach( var field in CriteriaFields )
                types.Add( field.ContainingType );
        }


        public void FillWith( Case otherCase )
        {
            Debug.Assert( otherCase.ParameterName == ParameterName );
            CaseExpressions.AddRange( otherCase.CaseExpressions );
            CriteriaFields.AddRange( otherCase.CriteriaFields );
            if( otherCase.HasError )
                HasError = true;
        }


        public void FillExpressionSyntax( List<ExpressionSyntax> result )
        {
            foreach( var expression in CaseExpressions )
                if( !result.Contains( expression ) )
                    result.Add( expression );
        }

        public override bool Equals(object other) => Equals(other as Case);


        private bool Equals(Case other) => string.Equals(ParameterName, other?.ParameterName) &&
                                             // ReSharper disable once PossibleNullReferenceException
                                             CriteriaFields.Equivalent(other.CriteriaFields);


        public override int GetHashCode() => CriteriaFields.Aggregate(ParameterName.GetHashCode(), (current, field) => current ^ field.GetHashCode());

#endif


        public void ToString( StringBuilder result )
        {
            if( ParameterName != NoParameter )
            {
                result.Append( ParameterName );
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