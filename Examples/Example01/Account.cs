using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace Example01
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Account: INotifyPropertyChanged
    {
        private static int _IdGenerator = 1;


        internal Account( Customer customer )
        {
            Customer = customer ?? throw new ArgumentNullException( nameof(customer) );
            Id = ++_IdGenerator;
        }


        public Customer Customer { get; }

        public int Id { get; }


        #region Balance Property

        private double _Balance;

        public double Balance
        {
            get => _Balance;
            private set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if( value == _Balance )
                    return;
                _Balance = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        private readonly List<Transaction> _Transactions = new List<Transaction>();
        public IReadOnlyList<Transaction> Transactions => _Transactions;


        public void Deposit( double amount )
        {
            if( amount <= 0 )
                throw new ArgumentOutOfRangeException( nameof(amount) );

            Balance += amount;
            _Transactions.Add( new Transaction( TransactionKind.Deposit, amount, this ) );
        }


        public bool Withdraw( double amount )
        {
            if( amount <= 0 )
                throw new ArgumentOutOfRangeException( nameof(amount) );

            if( amount > Balance )
                return false;

            Balance -= amount;
            _Transactions.Add( new Transaction( TransactionKind.Withdraw, -amount, this ) );
            return true;
        }


        public bool Transfer( double amount, Account toAccount )
        {
            if( amount <= 0 )
                throw new ArgumentOutOfRangeException( nameof(amount) );
            if( toAccount == null )
                throw new ArgumentNullException( nameof(toAccount) );

            if( amount > Balance )
                return false;

            Balance -= amount;
            _Transactions.Add( new Transaction( TransactionKind.Transfer, -amount, this, toAccount ) );
            toAccount.Balance += amount;
            toAccount._Transactions.Add( new Transaction( TransactionKind.Transfer, amount, toAccount, this ) );
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}