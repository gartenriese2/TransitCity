namespace Utility.Threading
{
    public interface IAtomic<T>
    {
        void Replace(T other);
    }
}
