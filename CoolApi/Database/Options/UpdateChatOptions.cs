using CoolApi.Database.Models;
using System;

namespace CoolApi.Database.Options
{
    public class UpdateChatOptions : BaseUpdateOptions<Chat>
    {
        public Guid Id { get; init; }
    }
}
