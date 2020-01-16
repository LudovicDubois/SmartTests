using System;

using Example01;

using NUnit.Framework;

using SmartTests.Assertions;
using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace Example01Tests
{
    [TestFixture]
    public class CustomerConstructorTests
    {
        [Test]
        public void HappyPath()
        {
            // Arrange Part
            var previous = new Customer( "Customer", "Address" );

            // Act part: shows test case with combination of Case (per parameter)
            // The first parameter is the name of the parameter, the second is the tested criteria
            // Once you have your cases combinations, missing cases will appear automatically
            // (that will disappear when the corresponding tests are written)
            var customer = RunTest( Case( "name", ValidString.HasContent ) &
                                    Case( "address", ValidString.HasContent ),
                                    () => new Customer( "Ludovic Dubois", "Montreal" ) );

            // Assert Part
            Assert.AreEqual( previous.Id + 1, customer.Id );
            Assert.AreEqual( "Ludovic Dubois", customer.Name );
            Assert.AreEqual( "Montreal", customer.Address );
            Assert.IsEmpty( customer.Accounts );
        }


        [Test]
        public void EmptyName()
        {
            // Act part: implement another case.
            // This case is an error, should be treated specifically (one error at a time)
            RunTest( Case( "name", ValidString.IsEmpty ) &
                     Case( "address", ValidString.HasContent ),
                     () => new Customer( "", "Montreal" ),
                     // Assert Part
                     // Ensure the ArgumentOutOfRangeException is thrown when the Customer is created
                     // Better than Assert.Catch, as you can still have Smart Assertions after catching the failure
                     SmartAssert.Throw<ArgumentOutOfRangeException>( "name" ) );
        }


        [Test]
        public void NullName()
        {
            // Act part: implement another case.
            // This case is an error, should be treated specifically (one error at a time)
            RunTest( Case( "name", ValidString.IsNull ) &
                     Case( "address", ValidString.HasContent ),
                     () => new Customer( null, "Montreal" ),
                     // Assert Part
                     // Ensure the ArgumentNullException is thrown when the Customer is created
                     // Better than Assert.Catch, as you can still have Smart Assertions after catching the failure
                     SmartAssert.Throw<ArgumentNullException>( "name" ) );
        }


        [Test]
        public void EmptyAddress()
        {
            // Act part: implement another case.
            // This case is an error, should be treated specifically (one error at a time)
            RunTest( Case( "name", ValidString.HasContent ) &
                     Case( "address", ValidString.IsEmpty ),
                     () => new Customer( "Ludovic Dubois", "" ),
                     // Assert Part
                     // Ensure the ArgumentOutOfRangeException is thrown when the Customer is created
                     // Better than Assert.Catch, as you can still have Smart Assertions after catching the failure
                     SmartAssert.Throw<ArgumentOutOfRangeException>( "address" ) );
        }


        [Test]
        public void NullAddress()
        {
            // Act part: implement another case.
            // This case is an error, should be treated specifically (one error at a time)
            RunTest( Case( "name", ValidString.HasContent ) &
                     Case( "address", ValidString.IsNull ),
                     () => new Customer( "Ludovic Dubois", null ),
                     // Assert Part
                     // Ensure the ArgumentNullException is thrown when the Customer is created
                     // Better than Assert.Catch, as you can still have Smart Assertions after catching the failure
                     SmartAssert.Throw<ArgumentNullException>( "address" ) );
        }
    }
}