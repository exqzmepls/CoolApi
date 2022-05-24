namespace CoolApi.Database.Hashers
{
    public interface IPasswordHasher
    {
        public string GetPasswordHash(string password);

        public bool VerifyPassword(string passwordHash, string password);
    }
}
