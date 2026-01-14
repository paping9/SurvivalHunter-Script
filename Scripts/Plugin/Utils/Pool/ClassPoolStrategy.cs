namespace Utils.Pool
{
    public class ClassPoolStrategy<T> : IGenericPoolStrategy<T> where T : class, new()
    {
        public T Create() => new T();
        public void OnGet(T item) { }
        public void OnRelease(T item) { }
        public void OnDestroy(T item) { }
        public void OnReuse(T item) { }
    }
}