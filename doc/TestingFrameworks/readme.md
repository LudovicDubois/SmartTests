# Adding Test Frameworks

In fact, *Smart Test*, for the runtime part, is independent of:

* any programming language (as far as it is a .NET framework)
* any testing framework.

However, the *Smart Test Analyzer* is not. It has to retrieve the tests in your code.

Right now, *Smart Tests Analyzer* Supports two Testing Frameworks:

1. NUnit.
1. MS Tests.

It is very easy to extend the supported Testing Frameworks, but not so easy to use the new ones.

## Supporting a new Testing Framework

### `TestingFrameworkStrategy`

`TestingFrameworkStrategy` is the base class of all Testing Framework support.

```C#
public abstract class TestingFrameworkStrategy
{
    public abstract bool IsValid { get; }
    public abstract bool IsTestClass( ITypeSymbol type );
    public abstract bool IsTestMethod( IMethodSymbol method );
}
```

You only have to define a subclass of this class with a constructor with one parameter: `Compilation`.

Lots of Testing Frameworks uses attributes to specify what classes are Test Classes and what methods are Test Methods. Thus, we have a subclass to simplify these cases:

### `AttributedTestingFramework`

`AttributedTestingFramework` is the base class of all Testing Framework support that uses attribute to spot Test Classes and Test Methods.

It has only one constructor: `protected AttributedTestingFramework( Compilation compilation, string testClassAttribute, string testMethodAttribute )`

Thus, your subclass only have to provide a constructor which takes a `Compilation` parameter and call this base constructor with the full names of the attribute for the test class and for the test method.

That's all.

### `NUnitTestingFramework`

For example, here is the code of the `NUnitTestingFramework`:

```C#
public class NUnitStrategy: AttributedTestingFramework
{
    public NUnitStrategy( Compilation compilation )
        : base( compilation, "NUnit.Framework.TestFixtureAttribute", "NUnit.Framework.TestAttribute" )
    { }
}
```

## Using Added Testing Frameworks

The difficult part is to use new Testing Frameworks.

As the *Smart Test Analyzer* is a Portable DLL, it cannot load files dynamically. Thus, you have to change the source code to support new Testing Frameworks! :(

In `TestingFrameworkStrategy.cs` file, locate the `TestingFrameworks` constructor.

It looks like the following:

```C#
public TestingFrameworks( Compilation compilation )
{
    var nunit = new NUnitStrategy( compilation );
    if( nunit.IsValid )
        _TestingFrameworks.Add( nunit );

    var mstest = new MSTestStrategy( compilation );
    if( mstest.IsValid )
        _TestingFrameworks.Add( mstest );
}
```

Add your own the same way.

That's all!
