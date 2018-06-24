using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SmartTests;
using SmartTests.Ranges;

using SmartTestsAnalyzer.Criterias;
using SmartTestsAnalyzer.Helpers;



namespace SmartTestsAnalyzer
{
    class CriteriaVisitor: CSharpSyntaxVisitor<CasesAndOr>
    {
        public CriteriaVisitor( SemanticModel model, ExpressionSyntax casesExpression, ExpressionSyntax parameterNameExpression, Action<Diagnostic> reportDiagnostic )
        {
            _Model = model;
            _CasesExpression = casesExpression;
            _ParameterNameExpression = parameterNameExpression;
            _ReportDiagnostic = reportDiagnostic;
            _ErrorAttribute = _Model.Compilation.GetTypeByMetadataName( "SmartTests.ErrorAttribute" );
            Debug.Assert( _ErrorAttribute != null );

            var smartTestType = _Model.Compilation.GetTypeByMetadataName( "SmartTests.SmartTest" );
            // SmartTest type roots
            AddType( smartTestType, "Int", () => SmartTest.Int );

            // SmartTest type extension methods
            AddRangeExtension( smartTestType, "Range", node => Range<int>( node, ( type, min, max ) => type.Range( min, max ) ) );
            AddRangeExtension( smartTestType, "AboveOrEqual", node => Range<int>( node, ( type, min ) => type.Range( min, type.MaxValue ) ) );
            AddRangeExtension( smartTestType, "Above", node => Range<int>( node, ( type, min ) => type.Range( type.GetNext( min ), type.MaxValue ) ) );
            AddRangeExtension( smartTestType, "BelowOrEqual", node => Range<int>( node, ( type, max ) => type.Range( type.MinValue, max ) ) );
            AddRangeExtension( smartTestType, "Below", node => Range<int>( node, ( type, max ) => type.Range( type.MinValue, type.GetPrevious( max ) ) ) );

            // IType<T> methods
            AddITypeTMethods();
        }


        private readonly SemanticModel _Model;
        private readonly ExpressionSyntax _CasesExpression;
        private readonly ExpressionSyntax _ParameterNameExpression;
        private readonly Action<Diagnostic> _ReportDiagnostic;
        private readonly INamedTypeSymbol _ErrorAttribute;
        private readonly Dictionary<IPropertySymbol, Func<CasesAndOr>> _TypeProperties = new Dictionary<IPropertySymbol, Func<CasesAndOr>>();
        private readonly Dictionary<IMethodSymbol, Func<InvocationExpressionSyntax, CasesAndOr>> _RangeMethods = new Dictionary<IMethodSymbol, Func<InvocationExpressionSyntax, CasesAndOr>>();


        private void AddType( ITypeSymbol smartTestType, string propertyName, Func<IType> iTypeCreator )
        {
            var rootProperty = (IPropertySymbol)smartTestType.GetMembers( propertyName )[ 0 ];
            Debug.Assert( rootProperty != null, $"Problem with SmartTest.{propertyName} property" );
            _TypeProperties[ rootProperty ] = () => Root( iTypeCreator() );
        }


        private void AddRangeExtension( ITypeSymbol smartTestType, string methodName, Func<InvocationExpressionSyntax, CasesAndOr> func )
        {
            var rangeMethods = smartTestType.GetMethods( methodName );
            foreach( var rangeMethod in rangeMethods )
            {
                Debug.Assert( rangeMethod != null, $"Problem with SmartTest.{methodName}<T> method" );
                _RangeMethods[ rangeMethod ] = func;
            }
        }


        private void AddITypeTMethods()
        {
            var typeName = typeof(IType<>).FullName;
            var iTypeType = _Model.Compilation.GetTypeByMetadataName( typeName );
            var rangeMethod = iTypeType.GetMethods( "Range" )[ 0 ];
            Debug.Assert( rangeMethod.Parameters.Length == 2, $"Problem with {typeName}.Range(int, int) method" );
            _RangeMethods[ rangeMethod ] = Range<int>;

            // GetValue
            var getValueMethod = iTypeType.GetMethods( "GetValue" )[ 0 ];
            Debug.Assert( getValueMethod.Parameters.Length == 1, $"Problem with {typeName}.GetValue(out int) method" );
            Debug.Assert( getValueMethod.Parameters[ 0 ].RefKind == RefKind.Out, $"Problem with {typeName}.GetValue(out int) method" );
            _RangeMethods[ getValueMethod ] = GetValue;
        }


        // Visit Methods


        public override CasesAndOr VisitIdentifierName( IdentifierNameSyntax node )
        {
            if( _Model.GetSymbol( node ) is IPropertySymbol propertySymbol &&
                _TypeProperties.TryGetValue( propertySymbol, out var func ) )
                return func();

            return base.VisitIdentifierName( node );
        }


        public override CasesAndOr VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            var member = _Model.GetSymbol( node );
            if( member is IFieldSymbol criteria )
            {
                var parameterName = _ParameterNameExpression != null
                                        ? _Model.GetConstantValue( _ParameterNameExpression ).Value as string
                                        : null;
                return new CasesAndOr( _CasesExpression, _ParameterNameExpression, parameterName ?? Case.NoParameter, new FieldAnalysis( criteria ), criteria.HasAttribute( _ErrorAttribute ) );
            }

            if( member is IPropertySymbol type )
                if( _TypeProperties.TryGetValue( type, out var func ) )
                    return func();

            return node.Expression.Accept( this );
        }


        public override CasesAndOr VisitParenthesizedExpression( ParenthesizedExpressionSyntax node )
        {
            return node.Expression.Accept( this );
        }


        public override CasesAndOr VisitBinaryExpression( BinaryExpressionSyntax node )
        {
            var leftCriteria = node.Left.Accept( this );
            if( leftCriteria == null )
                return null;
            var rightCriteria = node.Right.Accept( this );
            if( rightCriteria == null )
                return null;

            switch( node.OperatorToken.Kind() )
            {
                case SyntaxKind.AmpersandToken:
                    leftCriteria.CombineAnd( rightCriteria );
                    return leftCriteria;

                case SyntaxKind.BarToken:
                    leftCriteria.CombineOr( rightCriteria );
                    return leftCriteria;

                default:
                    return null;
            }
        }


        // Analysis Methods

        private IType _CurrentType;


        private CasesAndOr Root( IType currentType )
        {
            _CurrentType = currentType;
            return new CasesAndOr( _CasesExpression, _ParameterNameExpression, Case.NoParameter, new RangeAnalysis( _CurrentType ), false );
        }


        private bool TryGetConstant<T>( ExpressionSyntax expression, out T value )
            where T: struct
        {
            var constant = _Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                _ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( expression ) );
                value = default(T);
                return false;
            }
            value = (T)constant.Value;
            return true;
        }


        private CasesAndOr Range<T>( InvocationExpressionSyntax node, Action<IType<T>, T, T> addRange )
            where T: struct, IComparable<T>
        {
            if( !( TryGetConstant( node.GetArgument( 0 ).Expression, out T min ) &
                   TryGetConstant( node.GetArgument( 1 ).Expression, out T max ) ) )
                return base.VisitInvocationExpression( node );

            var result = node.Expression.Accept( this );
            addRange( (IType<T>)_CurrentType, min, max );
            return result;
        }


        private CasesAndOr Range<T>( InvocationExpressionSyntax node, Action<IType<T>, T> addRange )
            where T: struct, IComparable<T>
        {
            if( !TryGetConstant( node.GetArgument( 0 ).Expression, out T min ) )
                return base.VisitInvocationExpression( node );

            var result = node.Expression.Accept( this );
            addRange( (IType<T>)_CurrentType, min );
            return result;
        }


        private CasesAndOr Range<T>( InvocationExpressionSyntax node )
            where T: struct, IComparable<T>
        {
            var result = node.Expression.Accept( this );

            if( !( TryGetConstant( node.GetArgument( 0 ).Expression, out T min ) &
                   TryGetConstant( node.GetArgument( 1 ).Expression, out T max ) ) )
                return base.VisitInvocationExpression( node );

            ( (IType<T>)_CurrentType ).Range( min, max );

            if( node.ArgumentList.Arguments.Count == 3 )
                _CurrentType = null;

            return result;
        }


        private CasesAndOr GetValue( InvocationExpressionSyntax node )
        {
            var result = node.Expression.Accept( this );
            _CurrentType = null;
            return result;
        }


        public override CasesAndOr VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var criteria = _Model.GetSymbol( node ) as IMethodSymbol;
            if( criteria == null )
                return base.VisitInvocationExpression( node );


            if (_RangeMethods.TryGetValue(criteria, out var func))
                return Analyze(node);

            if( criteria.ReducedFrom != null &&
                _RangeMethods.TryGetValue( criteria.ReducedFrom, out func ) )
                return Analyze(node);

            if ( criteria.OriginalDefinition != null &&
                _RangeMethods.TryGetValue( criteria.OriginalDefinition, out func ) )
                return Analyze(node);

            return base.VisitInvocationExpression( node );
        }


        private CasesAndOr Analyze(InvocationExpressionSyntax node)
        {
            var typeSearchVisitor = new TypeSearchVisitor(_Model, _ReportDiagnostic, node);
            var iType = node.Accept(typeSearchVisitor);
            if (iType == null)
                // There was an error
                return null;
            return new CasesAndOr(_CasesExpression, _ParameterNameExpression, Case.NoParameter, new RangeAnalysis(iType), false);
        }
    }
}