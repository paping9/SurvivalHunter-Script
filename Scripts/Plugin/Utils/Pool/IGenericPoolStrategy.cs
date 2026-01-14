namespace Utils.Pool
{
    public interface IGenericPoolStrategy<T>
    {
        T Create();
        void OnGet(T item);
        void OnRelease(T item);
        void OnDestroy(T item);
        void OnReuse(T item);
    }
    
    public interface IPoolClearable
    {
        void Clear();
    }

}