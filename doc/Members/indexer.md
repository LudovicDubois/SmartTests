# Testing an Indexer

An indexer has two methods associated: the getter and/or the setter.

## Testing an Indexer Getter

To test an Indexer Getter, you only have to call it in your lambda expression, just like for constructors, methods and properties.

Example for an Indexer Getter test:

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

        var result = RunTest( AnyValue.IsValid,
                              () => mc.Values[0] );

        Assert.AreEqual( 10, result );
    }
}
```

## Testing an Indexer Setter

To test an Indexer Setter, you have to use a special syntax, as C# does not accept assignment in `Expression`s.

Thus, just like for [Properties](property.md), you can specify your assignment using a special construct: a static method `Assign` exist, that takes an expression of the lambda as first argument and the value to set as the second argument.

The other difference with Properties is that we actually have to provide several criterias: one for the value to set and one for each indexer parameter.

Your first argument to RunTest can express both using [Cases](../Cases/readme.md).

A `Case` static method exists to enables you to specify the parameter name to which it applies, if any, and the corresponding criteria.

You can then combine Cases with the `&` operator.

Example for an Indexer Setter test:

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

        RunTest( Case( "index", MinIncluded.IsMin) &
                 Case( MinIncluded.IsAboveMin ),
                 Assign( () => mc.Values[0], 2 ) );

        Assert.AreEqual( 2, mc.Values[0] );
    }
}
```

> Warning: this implies that the getter has to exist to test the setter!
