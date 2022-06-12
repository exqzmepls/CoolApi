using CoolApi.Database.Hashers;
using CoolApi.Database.Models;
using System;
using System.Linq;

namespace CoolApi.Database.Identity
{
    public class UserManager
    {
        private readonly CoolContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserManager(CoolContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public bool ChangeLogin(Guid userId, string login, string currentPassword)
        {
            var user = FindById(userId);
            if (user == null)
                return false;

            var isCurrentPasswordVerified = VerifyPassword(user, currentPassword);
            if (!isCurrentPasswordVerified)
                return false;

            user.Login = login;
            _context.Users.Update(user);
            return true;
        }

        public bool ChangePassword(Guid userId, string password, string currentPassword)
        {
            var user = FindById(userId);
            if (user == null)
                return false;

            var isCurrentPasswordVerified = VerifyPassword(user, currentPassword);
            if (!isCurrentPasswordVerified)
                return false;

            var passwordHash = _passwordHasher.GetPasswordHash(password);
            user.PasswordHash = passwordHash;
            _context.Users.Update(user);
            return true;
        }

        public Guid Create(string login, string password)
        {
            var passwordHash = _passwordHasher.GetPasswordHash(password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = login,
                PasswordHash = passwordHash
            };
            var addedUser = _context.Users.Add(user);
            return addedUser.Entity.Id;
        }

        public User FindById(Guid id)
        {
            var user = _context.Users.Find(id);
            return user;
        }

        public UsersCollection<User> FindByLogin(int offset, int count, string loginSubstring)
        {
            var totalCount = _context.Users.Count();
            var users = _context.Users
                .Where(u => u.Login.Contains(loginSubstring))
                .OrderBy(u => u.Login)
                .Skip(offset)
                .Take(count)
                .ToList();

            var collection = new UsersCollection<User>
            {
                TotalCount = totalCount,
                Collection = users
            };
            return collection;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool VerifyPassword(string login, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Login == login);
            if (user == null)
                return false;
            var isPasswordVerified = VerifyPassword(user, password);
            return isPasswordVerified;
        }

        private bool VerifyPassword(User user, string password)
        {
            var passwordHash = user.PasswordHash;
            var result = _passwordHasher.VerifyPassword(passwordHash, password);
            return result;
        }
    }
}
