using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace Example01
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Customer: INotifyPropertyChanged
    {
        private static int _IdGenerator;


        public Customer( string name, string address )
        {
            if( name == null )
                throw new ArgumentNullException( nameof(name) );
            if( address == null )
                throw new ArgumentNullException( nameof(address) );
            if( string.IsNullOrEmpty( name ) )
                throw new ArgumentOutOfRangeException( nameof(name) );
            if( string.IsNullOrEmpty( address ) )
                throw new ArgumentOutOfRangeException( nameof(address) );

            Id = ++_IdGenerator;
            Name = name;
            Address = address;
        }


        public int Id { get; }

        public string Name { get; }


        #region Address Property

        private string _Address;

        public string Address
        {
            get => _Address;
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if( value == _Address )
                    return;
                if( value == null )
                    throw new ArgumentNullException( nameof(value), "Address cannot be empty nor null!" );
                if( string.IsNullOrEmpty( value ) )
                    throw new ArgumentOutOfRangeException( nameof(value), "Address cannot be empty nor null!" );

                _Address = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        public Account CreateAccount()
        {
            var result = new Account( this );
            _Accounts.Add( result );
            return result;
        }


        public bool CloseAccount( Account account )
        {
            if( account == null )
                throw new ArgumentNullException( nameof(account), "account belonging to this customer is required" );
            return _Accounts.Remove( account );
        }


        #region Accounts Property

        private readonly ObservableCollection<Account> _Accounts = new ObservableCollection<Account>();

        public IReadOnlyList<Account> Accounts => _Accounts;

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}