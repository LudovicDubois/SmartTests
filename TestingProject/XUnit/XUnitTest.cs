using System;

using SmartTests;
using SmartTests.Criterias;

using Xunit;



namespace TestingProject.XUnit
{
    public class XUnitTest
    {
        class MyClass
        {
            public double Inverse(double d) => 1 / d;
            public void SetPositive(int i) { Console.WriteLine(i); }
        }


        [Fact]
        public void FactBelowMin()
        {
            var mc = new MyClass();

            SmartTest.RunTest(MinExcluded.IsBelowMin,
                     () => mc.SetPositive(-1));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TheoryInverse(int data)
        {
            var mc = new MyClass();

            SmartTest.RunTest(ValidValue.IsValid,
                     () => mc.Inverse(data));
        }
    }
}