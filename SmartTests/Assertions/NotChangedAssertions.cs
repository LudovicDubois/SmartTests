// ReSharper disable UnusedParameter.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using SmartTests.Helpers;



namespace SmartTests.Assertions
{
    [Flags]
    public enum NotChangedKind
    {
        PublicProperties = Visibility.Public,
        ProtectedProperties = Visibility.Protected,
        InternalProperties = Visibility.Internal,
        PrivateProperties = Visibility.Private,
        NonPublicProperties = ProtectedProperties | InternalProperties | PrivateProperties,
        VisibleProperties = PublicProperties | ProtectedProperties,
        AllProperties = PublicProperties | ProtectedProperties | InternalProperties | PrivateProperties,
        PublicFields = Visibility.Public << 4,
        ProtectedFields = Visibility.Protected << 4,
        InternalFields = Visibility.Internal << 4,
        PrivateFields = Visibility.Private << 4,
        NonPublicFields = ProtectedFields | InternalFields | PrivateFields,
        VisibleFields = PublicFields | ProtectedFields,
        AllFields = PublicFields | ProtectedFields | InternalFields | PrivateFields,
        All = AllProperties | AllFields
    }


    public static class NotChangedAssertions
    {
        public static Assertion NotChanged( this SmartAssertPlaceHolder @this, NotChangedKind kind = NotChangedKind.PublicProperties ) => new NotChangedAssertion( null, kind, null );
        public static Assertion NotChanged( this SmartAssertPlaceHolder @this, object instance, NotChangedKind kind = NotChangedKind.PublicProperties ) => new NotChangedAssertion( instance, kind, null );

        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this ) => new NotChangedAssertion( true );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, params string[] exceptions ) => new NotChangedAssertion( null, NotChangedKind.PublicProperties, exceptions );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, NotChangedKind kind, params string[] exceptions ) => new NotChangedAssertion( null, kind, exceptions );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, object instance, params string[] exceptions ) => new NotChangedAssertion( instance, NotChangedKind.PublicProperties, exceptions );
        public static Assertion NotChangedExcept( this SmartAssertPlaceHolder @this, object instance, NotChangedKind kind, params string[] exceptions ) => new NotChangedAssertion( instance, kind, exceptions );


        public static Assertion NotChangedExcept<T>( this SmartAssertPlaceHolder @this, Expression<Func<T>> expression, NotChangedKind kind = NotChangedKind.PublicProperties )
        {
            object instance;
            PropertyInfo property;
            GetInstanceAndProperty( expression, out instance, out property );
            return new NotChangedAssertion( instance, kind, new[] { property.Name } );
        }


        private static void GetInstanceAndProperty<T>( Expression<Func<T>> expression, out object instance, out PropertyInfo property )
        {
            MemberInfo member;
            if( !expression.GetMemberContext( out instance, out member ) )
                throw new BadTestException( string.Format( Resource.BadTest_NotPropertyNorIndexer ) );

            property = member as PropertyInfo;
            if( property == null )
                throw new BadTestException( string.Format( Resource.BadTest_NotPropertyNorIndexer, member.GetFullName() ) );
        }


        private class NotChangedAssertion: Assertion
        {
            public NotChangedAssertion( object instance, NotChangedKind kind, string[] exceptions )
            {
                _Instance = instance;
                _Kind = kind;
                _Exceptions = exceptions;
            }


            public NotChangedAssertion( bool isImplicit )
            {
                _IsImplicit = isImplicit;
                _Instance = null;
                _Kind = NotChangedKind.PublicProperties;
                _Exceptions = null;
            }


            private readonly bool _IsImplicit;
            private object _Instance;
            private readonly NotChangedKind _Kind;
            private string[] _Exceptions;
            private PropertyInfo[] _Properties;
            private FieldInfo[] _Fields;
            private readonly Dictionary<PropertyInfo, object> _PropertyValues = new Dictionary<PropertyInfo, object>();
            private readonly Dictionary<FieldInfo, object> _FieldValues = new Dictionary<FieldInfo, object>();


            public override void BeforeAct( ActBase act )
            {
                if( _Instance == null )
                {
                    _Instance = act.Instance;
                    if( _IsImplicit )
                        _Exceptions = new[] { act.Property.Name };
                }

                var propertiesVisibility = (Visibility)( _Kind & NotChangedKind.AllProperties );
                var instanceType = _Instance.GetType();
                _Properties = propertiesVisibility != 0
                                  ? instanceType.GetProperties( propertiesVisibility )
                                  : new PropertyInfo[0];

                var fieldsVisibility = (Visibility)( (int)( _Kind & NotChangedKind.AllFields ) >> 4 );
                _Fields = fieldsVisibility != 0
                              ? instanceType.GetFields( fieldsVisibility )
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
                    throw new BadTestException( message.ToString( Environment.NewLine.Length, message.Length - Environment.NewLine.Length ) );
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