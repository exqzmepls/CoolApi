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

        private readonly IRepository<Message> _messagesRepository;

        public ChatRepository(CoolContext context)
        {
            _context = context;
            _messagesRepository = new MessageRepository(context);
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
            var chat = Read(entityId, userId);
            if (chat == null)
                throw new Exception("Chat not found");

            _context.Chats.Remove(chat);
            Save();
        }

        // pox
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
            var userChats = _context.Chats
                .Where(c => c.ChatMembers.Any(m => m.UserId == userId))
                .Where(filter)
                .ToList();
            var lastVisibleMessages = userChats
                .Select(c => _messagesRepository.ReadPortion(0, 1, (m) => m.ChatMember.ChatId == c.Id, userId).DataCollection.SingleOrDefault())
                .Where(m => m != null);
            var totalCount = lastVisibleMessages.Count();
            var chats = lastVisibleMessages
                .Skip(offset)
                .Take(size)
                .Select(m => m.ChatMember.Chat);

            var portion = new ReadPortionResult<Chat>
            {
                TotalCount = totalCount,
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
