# `ValidString` Criteria

You use `validString` when you want to test a string that must have no specific format, but cannot be `null` nor empty.

It contains three criterions:

* `IsNull` (an error)
* `IsEmpty` (an error)
* `HasContent`

Use [`FormattedString`](FormattedString.md) if your string has a specific format (such as a phone number, for example).

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

        RunTest( ValidString.IsValid,
                 Assign( () => mc.Name, "Ludovic Dubois" ),
                 SmartAssert.ChangedTo() );
    }
}
```
