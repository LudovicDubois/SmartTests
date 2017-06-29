# `AnyValue` Criteria

You use `AnyValue` criteria when you want to explicitly specify that any value is valid.

Thus, it contains only one criterion:

* `Valid`

## Example

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest()
    {
        var mc = new MyClass();

        RunTest( AnyValue.IsValid,
                 Assign( () => mc.MyProperty, 10 ),
                 SmartTest.ChangedTo() );
    }
}
```