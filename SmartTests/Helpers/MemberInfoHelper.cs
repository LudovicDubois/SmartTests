using System;
using System.Reflection;
using System.Text;



namespace SmartTests.Helpers
{
    public static class MemberInfoHelper
    {
        public static string GetFullName( this MemberInfo @this )
        {
            var result = new StringBuilder();
            AppendType( @this.DeclaringType, result );
            result.Append( @this.MemberType == MemberTypes.NestedType ? '+' : '.' );
            result.Append( @this.Name );
            return result.ToString();
        }


        private static void AppendType( Type type, StringBuilder result )
        {
            if( type.DeclaringType != null )
            {
                AppendType( type.DeclaringType, result );
                result.Append( "+" );
            }

            result.Append( type.Name );
        }
    }
}
