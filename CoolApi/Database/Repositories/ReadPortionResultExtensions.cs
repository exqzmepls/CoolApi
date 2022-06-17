using CoolApi.Database.Models;
using CoolApiModels.Chats;
using CoolApiModels.Messages;
using System.Linq;

namespace CoolApi.Database.Repositories
{
    public static class ReadPortionResultExtensions
    {
        public static ChatsPortionDetails GetDto(this ReadPortionResult<Chat> chatsPortion)
        {
            var chats = chatsPortion.DataCollection;
            var dto = new ChatsPortionDetails
            {
                Offset = chatsPortion.Offset,
                Portion = chats.Count(),
                TotalCount = chatsPortion.TotalCount,
                Content = chats.Select(c => new ChatShortDetails
                {
                    Id = c.Id,
                    CreationTimeUtc = c.CreationTimeUtc
                })
            };
            return dto;
        }

        public static MessagesPortionDetails GetDto(this ReadPortionResult<Message> messagesPortion)
        {
            var messages = messagesPortion.DataCollection;
            var response = new MessagesPortionDetails
            {
                Offset = messagesPortion.Offset,
                Portion = messages.Count(),
                TotalCount = messagesPortion.TotalCount,
                Content = messages.Select(m => new MessageShortDetails
                {
                    Id = m.Id,
                    IsViewed = m.IsViewed,
                    SendingTimeUtc = m.SendingTimeUtc,
                    SenderId = m.ChatMember.UserId,
                    ModificationTimeUtc = m.ModificationTimeUtc,
                    Text = m.Text,
                    AttachmentsCount = m.Attachments.Count
                })
            };
            return response;
        }
    }
}
