using System;
using System.ComponentModel;



namespace Example01
{
    public enum TransactionKind
    {
        // ReSharper disable once UnusedMember.Global
        None,
        Deposit,
        Withdraw,
        Transfer
    }


    public class Transaction
    {
        internal Transaction( TransactionKind kind, double amount, Account account, Account secondAccount = null )
        {
            if( kind == TransactionKind.None ||
                !Enum.IsDefined( typeof(TransactionKind), kind ) )
                throw new InvalidEnumArgumentException( nameof(kind), (int)kind, typeof(TransactionKind) );
            if( kind == TransactionKind.Transfer )
            {
                if( secondAccount == null )
                    throw new ArgumentNullException( nameof(secondAccount) );
            }
            else if( secondAccount != null )
                throw new ArgumentOutOfRangeException( nameof(secondAccount) );


            Date = DateTime.Now;
            Kind = kind;
            Amount = amount;
            Account = account ?? throw new ArgumentNullException( nameof(account) );
            SecondAccount = secondAccount;
        }


        public DateTime Date { get; }
        public TransactionKind Kind { get; }


        public double Amount { get; }

        public Account Account { get; }
        public Account SecondAccount { get; }
    }
}