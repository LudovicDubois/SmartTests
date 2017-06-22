# Testing Constructors and Methods

To test a Constructor or a Method, you only have to call it in your lambda expression.

## Testing a Constructor

Example for a constructor test:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void ConstructorTest()
    {
        var result = RunTest( MinIncluded.IsAboveMin,
                              () => new MyClass( 10 ) );

        Assert.AreEqual( 10, result.Value );
    }
}
```

## Testing a Method

### Testing a method returning a value

Example for a method test:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MathTest
{
    [Test]
    public void Sqrt_ValueGreaterThanMin()
    {
        var result = RunTest( MinIncluded.IsAboveMin,
                              () => Math.Sqrt(2) );

        Assert.AreEqual( 2, result );
    }
}
```

### Testing a method returning no value

If your method does not return anything (return type is `void`), you test it the same way, but the involved `RunTest` overload returns nothing too (`void`).

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void ReturnVoidTest()
    {
        var mc = new MyClass( 10 );

        RunTest( MinIncluded.IsAboveMin,
                 () => mc.SetValue( 2 ) );

        Assert.AreEqual( 2, mc.Value );
    }
}
```
