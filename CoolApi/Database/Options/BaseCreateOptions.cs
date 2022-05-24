namespace CoolApi.Database.Options
{
    public abstract class BaseCreateOptions<TEntity> where TEntity : class
    {
        public TEntity Entity { get; init; }
    }
}
