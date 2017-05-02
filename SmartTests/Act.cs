namespace SmartTests
{
    public abstract class Act
    {
        public abstract void Invoke();
    }


    public abstract class Act<T>
    {
        public abstract T Invoke();
    }
}