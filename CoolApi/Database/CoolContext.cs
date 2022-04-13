using CoolApi.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolApi.Database
{
    public class CoolContext : DbContext
    {
        public CoolContext(DbContextOptions<CoolContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatMember> ChatMembers { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Attachment> Attachments { get; set; }
    }
}
