# Smart Tests

## Context

We all know how unit testing is important for non-regression of our softwares.

We all know that TDD (Test Driven Development) enables us to have better code and better management of the development process.

Fortunately, it is very easy to learn Testing Frameworks and to write unit tests technically.

BUT, it is very hard to think the good way to write unit test:

1. How do I know how many tests to write?
1. More, how do I know which cases we have to test?
1. According to our Act code, it is very easy to assert that:
   1. A property changed, but how do we know if any other unexpected side effect occurred?
   1. A property changed, but how do we know if our test is still correct if anyone changed our Setup?
   For example, by giving the value you want to set as the initial value of your object?
1. How can we show the expected effect of the Act code as we generally use hard coded values (because it is easier to code)?
1. Finally, how can we do high level testing without multiple lines of code?

## What is *Smart Tests*

*Smart Tests* is a library and a Visual Studio Analyzer to respond to all these questions... and more!

1. The Act line of your test is instantly identifiable.
1. You express your Act logical intent and the Analyzer will display all missing tests.
1. You can use smarter assertions than the ones provided by usual Testing Frameworks as *Smart Tests* knows what is your Act and when it is run.
   1. You ensure that a PropertyChanged event is raised with only one line of code.
   1. You ensure that any event is raised with only one line of code.
   1. You ensure that an object didn't changed at all with only one line of code.
   1. You ensure that your property set is effective by checking its value after is not equal to its value before with only one line of code.
   1. You show each effect of your Act code relatively its previous value with only one line of code.

For now, it supports C# with NUnit, MSTest and Xunit.

However, it will very easy to add other Testing Frameworks (you only write a very simple sub-class).  
You can also extend smart assertions in a very easily way.

## Documentation

This documentation explains:

1. [What's new](doc/WhatsNew.md)
1. [How to write your first Smart Test](doc/WriteSmartTest.md).
1. [How to test Class Members](doc/Members/readme.md)
1. [Criteria](doc/Criteria/readme.md)
1. [Cases](doc/Cases/readme.md)
1. [Smart Assertions](doc/Assertions/readme.md)

With more advanced usages:

1. [Adding Criteria](doc/Criteria/adding.md)
1. [Extending Smart Assertions](doc/Assertions/extend.md)
1. [Supporting Other Testing Frameworks](doc/TestingFrameworks/readme.md)