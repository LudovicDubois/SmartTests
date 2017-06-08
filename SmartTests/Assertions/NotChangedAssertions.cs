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
        PublicFields = 0b0000_0100,
        NonPublicFields = 0b0000_1000,
        AllFields = PublicFields | NonPublicFields,
        All = AllProperties | AllFields
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
            private FieldInfo[] _Fields;
            private readonly Dictionary<PropertyInfo, object> _PropertyValues = new Dictionary<PropertyInfo, object>();
            private readonly Dictionary<FieldInfo, object> _FieldValues = new Dictionary<FieldInfo, object>();


            private static readonly Dictionary<NotChangedKind, BindingFlags> _Flags = new Dictionary<NotChangedKind, BindingFlags>
                                                                                      {
                                                                                          [ NotChangedKind.NonPublicProperties ] = BindingFlags.Instance | BindingFlags.NonPublic,
                                                                                          [ NotChangedKind.PublicProperties ] = BindingFlags.Instance | BindingFlags.Public,
                                                                                          [ NotChangedKind.AllProperties ] = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                                          [ NotChangedKind.NonPublicFields ] = BindingFlags.Instance | BindingFlags.NonPublic,
                                                                                          [ NotChangedKind.PublicFields ] = BindingFlags.Instance | BindingFlags.Public,
                                                                                          [ NotChangedKind.AllFields ] = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                                      };


            public override void BeforeAct( ActBase act )
            {
                if( _Instance == null )
                    _Instance = act.Instance;

                var propertiesKind = _Kind & NotChangedKind.AllProperties;
                var instanceType = _Instance.GetType();
                _Properties = propertiesKind != 0
                                  ? instanceType.GetProperties( _Flags[ propertiesKind ] )
                                  : new PropertyInfo[0];

                var fieldsKind = _Kind & NotChangedKind.AllFields;
                _Fields = fieldsKind != 0
                              ? instanceType.GetFields( _Flags[ fieldsKind ] )
                              : new FieldInfo[0];

                if( _Exceptions != null )
                {
                    CheckExceptions();
                    _Properties = _Properties.Where( prop => !_Exceptions.Contains( prop.Name ) ).ToArray();
                    _Fields = _Fields.Where( field => !_Exceptions.Contains( field.Name ) ).ToArray();
                }

                foreach( var property in _Properties )
                    _PropertyValues[ property ] = property.GetValue( _Instance );
                foreach( var field in _Fields )
                    _FieldValues[ field ] = field.GetValue( _Instance );
            }


            private void CheckExceptions()
            {
                var names = _Properties.Select( prop => prop.Name ).ToList();
                names.AddRange( _Fields.Select( field => field.Name ) );

                var message = new StringBuilder();
                foreach( var exception in _Exceptions )
                {
                    if( !names.Contains( exception ) )
                    {
                        message.AppendLine();
                        message.Append( string.Format( Resource.BadTest_NotPropertyNorField, exception, _Instance.GetType().GetFullName() ) );
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
                foreach( var pair in _FieldValues )
                    if( !Equals( _FieldValues[ pair.Key ], pair.Key.GetValue( _Instance ) ) )
                    {
                        message.AppendLine();
                        message.Append( string.Format( Resource.FieldChanged, pair.Key.GetFullName() ) );
                    }

                if( message.Length > 0 )
                    throw new SmartTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
            }
        }
    }
}