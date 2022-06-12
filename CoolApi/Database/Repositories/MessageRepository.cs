using CoolApi.Database.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CoolApi.Database.Repositories
{
    public class MessageRepository : IRepository<Message>
    {
        private readonly CoolContext _context;

        public MessageRepository(CoolContext context)
        {
            _context = context;
        }

        public Guid Create(Message entity, Guid userId)
        {
            return Guid.NewGuid();
            Save();
        }

        public void Delete(Guid entityId, Guid userId)
        {

            Save();
        }

        public void Hide(Guid entityId, Guid userId)
        {

            Save();
        }

        public Message Read(Guid entityId, Guid userId)
        {
            // todo only userMessages
            var message = _context.Messages.Find(entityId);
            return message;
        }

        public ReadPortionResult<Message> ReadPortion(int offset, int size, Expression<Func<Message, bool>> filter, Guid userId)
        {
            var portion = new ReadPortionResult<Message>
            {
                TotalCount = int.MaxValue,
                DataCollection = Enumerable.Range(0, size).Select(i => new Message())
            };
            return portion;
        }

        public void Update(Message entity, Guid userId)
        {

            Save();
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
