using System.Collections.Generic;

namespace CoolApi.Database.Identity
{
    public class UsersCollection<TUser> where TUser : class
    {
        public int TotalCount { get; init; }

        public IEnumerable<TUser> Collection { get; init; }
    }
}
