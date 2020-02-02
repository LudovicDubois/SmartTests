# `Optional` Criteria

You use `Optional` criteria when you want to explicitly specify that value can be optional, i.e. absent or present. Typically, use it for an optional parameter (a parameter with a default value).

Thus, it contains only two criterions:

* `Absent`
* `Present`

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethod_Absent()
    {
        var mc = new MyClass();

        RunTest( Optional.Absent,
                 () => mc.MyMethod() );
    }


    [Test]
    public void MyMethod_Present()
    {
        var mc = new MyClass();

        RunTest( Optional.Present,
                 () => mc.MyMethod(10) );
    }
}
```
