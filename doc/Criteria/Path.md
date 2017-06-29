# `Path` Criteria

You use `Path` criteria when you want to test a path string to an existing file or directory.

It contains four criterions:

* `IsNull` (an error)
* `IsInvalid` (an error)
* `IsNonExistent` (an error)
* `IsValid`

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void LoadTest_IsBelowMin()
    {
        var mc = new MyClass();

        RunTest( Path.IsValid,
                 () => mc.Load( @"c:\MyData.csv" ),
                 SmartAssert.Change( () => mc.Count + 10 ) );
    }
}
```
