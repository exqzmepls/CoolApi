using CoolApi.Database.Models;
using Microsoft.EntityFrameworkCore;
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
            var chatMemberDetails = entity.ChatMember;
            var isUser = chatMemberDetails.UserId == userId;
            if (!isUser)
                throw new Exception("Invalid operation");

            var chatMember = _context.ChatMembers.SingleOrDefault(m => m.ChatId == chatMemberDetails.ChatId && m.UserId == chatMemberDetails.UserId);
            if (chatMember == null)
                throw new Exception("Invalid operation");

            var messageId = Guid.NewGuid();
            var attachments = entity.Attachments?.Select(a => new Attachment
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                Content = a.Content
            });
            var message = new Message
            {
                Id = messageId,
                SendingTimeUtc = DateTime.UtcNow,
                IsViewed = false,
                ModificationTimeUtc = null,
                ChatMemberId = chatMember.Id,
                Text = entity.Text,
                Attachments = attachments?.ToList()
            };

            var createdMessage = _context.Messages.Add(message);
            var isAttachmentsExist = attachments != null;
            if (isAttachmentsExist)
                _context.Attachments.AddRange(attachments);
            Save();

            if (isAttachmentsExist)
            {
                foreach (var attachment in attachments)
                    _context.Attachments.Where(a => a.MessageId == messageId).Load();
            }
            var createdMessageId = createdMessage.Entity.Id;
            return createdMessageId;
        }

        public void Delete(Guid entityId, Guid userId)
        {
            var message = Read(entityId, userId);
            if (message == null)
                throw new Exception("Message not found");

            _context.Messages.Remove(message);
            Save();
        }

        // later
        public void Hide(Guid entityId, Guid userId)
        {

            Save();
        }

        public Message Read(Guid entityId, Guid userId)
        {
            var message = _context.Messages.Find(entityId);
            if (message != null)
            {
                var isUserMessage = message.ChatMember.Chat.ChatMembers.Any(m => m.UserId == userId);
                if (isUserMessage)
                {
                    var isDeleted = _context.DeletedMessages
                        .Where(d => d.MessageId == message.Id)
                        .Select(d => d.Deleted)
                        .Any(d => d.ChatMemberDeleteds.Any(d => d.ChatMember.UserId == userId));
                    if (isDeleted)
                        return default;
                    return message;
                }
                return default;
            }
            return message;
        }

        public ReadPortionResult<Message> ReadPortion(int offset, int size, Expression<Func<Message, bool>> filter, Guid userId)
        {
            var messagesQuery = _context.Messages
                .Where(m => m.ChatMember.Chat.ChatMembers.Any(m => m.UserId == userId))
                .Where(m => !m.DeletedMessages.Select(d => d.Deleted).Any(d => d.ChatMemberDeleteds.Any(d => d.ChatMember.UserId == userId)))
                .Where(filter);
            var totalCount = messagesQuery.Count();
            var messages = messagesQuery
                .OrderByDescending(m => m.SendingTimeUtc)
                .Skip(offset)
                .Take(size)
                .ToList();

            var portion = new ReadPortionResult<Message>
            {
                TotalCount = totalCount,
                Offset = offset,
                DataCollection = messages
            };
            return portion;
        }

        public void Update(Message entity, Guid userId)
        {
            var message = Read(entity.Id, userId);
            if (message == null)
                throw new Exception("Message not found");

            if (message.ChatMember.UserId == userId)
            {
                if (string.IsNullOrEmpty(entity.Text) && (entity.Attachments == null || !entity.Attachments.Any()))
                    throw new Exception("Text and Attachments are empty.");


            }
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
