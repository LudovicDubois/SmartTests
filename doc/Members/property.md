# Testing a Property

A property has two methods associated: the getter and/or the setter.

## Testing a Property Getter

To test a Property Getter, you only have to call it in your lambda expression, just like for constructors, methods and indexers.

Example for a Property Getter test:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest()
    {
        var mc = new MyClass( 10 );

        var result = RunTest( AnyValue.Valid,
                              () => mc.Value );

        Assert.AreEqual( 10, result );
    }
}
```

## Testing a Property Setter

To test a Property Setter, you have to use a special syntax, as C# does not accept assignment in `Expression`s.

Thus, you can specify your assignment using a special construct: a static method `Assign` exist, that takes an expression of the lambda as first argument and the value to set as the second argument.

Example for a Property Setter test:

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyPropertyTest()
    {
        var mc = new MyClass( 10 );

        RunTest( MinIncluded.IsAboveMin,
                 Assign( () => mc.Value, 2 ) );

        Assert.AreEqual( 2, mc.Value );
    }
}
```

> Warning: this implies that the getter has to exist to test the setter!
