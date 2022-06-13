using CoolApiModels.Chats;
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


    }
}
