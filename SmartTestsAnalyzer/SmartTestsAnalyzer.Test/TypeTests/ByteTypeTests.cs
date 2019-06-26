﻿using NUnit.Framework;



namespace SmartTestsAnalyzer.Test.TypeTests
{
    [TestFixture]
    class ByteTypeTests: TypeBaseTests
    {
        public ByteTypeTests()
            : base("byte", "ByteRange")
        { }
    }
}