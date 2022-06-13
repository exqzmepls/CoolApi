using CoolApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CoolApi.Database.Repositories
{
    public class ChatRepository : IRepository<Chat>
    {
        private readonly CoolContext _context;

        public ChatRepository(CoolContext context)
        {
            _context = context;
        }

        public Guid Create(Chat entity, Guid userId)
        {
            var chatId = Guid.NewGuid();
            var chatMembers = entity.ChatMembers.Select(m => new ChatMember
            {
                Id = Guid.NewGuid(),
                UserId = m.UserId,
                ChatId = chatId
            });
            var creator = chatMembers.First();
            if (creator.UserId != userId)
                throw new Exception("Invalid operation");
            var receiver = chatMembers.Skip(1).First();

            var isChatExist = _context.Chats.Any(c => c.ChatMembers.Any(m => m.UserId == creator.UserId) && c.ChatMembers.Any(m => m.UserId == receiver.UserId));
            if (isChatExist)
                throw new Exception("Chat already exists");

            var chat = new Chat
            {
                Id = chatId,
                CreationTimeUtc = DateTime.UtcNow
            };

            var createdChat = _context.Chats.Add(chat);
            _context.ChatMembers.AddRange(chatMembers);
            Save();

            foreach (var chatMember in chatMembers)
                _context.Users.Where(u => u.Id == chatMember.UserId).Load();
            var createdChatId = createdChat.Entity.Id;
            return createdChatId;
        }

        public void Delete(Guid entityId, Guid userId)
        {
            var chat = _context.Chats.Find(entityId);
            if (chat == null)
                throw new Exception("Chat not found");

            _context.Chats.Remove(chat);
            Save();
        }

        public void Hide(Guid entityId, Guid userId)
        {

            Save();
        }

        public Chat Read(Guid entityId, Guid userId)
        {
            var chat = _context.Chats.Find(entityId);
            if (chat != null)
            {
                var isUserChat = chat.ChatMembers.Any(m => m.UserId == userId);
                if (isUserChat)
                    return chat;
                return default;
            }

            var chatByReceiverId = _context.Chats.FirstOrDefault(
                c => c.ChatMembers.Any(m => m.UserId == userId) &&
                c.ChatMembers.Any(m => m.UserId == entityId));
            return chatByReceiverId;
        }

        public ReadPortionResult<Chat> ReadPortion(int offset, int size, Expression<Func<Chat, bool>> filter, Guid userId)
        {
            var chatMemberments = _context.ChatMembers.Where(m => m.Chat.ChatMembers.Any(m => m.UserId == userId));

            var visibleMessages = chatMemberments.Where(m => m.Messages.Any(m => !m.DeletedMessages.Any()));
            var visibleChats = chatMemberments.Where(m => m.Messages.Any());
            var totalConut = _context.Chats.Where(c => c.ChatMembers.Any(m => m.UserId == userId))
                .Where(c => c.ChatMembers.First(m => m.UserId == userId).Messages.Any(m => !m.DeletedMessages.Any(d => d.Deleted.ChatMemberDeleteds.Any(d => d.ChatMember.UserId == userId))))
                .Count();
            var chats = _context.Chats.Where(c => c.ChatMembers.Any(m => m.UserId == userId))
                .Where(c => c.ChatMembers.First(m => m.UserId == userId).Messages.Any(m => !m.DeletedMessages.Any()))
                .Where(filter)
                .Skip(offset)
                .Take(size)
                .ToList();

            var portion = new ReadPortionResult<Chat>
            {
                TotalCount = totalConut,
                Offset = offset,
                DataCollection = chats
            };
            return portion;
        }

        public void Update(Chat entity, Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves all changes to the DB.
        /// </summary>
        /// <exception cref="DbUpdateException"></exception>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        private void Save()
        {
            _context.SaveChanges();
        }
    }
}
