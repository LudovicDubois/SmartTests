# MinIncMaxInc Criteria

You use `MinIncMaxInc` criteria when you want to explicitly specify that a value is in a range, with valid limits.

Thus, it contains five criterions:

* `IsBelowMin` (an error)
* `IsMin`
* `IsBetweenMinMax`
* `IsMax`
* `IsAboveMax` (an error)

Use [MinIncMaxExc](minincmaxexc.md), [MinExcMaxInc](minexmaxinc.md) or [MinExcMaxExc](minexmaxexc.md) to include/exclude min and/or max.

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest_IsBelowMin()
    {
        var mc = new MyClass();

        RunTest( MinIncMaxInc.IsBetweenMinMax,
                 Assign( () => mc.MyProperty, 10 ) );

        Assert.AreEqual( 10, mc.MyProperty );
    }
}
```
