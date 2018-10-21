using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer
{
    abstract class BaseVisitor
    {
        protected BaseVisitor( SemanticModel model, Action<Diagnostic> reportDiagnostic )
        {
            Model = model;
            ReportDiagnostic = reportDiagnostic;
        }


        protected SemanticModel Model { get; }
        protected Action<Diagnostic> ReportDiagnostic { get; }


        protected bool TryGetConstant<T>( ExpressionSyntax expression, out T value )
        {
            var constant = Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( expression ) );
                value = default(T);
                return false;
            }

            value = (T)Convert.ChangeType( constant.Value, typeof(T) );
            return true;
        }


        protected bool TryGetConstant( ExpressionSyntax expression, out bool value )
        {
            var constant = Model.GetConstantValue( expression );
            if( !constant.HasValue )
            {
                ReportDiagnostic( SmartTestsDiagnostics.CreateNotAConstant( expression ) );
                value = default(bool);
                return false;
            }

            value = (bool)Convert.ChangeType( constant.Value, typeof(bool) );
            return true;
        }
    }
}