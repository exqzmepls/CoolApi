using CoolApi.Database.Repositories.Results;
using System;
using System.Linq.Expressions;

namespace CoolApi.Database.Repositories
{
    public interface IRepository<TEntity, TReadPortionOptions, TReadOptions, TCreateOptions, TUpdateOptions, TDeleteOptions> where TEntity : class
    {
        public TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> whereExpression);
        
        public PortionResult<TEntity> ReadPortion(TReadPortionOptions options);

        public TEntity Read(TReadOptions options);

        public void Create(TCreateOptions options);

        public void Update(TUpdateOptions options);

        public void Delete(TDeleteOptions options);

        public void Save();
    }
}
