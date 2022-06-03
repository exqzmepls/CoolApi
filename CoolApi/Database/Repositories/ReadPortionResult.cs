using System.Collections.Generic;

namespace CoolApi.Database.Repositories
{
    public class ReadPortionResult<TEntity> where TEntity : class
    {
        public int TotalCount { get; init; }

        public IEnumerable<TEntity> DataCollection { get; init; }
    }
}
