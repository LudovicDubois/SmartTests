# `ValidValue` Criteria

You use `ValidValue` criteria when you want to explicitly specify that value can be either valid or invalid.

Thus, it contains only two criterions:

* `IsValid`
* `IsInvalid` (an error)

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

        RunTest( ValidValue.Valid,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.ChangedTo() );
    }
}
```
