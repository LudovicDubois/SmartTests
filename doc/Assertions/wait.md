# Wait Smart Assertions <!-- omit in toc -->

*Wait Smart Assertions* are assertions that wait for a handle set in the *Act* part of your test.

1. Before *Act*

   Creates an implicit `AutoResetEvent` in the Act Context, if none is specified.

2. After *Act*
   Waits for the handle (specified or implicit) for the maximum specified amount of time. If this timeout is reached, the assertion fails.

We have four *Wait* assertions:

- [`Wait(WaitHandle,double)`](#waitwaithandledouble)
- [`Wait(WaitHandle,TimeSpan)`](#waitwaithandletimespan)
- [`WaitContextHandle(double)`](#waitcontexthandledouble)
- [`WaitContextHandle(TimeSpan)`](#waitcontexthandletimespan)

## `Wait(WaitHandle,double)`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();
        var handle = new ManualResetEvent( false );

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod( () => handle.set() ),
                 SmartAssert.Wait( handle, 1000 ) );
    }
}
```

In this example, the Smart Assertion ensures that the call to `MyMethod` calls the provided argument within 1000ms.

## `Wait(WaitHandle,TimeSpan)`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
        var mc = new MyClass();
        var handle = new ManualResetEvent( false );

        RunTest( ValidValue.IsValid,
                 () => mc.MyMethod( () => handle.set() ),
                 SmartAssert.Wait( handle, TimeSpan.FromSeconds( 1 ) ) );

        Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
    }
}
```

In this example, the Smart Assertion ensures that the call to `MyMethod` calls the provided argument within 1s.

## `WaitContextHandle(double)`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
       var mc = new MyClass(300);

        RunTest( AnyValue.IsValid,
                ctx => mc.Method( ctx.SetHandle ),
                SmartAssert.Within( 100 ),
                SmartAssert.WaitContextHandle( 1000 ) );

        Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
    }
}
```

In this example, the Smart Assertion ensures that the call to `MyMethod` calls the provided argument within 1000ms.

## `WaitContextHandle(TimeSpan)`

```C#
using NUnit.Framework;
using static SmartTests.SmartTest;

[TestFixture]
public class MyClassTest
{
    [Test]
    public void MyMethodTest()
    {
       var mc = new MyClass(300);

        RunTest( AnyValue.IsValid,
                ctx => mc.Method( ctx.SetHandle ),
                SmartAssert.Within( 100 ),
                SmartAssert.WaitContextHandle( TimeSpan.FromSeconds( 1 ) ) );

        Assert.IsTrue( mc.Done ); // The method runs the parallel code (ctx.SetHandle) to its end.
        Assert.IsNull( mc.Exception ); // There was no exception in the parallel code thread.
    }
}
```

In this example, the Smart Assertion ensures that the call to `MyMethod` calls the provided argument within 1s.
