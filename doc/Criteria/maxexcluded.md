# MaxExcluded Criteria

You use `MaxExcluded` criteria when you want to explicitly specify that a value has a maximum, that is invalid.

Thus, it contains three criterions:

* `IsBelowMax`
* `IsMax` (an error)
* `IsAboveMax` (an error)

Use [MaxIncluded](maxincluded.md) if you want the maximum be included.

Use [MinIncluded](minincluded.md) or [MinExcluded](minexcluded.md) if you want a minimum instead of a maximum.

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

        RunTest( MaxExcluded.IsBelowMax,
                 Assign( () => mc.MyProperty, -10 ) );

        Assert.AreEqual( -10, mc.MyProperty );
    }
}
```
