using System;
using System.Linq;

using Example01;

using NUnit.Framework;

using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace Example01Tests
{
    [TestFixture]
    public class CustomerTests
    {
        private Customer _Customer;


        [SetUp]
        public void Setup()
        {
            _Customer = new Customer( "Ludovic Dubois", "Montreal" );
        }


        //
        // Address PROPERTY TESTS
        //


        [Test]
        public void SetAddress_HappyPath()
        {
            RunTest( ValidString.HasContent,
                     // Act
                     // To Test Assignment, use Assign
                     Assign( () => _Customer.Address, "Quebec" ),

                     // Assert (and implicit Assume)
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert PropertyChanged raised when assignment is run, with "Address" as PropertyName
                     SmartAssert.Raised_PropertyChanged(),
                     // Before Act: Assume _Customer.Address != "Quebec" before assignment, keep track of all other public properties
                     // AfterAct: _Customer.Address == "Quebec", all other public properties did not change
                     SmartAssert.NotChangedExceptAct()
                   );
        }


        [Test]
        public void SetAddress_Empty()
        {
            RunTest( ValidString.IsEmpty,
                     // Act
                     // To Test Assignment, use Assign
                     Assign( () => _Customer.Address, "" ),

                     // Assert (and implicit Assume)
                     // After Act: Assert ArgumentOutOfRangeException is thrown with ParamName == "value" and Message == "Address Cannot be empty not null!" (omitting Parameter Name: value)
                     SmartAssert.Throw<ArgumentOutOfRangeException>( "value", "Address cannot be empty nor null!" ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert not PropertyChanged is raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: keep track of all properties and fields
                     // After Act: Assert no property nor field changed => the _Customer is not changed at all
                     SmartAssert.NotChanged( NotChangedKind.All )
                   );
        }


        [Test]
        public void SetAddress_Null()
        {
            RunTest( ValidString.IsNull,
                     // Act
                     // To Test Assignment, use Assign
                     Assign( () => _Customer.Address, null ),

                     // Assert (and implicit Assume)
                     // After Act: Assert ArgumentOutOfRangeException is thrown with ParamName == "value" and Message == "Address Cannot be empty not null!" (omitting Parameter Name: value)
                     SmartAssert.Throw<ArgumentNullException>( "value", "Address cannot be empty nor null!" ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert not PropertyChanged is raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: keep track of all properties and fields
                     // After Act: Assert no property nor field changed => the _Customer is not changed at all
                     SmartAssert.NotChanged( NotChangedKind.All )
                   );
        }


        //
        // CreateAccount METHOD TESTS
        //


        [Test]
        public void CreateAccount()
        {
            var previous = _Customer.CreateAccount();

            var account = RunTest( // We always can call CreateAccount
                                  AnyValue.IsValid,
                                  // Act
                                  () => _Customer.CreateAccount(),

                                  // Assert (and implicit Assume)
                                  // Before act: Register to PropertyChangedEvent
                                  // After Act: Assert not PropertyChanged is raised
                                  SmartAssert.NotRaised_PropertyChanged(),
                                  // Before Act: Keep track of all properties and fields
                                  // After Act: Assert no property nor field changed => the _Customer is not changed at all
                                  SmartAssert.NotChanged( NotChangedKind.All ),
                                  // Before Act: Compute the expression _Customer.Account.Count + 1 and save its result
                                  // After Act: Assert current _Customer.Accounts.Count is the saved value (thus, that Count is one more than before)
                                  SmartAssert.Change( () => _Customer.Accounts.Count + 1 )
                                 );

            // Assert
            Assert.AreSame( account, _Customer.Accounts.Last() );
            Assert.AreEqual( previous.Id + 1, account.Id );
            Assert.AreSame( _Customer, account.Customer );
            Assert.AreEqual( 0, account.Balance );
            Assert.IsEmpty( account.Transactions );
        }


        //
        // CloseAccount METHOD TEST
        //


        [Test]
        public void CloseAccount_HappyPath()
        {
            var account = _Customer.CreateAccount();

            var result = RunTest( CollectionItem.IsInCollection,
                                  // Act
                                  () => _Customer.CloseAccount( account ),

                                  // Assert (and implicit Assume)
                                  // Before Act: Keep track of all properties and fields
                                  // After Act: Assert no property nor field changed => the _Customer is not changed at all
                                  SmartAssert.NotChanged( NotChangedKind.All ),
                                  // Before Act: Register to PropertyChangedEvent
                                  // After Act: Assert not PropertyChanged is raised
                                  SmartAssert.NotRaised_PropertyChanged(),
                                  // Before Act: Compute the expression _Customer.Account.Count - 1 and save its result
                                  // After Act: Assert current _Customer.Accounts.Count is the saved value (thus, that Count is one less than before)
                                  SmartAssert.Change( () => _Customer.Accounts.Count - 1 )
                                );

            // Assert
            Assert.IsTrue( result );
        }


        [Test]
        public void CloseAccount_NotOurAccount()
        {
            // Arrange
            var customer2 = new Customer( "You", "World" );
            var account = customer2.CreateAccount();

            var result = RunTest( CollectionItem.IsNotInCollection,
                                  // Act
                                  () => _Customer.CloseAccount( account ),

                                  // Assert (and implicit Assume)
                                  // Before Act: Keep track of all properties and fields
                                  // After Act: Assert no property nor field changed => the _Customer is not changed at all
                                  SmartAssert.NotChanged( NotChangedKind.All ),
                                  // Before Act: Register to PropertyChangedEvent
                                  // Assert Act: Ensure not PropertyChanged is raised
                                  SmartAssert.NotRaised_PropertyChanged(),
                                  // Before Act: Keep track of all public properties of _Customer.Accounts
                                  // After Act: Assert current _Customer.Accounts public properties are the saved value (thus, that _Customer.Accounts did not changed at all)
                                  SmartAssert.NotChanged( _Customer.Accounts )
                                );

            // Assert
            Assert.IsFalse( result );
        }


        [Test]
        public void CloseAccount_NoAccount()
        {
            RunTest( CollectionItem.IsNull,
                     // Act
                     () => _Customer.CloseAccount( null ),

                     // Assert (and implicit Assume)
                     // After: Assert ArgumentNullException is thrown for the right parameter (optional) with the right error message (optional)
                     SmartAssert.Throw<ArgumentNullException>( "account", "account belonging to this customer is required" ),
                     // Before Act: Register to PropertyChangedEvent
                     // After Act: Assert not PropertyChanged is raised
                     SmartAssert.NotRaised_PropertyChanged(),
                     // Before Act: Keep track of all properties and fields
                     // After Act: Assert no property nor field changed => the _Customer is not changed at all
                     SmartAssert.NotChanged( NotChangedKind.All ),
                     // Before Act: Keep track of all public properties of _Customer.Accounts
                     // After Act: Assert current _Customer.Accounts public properties are the saved value (thus, that _Customer.Accounts did not changed at all)
                     SmartAssert.NotChanged( _Customer.Accounts )
                   );
        }
    }
}