using CoolApi.Database.Models;
using CoolApi.Database.Options;
using CoolApi.Database.Repositories.Results;
using System;
using System.Linq;

namespace CoolApi.Database.Repositories
{
    public class UserRepository : BaseRepository<User, ReadPortionUserOptions, ReadUserOptions, CreateUserOptions, UpdateUserOptions, DeleteUserOptions>
    {
        public UserRepository(CoolContext context) : base(context)
        {
        }

        public override User Read(ReadUserOptions options)
        {
            var user = GetUser(options.EntityId);

            return user;
        }

        public override PortionResult<User> ReadPortion(ReadPortionUserOptions options)
        {
            var totalCount = DbContext.Users.Count();
            var users = DbContext.Users
                .Where(u => u.Login.Contains(options.LoginSubstring))
                .OrderBy(u => u.Login)
                .Skip(options.Offset)
                .Take(options.Portion)
                .ToList();

            var result = new PortionResult<User>
            {
                TotalCount = totalCount,
                DataCollection = users
            };
            return result;
        }

        public override void Create(CreateUserOptions options)
        {
            DbContext.Users.Add(options.Entity);
        }

        public override void Update(UpdateUserOptions options)
        {
            DbContext.Users.Update(options.Entity);
        }

        public override void Delete(DeleteUserOptions options)
        {
            var user = GetUser(options.Id);
            DbContext.Users.Remove(user);
        }

        private User GetUser(Guid id)
        {
            var user = DbContext.Users.Find(id);

            return user;
        }
    }
}
