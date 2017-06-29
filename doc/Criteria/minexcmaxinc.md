# `MinExcMaxInc` Criteria

You use `MinExcMaxInc` criteria when you want to explicitly specify that a value is in a range, with valid maximum but invalid minimum.

Thus, it contains five criterions:

* `IsBelowMin` (an error)
* `IsMin` (an error)
* `IsBetweenMinMax`
* `IsMax`
* `IsAboveMax` (an error)

Use [MinIncMaxInc](MinIncMaxInc.md), [MinIncMaxExc](MinIncMaxExc.md) or [MinExcMaxExc](MinExcMaxExc.md) to include/exclude min and/or max.

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

        RunTest( MinExcMaxInc.IsBetweenMinMax,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartAssert.ChangedTo() );
    }
}
```
