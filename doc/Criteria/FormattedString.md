# `FormattedString` Criteria

You use `FormattedString` when you want to test a string that must have a specific format (such as a phone number, for example).

It contains four criterions:

* `IsNull` (an error)
* `IsEmpty` (an error)
* `IsInvalid` (an error)
* `IsValid`

Use [`ValidString`](ValidString.md) if your string has no specific format.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_Set_IsValid()
    {
        var mc = new MyClass();

        RunTest( FormattedString.IsValid,
                 Assign( () => mc.PhoneNumber, "555-123-4567" ),
                 SmartAssert.ChangedTo() );
    }
}
```
