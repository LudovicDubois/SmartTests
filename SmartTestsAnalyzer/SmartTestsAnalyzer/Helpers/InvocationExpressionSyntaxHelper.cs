using System;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace SmartTestsAnalyzer.Helpers
{
    public static class InvocationExpressionSyntaxHelper
    {
        public static ArgumentSyntax GetArgument( [NotNull] this InvocationExpressionSyntax @this, int index )
        {
            if( @this == null )
                throw new ArgumentNullException( nameof( @this ) );
            if( index < 0 )
                throw new ArgumentOutOfRangeException( nameof( index ) );

            var args = @this.ArgumentList.Arguments;
            return index < args.Count
                       ? args[ index ]
                       : null;
        }
    }
}