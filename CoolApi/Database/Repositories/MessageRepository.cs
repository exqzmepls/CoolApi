﻿using CoolApi.Database.Models;
using System;
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
            Save();
            throw new NotImplementedException();
        }

        public void Delete(Guid entityId, Guid userId)
        {
            Save();
            throw new NotImplementedException();
        }

        public void Hide(Guid entityId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Message Read(Guid entityId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public ReadPortionResult<Message> ReadPortion(int offset, int size, Expression<Func<Message, bool>> filter, Guid userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Message entity, Guid userId)
        {
            Save();
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