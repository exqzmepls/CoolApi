using System;

namespace CoolApi.Database.Options
{
    public abstract class BaseReadOptions
    {
        public Guid EntityId { get; init; }
    }
}
