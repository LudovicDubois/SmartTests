using System;



namespace SmartTests
{
    public abstract class Assertion
    {
        public abstract void BeforeAct();
        public abstract void AfterAct( Exception e );
    }
}