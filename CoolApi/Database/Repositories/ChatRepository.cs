using CoolApi.Database.Models;
using System;
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
            throw new NotImplementedException();
            var addedChat = _context.Chats.Add(entity);
            Save();
            return addedChat.Entity.Id;
        }

        public void Delete(Guid entityId, Guid userId)
        {

            Save();
        }

        public void Hide(Guid entityId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Chat Read(Guid entityId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public ReadPortionResult<Chat> ReadPortion(int offset, int size, Expression<Func<Chat, bool>> filter, Guid userId)
        {
            throw new NotImplementedException();
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
