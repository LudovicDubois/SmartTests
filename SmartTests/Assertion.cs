using System;



namespace SmartTests
{
    public abstract class Assertion
    {
        public abstract void BeforeAct( ActBase act );
        public abstract void AfterAct( ActBase act );
    }
}