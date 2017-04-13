using System;

using NUnit.Framework;

using SmartTests.Criterias;

using static SmartTests.SmartTest;



namespace TestingProject
{
    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestMethod()
        {
            var result = RunTest( Case( ValidValue.Valid ), () => Math.Sqrt( 4 ) );

            Assert.That( result, Is.EqualTo( 2 ) );
        }
    }
}