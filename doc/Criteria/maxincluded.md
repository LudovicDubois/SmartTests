# `MaxIncluded` Criteria

You use `MaxIncluded` criteria when you want to explicitly specify that a value has a maximum, that is valid.

Thus, it contains three criterions:

* `IsBelowMax`
* `IsMax`
* `IsAboveMax` (an error)

Use [MaxExcluded](MaxExcluded.md) if you want the maximum be excluded.

Use [MinIncluded](MinIncluded.md) or [MinExcluded](MinExcluded.md) if you want a minimum instead of a maximum.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_Set_IsBelowMax()
    {
        var mc = new MyClass();

        RunTest( MaxIncluded.IsBelowMax,
                 Assign( () => mc.MyProperty, -10 ),
                 SmartAssert.ChangedTo() );
    }
}
```
