using CoolApiModels.Chats;
using CoolApiModels.Messages;
using CoolApiModels.Users;
using System.Linq;

namespace CoolApi.Database.Models.Extensions
{
    public static class DtoExtensions
    {
        public static ChatDetails GetDto(this Chat chat)
        {
            var dto = new ChatDetails
            {
                Id = chat.Id,
                CreationTimeUtc = chat.CreationTimeUtc,
                ChatMembers = chat.ChatMembers.Select(m => GetDto(m.User))
            };
            return dto;
        }

        public static UserDetails GetDto(this User user)
        {
            var dto = new UserDetails
            {
                Id = user.Id,
                Login = user.Login
            };
            return dto;
        }

        public static MessageDetails GetDto(this Message message)
        {
            var dto = new MessageDetails
            {
                Id = message.Id,
                SenderId = message.ChatMember.UserId,
                SendingTimeUtc = message.SendingTimeUtc,
                IsViewed = message.IsViewed,
                ModificationTimeUtc = message.ModificationTimeUtc,
                Text = message.Text,
                Attachments = message.Attachments?.Select(a => a.Content)
            };
            return dto;
        }
    }
}
