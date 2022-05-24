namespace CoolApi.Database.Options
{
    public abstract class BaseUpdateOptions<T> where T : class
    {
        public T Entity { get; init; }
    }
}
