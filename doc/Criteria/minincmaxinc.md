# `MinIncMaxInc` Criteria

You use `MinIncMaxInc` criteria when you want to explicitly specify that a value is in a range, with valid limits.

Thus, it contains five criterions:

* `IsBelowMin` (an error)
* `IsMin`
* `IsBetweenMinMax`
* `IsMax`
* `IsAboveMax` (an error)

Use [MinIncMaxExc](MinIncMaxExc.md), [MinExcMaxInc](MinExcMaxInc.md) or [MinExcMaxExc](MinExcMaxExc.md) to include/exclude min and/or max.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_Set_IsBetweenMinMax()
    {
        var mc = new MyClass();

        RunTest( MinIncMaxInc.IsBetweenMinMax,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.ChangedTo() );
    }
}
```
