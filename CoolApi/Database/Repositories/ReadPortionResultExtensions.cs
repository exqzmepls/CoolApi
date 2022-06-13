using CoolApi.Database.Models;
using CoolApiModels.Chats;
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
    }
}
