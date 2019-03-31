using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    public class TestedParameter
    {
        public TestedParameter( SemanticModel model, ExpressionSyntax expression )
        {
            Expression = expression;
            if( expression == null )
            {
                Name = Case.NoParameter;
                Path = Name;
                return;
            }

            if( model.GetConstantValue( Expression ).Value is string result )
            {
                Name = result;
                Path = Name;
                return;
            }

            // Lambda?
            if( !( Expression is ParenthesizedLambdaExpressionSyntax lambda ) )
            {
                Name = Case.NoParameter;
                Path = Name;
                return;
            }

            IsLambda = true;
            var parameter = lambda.ParameterList.Parameters[ 0 ];
            Type = (ITypeSymbol)model.GetSymbol( parameter.Type );
            Name = parameter.Identifier.Text;
            Path = Name;
            PathExpression = lambda.Body as ExpressionSyntax;
            if( PathExpression == null )
                PathHasErrors = true;
            else
            {
                var pathVisitor = new PathVisitor( Name );
                pathVisitor.Visit( PathExpression );
                PathHasErrors = pathVisitor.HasErrors;
                Path = PathHasErrors
                           ? PathExpression.ToString()
                           : pathVisitor.PathStr;
            }
        }


        public TestedParameter( string parameterName, string parameterPath = null)
        {
            Name = parameterName;
            Path = parameterPath ?? Name;
        }


        public ExpressionSyntax Expression { get; }

        public string Name { get; }
        public ITypeSymbol Type { get; }
        public bool IsLambda { get; }
        public ExpressionSyntax PathExpression { get; }
        public string Path { get; }
        public bool PathHasErrors { get; }


        protected bool Equals( TestedParameter other ) => string.Equals( Name, other.Name ) && string.Equals( Path, other.Path );


        public override bool Equals( object obj )
        {
            if( ReferenceEquals( null, obj ) ) return false;
            if( ReferenceEquals( this, obj ) ) return true;
            if( obj.GetType() != GetType() ) return false;
            return Equals( (TestedParameter)obj );
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return ( ( Name?.GetHashCode() ?? 0 ) * 397 ) ^ ( Path?.GetHashCode() ?? 0 );
            }
        }
    }
}