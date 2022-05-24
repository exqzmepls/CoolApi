using System.Collections.Generic;

namespace CoolApi.Database.Repositories.Results
{
    public class PortionResult<TEntity> where TEntity : class
    {
        public int TotalCount { get; init; }

        public IEnumerable<TEntity> DataCollection { get; init; }
    }
}
