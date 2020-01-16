using System;
using System.Linq;

using Example01;

using NUnit.Framework;

using SmartTests.Assertions;
using SmartTests.Criterias;
using SmartTests.Ranges;

using static SmartTests.SmartTest;



namespace Example01Tests
{
    [TestFixture]
    public class AccountTests
    {
        private Customer _Customer;
        private Account _Account;


        [SetUp]
        public void Setup()
        {
            _Customer = new Customer( "Ludovic", "Dubois" );
            _Account = _Customer.CreateAccount();
        }


        #region Deposit Tests

        [Test]
        public void Deposit_HappyPath()
        {
            var lowerNow = DateTime.Now;

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            RunTest( Case( ( double amount ) => amount.Above( 0 ), out var value ),
                     // Act
                     () => _Account.Deposit( value ),

                     // Assert (and implicit Assume)
                     // Before Act: Keep tracks of all public properties
                     // After Act: Assert all public properties have the same values, except Balance
                     SmartAssert.NotChangedExcept( nameof(Account.Balance) ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert PropertyChanged is raised for Balance
                     SmartAssert.Raised_PropertyChanged( _Account, nameof(Account.Balance) ),
                     // Before Act: Compute _Account.Balance + value
                     // After Act: Assert _Account.Balance is previous computed value
                     SmartAssert.Change( () => _Account.Balance + value ),
                     // Before Act: Compute _Account.Transactions.Count + 1
                     // After Act: Assert _Account.Transactions.Count is previous computed value
                     SmartAssert.Change( () => _Account.Transactions.Count + 1 )
                   );

            // Assert the added transaction reflects the Deposit
            var transaction = _Account.Transactions.Last();
            Assert.AreEqual( _Account, transaction.Account );
            Assert.AreEqual( value, transaction.Amount );
            Assert.IsTrue( lowerNow <= transaction.Date && transaction.Date <= DateTime.Now );
            Assert.AreEqual( TransactionKind.Deposit, transaction.Kind );
            Assert.IsNull( transaction.SecondAccount );
        }


        [Test]
        public void Deposit_Negative()
        {
            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            RunTest( Case( ( double amount ) => amount.BelowOrEqual( 0 ), out var value ),
                     // Act
                     () => _Account.Deposit( value ),

                     // Assert (and implicit Assume)
                     // After Act: Assert ArgumentOutOfRangeException is thrown with ParamName == "amount"
                     SmartAssert.Throw<ArgumentOutOfRangeException>( "amount" ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert not PropertyChanged is raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: keep track of all properties and fields
                     // After Act: Assert no property nor field changed => the _Customer is not changed at all
                     SmartAssert.NotChanged( NotChangedKind.All )
                   );
        }

        #endregion


        #region Withdraw Tests

        [Test]
        public void Withdraw_HappyPath()
        {
            var lowerNow = DateTime.Now;
            _Account.Deposit( 1000 );

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            var success = RunTest( Case( ( double amount ) => amount.Range( 0, false, 1000, true ), out var value ),
                                   // Act
                                   () => _Account.Withdraw( value ),

                                   // Assert (and implicit Assume)
                                   // Before Act: Keep tracks of all public properties
                                   // After Act: Assert all public properties have the same values, except Balance
                                   SmartAssert.NotChangedExcept( nameof(Account.Balance) ),
                                   // Before Act: Register to PropertyChangedEvent
                                   // After Act: Assert PropertyChanged is raised for Balance
                                   SmartAssert.Raised_PropertyChanged( _Account, nameof(Account.Balance) ),
                                   // Before Act: Compute _Account.Balance - value
                                   // After Act: Assert _Account.Balance is previous computed value
                                   SmartAssert.Change( () => _Account.Balance - value ),
                                   // Before Act: Compute _Account.Transactions.Count + 1
                                   // After Act: Assert _Account.Transactions.Count is previous computed value
                                   SmartAssert.Change( () => _Account.Transactions.Count + 1 )
                                 );

            Assert.IsTrue( success );
            // Assert the added transaction reflects the Withdraw
            var transaction = _Account.Transactions.Last();
            Assert.AreEqual( _Account, transaction.Account );
            Assert.AreEqual( -value, transaction.Amount );
            Assert.IsTrue( lowerNow <= transaction.Date && transaction.Date <= DateTime.Now );
            Assert.AreEqual( TransactionKind.Withdraw, transaction.Kind );
            Assert.IsNull( transaction.SecondAccount );
        }


        [Test]
        public void Withdraw_Negative()
        {
            _Account.Deposit( 1000 );

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            RunTest( Case( ( double amount ) => amount.BelowOrEqual( 0 ), out var value ),
                     // Act
                     () => _Account.Withdraw( value ),

                     // Assert (and implicit Assume)
                     // After Act: Assert ArgumentOutOfRangeException is thrown with ParamName == "amount"
                     SmartAssert.Throw<ArgumentOutOfRangeException>( "amount" ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert not PropertyChanged is raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: Keep track of all properties and fields
                     // After Act: Assert no property nor field changed => the _Customer is not changed at all
                     SmartAssert.NotChanged( NotChangedKind.All ),
                     // Before Act: Keep track of all public properties of _Account.Transactions
                     // After Act: Assert not property of _Account.Transactions changed (especially Count)
                     SmartAssert.NotChanged( _Account.Transactions )
                   );
        }


        [Test]
        public void Withdraw_TooBig()
        {
            _Account.Deposit( 1000 );
            // Assume
            Assert.AreEqual( 1000, _Account.Balance );

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            var success = RunTest( Case( ( double amount ) => amount.Above( 1000 ), out var value ),
                                   // Act
                                   () => _Account.Withdraw( value ),

                                   // Assert (and implicit Assume)
                                   // Before Act: Register to PropertyChangedEvent
                                   // After Act: Assert not PropertyChanged is raised
                                   SmartAssert.NotRaised_PropertyChanged(),
                                   // Before Act: keep track of all properties and fields
                                   // After Act: Assert no property nor field changed => the _Customer is not changed at all
                                   SmartAssert.NotChanged( NotChangedKind.All ),
                                   // Before Act: Keep track of all public properties of _Account.Transactions
                                   // After Act: Assert not property of _Account.Transactions changed (especially Count)
                                   SmartAssert.NotChanged( _Account.Transactions )
                                 );

            Assert.IsFalse( success );
        }

        #endregion


        #region Transfer Tests

        [Test]
        public void Transfer_HappyPath()
        {
            var lowerNow = DateTime.Now;
            _Account.Deposit( 1000 );
            var account2 = _Customer.CreateAccount();

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            var success = RunTest( Case( ( double amount ) => amount.Range( 0, false, 1000, true ), out var value ) &
                                   Case( "toAccount", ValidValue.IsValid ),

                                   // Act
                                   () => _Account.Transfer( value, account2 ),

                                   // Assert (and implicit Assume)
                                   // Before Act: Keep tracks of all public properties
                                   // After Act: Assert all public properties have the same values, except Balance
                                   SmartAssert.NotChangedExcept( nameof(Account.Balance) ),
                                   // Before Act: Keep tracks of all public properties of account2
                                   // After Act: Assert all public properties of account2 have the same values, except Balance
                                   SmartAssert.NotChangedExcept( account2, nameof(Account.Balance) ),
                                   // Before Act: Register to PropertyChangedEvent
                                   // After Act: Assert PropertyChanged is raised for Balance
                                   SmartAssert.Raised_PropertyChanged( _Account, nameof(Account.Balance) ),
                                   // Before Act: Register to PropertyChangedEvent
                                   // After Act: Assert PropertyChanged is raised for Balance
                                   SmartAssert.Raised_PropertyChanged( account2, nameof(Account.Balance) ),
                                   // Before Act: Compute _Account.Balance - value
                                   // After Act: Assert _Account.Balance is previous computed value
                                   SmartAssert.Change( () => _Account.Balance - value ),
                                   // Before Act: Compute account2.Balance - value
                                   // After Act: Assert account2.Balance is previous computed value
                                   SmartAssert.Change( () => account2.Balance + value ),
                                   // Before Act: Compute _Account.Transactions.Count + 1
                                   // After Act: Assert _Account.Transactions.Count is previous computed value
                                   SmartAssert.Change( () => _Account.Transactions.Count + 1 ),
                                   // Before Act: Compute account2.Transactions.Count + 1
                                   // After Act: Assert account2.Transactions.Count is previous computed value
                                   SmartAssert.Change( () => account2.Transactions.Count + 1 )
                                 );

            Assert.IsTrue( success );
            // Assert the added transaction reflects the Transfer
            var transaction = _Account.Transactions.Last();
            Assert.AreEqual( _Account, transaction.Account );
            Assert.AreEqual( -value, transaction.Amount );
            Assert.IsTrue( lowerNow <= transaction.Date && transaction.Date <= DateTime.Now );
            Assert.AreEqual( TransactionKind.Transfer, transaction.Kind );
            Assert.AreEqual( account2, transaction.SecondAccount );
            // Assert the added transaction reflects the Transfer
            var transaction2 = account2.Transactions.Last();
            Assert.AreEqual( account2, transaction2.Account );
            Assert.AreEqual( value, transaction2.Amount );
            Assert.IsTrue( lowerNow <= transaction2.Date && transaction2.Date <= DateTime.Now );
            Assert.AreEqual( TransactionKind.Transfer, transaction2.Kind );
            Assert.AreEqual( _Account, transaction2.SecondAccount );
        }


        [Test]
        public void Transfer_NegativeAmount()
        {
            _Account.Deposit( 1000 );
            var account2 = _Customer.CreateAccount();

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            RunTest( Case( ( double amount ) => amount.BelowOrEqual( 0 ), out var value ) &
                     Case( "toAccount", ValidValue.IsValid ),

                     // Act
                     () => _Account.Transfer( value, account2 ),

                     // Assert (and implicit Assume)
                     // After Act: Assert ArgumentOutOfRangeException is thrown with ParamName == "amount"
                     SmartAssert.Throw<ArgumentOutOfRangeException>( "amount" ),
                     // Before Act: Keep tracks of all public properties
                     // After Act: Assert all public properties have the same values
                     SmartAssert.NotChanged(),
                     // Before Act: Keep tracks of all public properties
                     // After Act: Assert all public properties have the same values
                     SmartAssert.NotChanged( account2 ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert PropertyChanged is not raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert PropertyChanged is not raised
                     SmartAssert.NotRaised_PropertyChanged( account2 ),
                     // Before Act: Keep track of all public properties of _Account.Transactions
                     // After Act: Assert not property of _Account.Transactions changed (especially Count)
                     SmartAssert.NotChanged( _Account.Transactions ),
                     // Before Act: Keep track of all public properties of account2.Transactions
                     // After Act: Assert not property of account2.Transactions changed (especially Count)
                     SmartAssert.NotChanged( account2.Transactions )
                   );
        }


        [Test]
        public void Transfer_NoAccount2()
        {
            _Account.Deposit( 1000 );

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            RunTest( Case( ( double amount ) => amount.Range( 0, false, 1000, true ), out var value ) &
                     Case( "toAccount", ValidValue.IsInvalid ),

                     // Act
                     () => _Account.Transfer( value, null ),

                     // Assert (and implicit Assume)
                     // After Act: Assert ArgumentNullException is thrown with ParamName == "toAccount"
                     SmartAssert.Throw<ArgumentNullException>( "toAccount" ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert PropertyChanged is not raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: Keep tracks of all public properties
                     // After Act: Assert all public properties have the same values
                     SmartAssert.NotChanged(),
                     // Before Act: Keep track of all public properties of _Account.Transactions
                     // After Act: Assert not property of _Account.Transactions changed (especially Count)
                     SmartAssert.NotChanged( _Account.Transactions )
                   );
        }


        [Test]
        public void Transfer_TooBig()
        {
            _Account.Deposit( 1000 );
            var account2 = _Customer.CreateAccount();

            // Assume
            Assert.AreEqual( 1000, _Account.Balance );

            // We use a lambda expression to show equivalence class and to generate a random value within this equivalence class
            var success = RunTest( Case( ( double amount ) => amount.Above( 1000 ), out var value ) &
                                   Case( "toAccount", ValidValue.IsValid ),

                                   // Act
                                   () => _Account.Transfer( value, account2 ),

                                   // Assert (and implicit Assume)
                                   // Before Act: Keep tracks of all public properties
                                   // After Act: Assert all public properties have the same values
                                   SmartAssert.NotChanged(),
                                   // Before Act: Keep tracks of all public properties of account2
                                   // After Act: Assert all public properties of account2 have the same values
                                   SmartAssert.NotChanged( account2 ),
                                   // Before Act: Register to PropertyChangedEvent
                                   // After Act: Assert no PropertyChanged is raised
                                   SmartAssert.NotRaised_PropertyChanged(),
                                   // Before Act: Register to PropertyChangedEvent
                                   // After Act: Assert no PropertyChanged is raised for account2
                                   SmartAssert.NotRaised_PropertyChanged( account2 ),
                                   // Before Act: Keep track of all public properties of _Account.Transactions
                                   // After Act: Assert not property of _Account.Transactions changed (especially Count)
                                   SmartAssert.NotChanged( _Account.Transactions ),
                                   // Before Act: Keep track of all public properties of account2.Transactions
                                   // After Act: Assert not property of account2.Transactions changed (especially Count)
                                   SmartAssert.NotChanged( account2.Transactions )
                                 );

            Assert.IsFalse( success );
        }

        #endregion
    }
}