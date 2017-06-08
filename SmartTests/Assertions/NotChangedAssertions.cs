// ReSharper disable UnusedParameter.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SmartTests.Helpers;



namespace SmartTests.Assertions
{
    [Flags]
    public enum NotChangedKind
    {
        PublicProperties = 0b0000_00001,
        NonPublicProperties = 0b0000_0010,
        AllProperties = PublicProperties | NonPublicProperties,
    }


    public static class NotChangedAssertions
    {
        public static Assertion NotChanged( this SmartAssertPlaceHolder @this, NotChangedKind kind = NotChangedKind.PublicProperties ) => new NotChangedAssertion( null, kind, null );
        public static Assertion NotChanged( this SmartAssertPlaceHolder @this, object instance, NotChangedKind kind = NotChangedKind.PublicProperties ) => new NotChangedAssertion( instance, kind, null );

        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, params string[] exceptions ) => new NotChangedAssertion( null, NotChangedKind.PublicProperties, exceptions );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, NotChangedKind kind, params string[] exceptions ) => new NotChangedAssertion( null, kind, exceptions );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, object instance, params string[] exceptions ) => new NotChangedAssertion( instance, NotChangedKind.PublicProperties, exceptions );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, object instance, NotChangedKind kind, params string[] exceptions ) => new NotChangedAssertion( instance, kind, exceptions );


        private class NotChangedAssertion: Assertion
        {
            public NotChangedAssertion( object instance, NotChangedKind kind, string[] exceptions )
            {
                _Instance = instance;
                _Kind = kind;
                _Exceptions = exceptions;
            }


            private object _Instance;
            private readonly NotChangedKind _Kind;
            private readonly string[] _Exceptions;
            private PropertyInfo[] _Properties;
            private readonly Dictionary<PropertyInfo, object> _PropertyValues = new Dictionary<PropertyInfo, object>();


            private static readonly Dictionary<NotChangedKind, BindingFlags> _Flags = new Dictionary<NotChangedKind, BindingFlags>
                                                                                      {
                                                                                          [ NotChangedKind.NonPublicProperties ] = BindingFlags.Instance | BindingFlags.NonPublic,
                                                                                          [ NotChangedKind.PublicProperties ] = BindingFlags.Instance | BindingFlags.Public,
                                                                                          [ NotChangedKind.AllProperties ] = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                                      };


            public override void BeforeAct( ActBase act )
            {
                if( _Instance == null )
                    _Instance = act.Instance;

                _Properties = _Instance.GetType().GetProperties( _Flags[ _Kind ] );
                if( _Exceptions != null )
                {
                    CheckExceptions();
                    _Properties = Enumerable.ToArray( Enumerable.Where( _Properties, prop => !Enumerable.Contains( _Exceptions, prop.Name ) ) );
                }

                foreach( var property in _Properties )
                    _PropertyValues[ property ] = property.GetValue( _Instance );
            }


            private void CheckExceptions()
            {
                var propertyNames = _Properties.Select( prop => prop.Name ).ToList();

                var message = new StringBuilder();
                foreach( var exception in _Exceptions )
                {
                    if( !propertyNames.Contains( exception ) )
                    {
                        message.AppendLine();
                        message.Append( string.Format( Resource.BadTest_NotProperty, exception, _Instance.GetType().GetFullName() ) );
                    }
                }

                if( message.Length > 0 )
                    throw new SmartTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
            }


            public override void AfterAct( ActBase act )
            {
                var message = new StringBuilder();
                foreach( var pair in _PropertyValues )
                    if( !Equals( _PropertyValues[ pair.Key ], pair.Key.GetValue( _Instance ) ) )
                    {
                        message.AppendLine();
                        message.Append( string.Format( Resource.PropertyChanged, pair.Key.GetFullName() ) );
                    }

                if( message.Length > 0 )
                    throw new SmartTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
            }
        }
    }
}