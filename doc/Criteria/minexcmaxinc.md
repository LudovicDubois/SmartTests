# MinExcMaxInc Criteria

You use `MinExcMaxInc` criteria when you want to explicitly specify that a value is in a range, with valid maximum but invalid minimum.

Thus, it contains five criterions:

* `IsBelowMin` (an error)
* `IsMin` (an error)
* `IsBetweenMinMax`
* `IsMax`
* `IsAboveMax` (an error)

Use [MinIncMaxInc](minincmaxinc.md), [MinIncMaxExc](minincmaxexc.md) or [MinExcMaxExc](minexmaxexc.md) to include/exclude min and/or max.

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

        RunTest( MinExcMaxInc.IsBetweenMinMax,
                 Assign( () => mc.MyProperty, 10 ) );

        Assert.AreEqual( 10, mc.MyProperty );
    }
}
```
