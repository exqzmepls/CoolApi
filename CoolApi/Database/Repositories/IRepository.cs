using System;
using System.Linq.Expressions;

namespace CoolApi.Database.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public ReadPortionResult<TEntity> ReadPortion(int offset, int size, Expression<Func<TEntity, bool>> filter, Guid userId);

        public TEntity Read(Guid entityId, Guid userId);

        public Guid Create(TEntity entity, Guid userId);

        public void Update(TEntity entity, Guid userId);

        public void Delete(Guid entityId, Guid userId);

        public void Hide(Guid entityId, Guid userId);
    }
}
