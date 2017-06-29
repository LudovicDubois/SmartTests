# `MinIncluded` Criteria

You use `MinExcluded` criteria when you want to explicitly specify that a value has a minimum, that is valid.

Thus, it contains three criterions:

* `IsBelowMin` (an error)
* `IsMin`
* `IsAboveMin`

Use [MinExcluded](MinExcluded.md) if you want the minimum be excluded.

Use [MaxIncluded](MaxIncluded.md) or [MaxExcluded](MaxExcluded.md) if you want a maximum instead of a minimum.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_Set_IsAboveMin()
    {
        var mc = new MyClass();

        RunTest( MinIncluded.IsAboveMin,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.ChangedTo() );
    }
}
```
