using CoolApi.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolApi.Database
{
    public class CoolContext : DbContext
    {
        public CoolContext(DbContextOptions<CoolContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatMember> ChatMembers { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Deleted> Deleteds { get; set; }

        public DbSet<ChatMemberDeleted> ChatMemberDeleteds { get; set; }

        public DbSet<DeletedMessage> DeletedMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMemberDeleted>().HasKey(e => new { e.ChatMemberId, e.DeletedId });
            modelBuilder.Entity<DeletedMessage>().HasKey(e => new { e.DeletedId, e.MessageId });

            modelBuilder.Entity<ChatMember>().HasAlternateKey(e => new { e.ChatId, e.UserId });
            modelBuilder.Entity<User>().HasAlternateKey(e => new { e.Login });
        }
    }
}
