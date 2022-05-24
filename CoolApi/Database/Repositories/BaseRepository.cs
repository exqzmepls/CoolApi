using CoolApi.Database.Options;
using CoolApi.Database.Repositories.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CoolApi.Database.Repositories
{
    public abstract class BaseRepository<TEntity, TReadPortionOptions, TReadOptions, TCreateOptions, TUpdateOptions, TDeleteOptions> :
        IRepository<TEntity, TReadPortionOptions, TReadOptions, TCreateOptions, TUpdateOptions, TDeleteOptions>
        where TEntity : class
        where TReadPortionOptions : BaseReadPortionOptions
        where TReadOptions : BaseReadOptions
        where TCreateOptions : BaseCreateOptions<TEntity>
        where TUpdateOptions : BaseUpdateOptions<TEntity>
        where TDeleteOptions : BaseDeleteOptions
    {
        protected readonly CoolContext DbContext;

        protected BaseRepository(CoolContext context)
        {
            DbContext = context;
        }

        public abstract void Create(TCreateOptions options);

        public abstract void Delete(TDeleteOptions options);

        public abstract TEntity Read(TReadOptions options);

        public abstract PortionResult<TEntity> ReadPortion(TReadPortionOptions options);

        public abstract void Update(TUpdateOptions options);

        /// <summary>
        /// Saves all changes to the DB.
        /// </summary>
        /// <exception cref="DbUpdateException"></exception>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        public void Save()
        {
            DbContext.SaveChanges();
        }

        public TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var single = DbContext.Set<TEntity>().SingleOrDefault(predicate);

            return single;
        }

    }
}
