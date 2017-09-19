using System;
using System.Linq;
using System.Reflection;
using System.Text;



namespace SmartTests.Helpers
{
    [Flags]
    public enum Visibility
    {
        Public = 1,
        Protected = 2,
        Internal = 4,
        Private = 8
    }


    public static class MemberInfoHelper
    {
        public static string GetFullName( this MemberInfo @this )
        {
            var result = new StringBuilder();
            AppendType( @this.DeclaringType, result );
            result.Append( '.' );
            result.Append( @this.Name );
            return result.ToString();
        }


        public static string GetFullName( this Type @this )
        {
            var result = new StringBuilder();
            AppendType( @this.DeclaringType, result );
            result.Append( @this.IsNested ? '+' : '.' );
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


        public static PropertyInfo[] GetProperties( this Type @this, Visibility visibility )
        {
            return @this.GetRuntimeProperties().Where( prop => IsVisibility( prop.GetMethod ?? prop.SetMethod, visibility ) ).ToArray();
        }


        public static FieldInfo[] GetFields( this Type @this, Visibility visibility )
        {
            return @this.GetRuntimeFields().Where( field => IsVisibility( field, visibility ) ).ToArray();
        }


        private static bool IsVisibility( MethodInfo method, Visibility visibility )
        {
            if( method.IsPublic &&
                ( visibility & Visibility.Public ) != 0 )
                return true;

            if( ( method.IsFamily || method.IsFamilyOrAssembly ) &&
                ( visibility & Visibility.Protected ) != 0 )
                return true;

            if( ( method.IsAssembly || method.IsFamilyOrAssembly ) &&
                ( visibility & Visibility.Internal ) != 0 )
                return true;

            if( method.IsPrivate &&
                ( visibility & Visibility.Private ) != 0 )
                return true;

            return false;
        }


        private static bool IsVisibility( FieldInfo field, Visibility visibility )
        {
            if( field.IsPublic &&
                ( visibility & Visibility.Public ) != 0 )
                return true;

            if( ( field.IsFamily || field.IsFamilyOrAssembly ) &&
                ( visibility & Visibility.Protected ) != 0 )
                return true;

            if( ( field.IsAssembly || field.IsFamilyOrAssembly ) &&
                ( visibility & Visibility.Internal ) != 0 )
                return true;

            if( field.IsPrivate &&
                ( visibility & Visibility.Private ) != 0 )
                return true;

            return false;
        }
    }
}