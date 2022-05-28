using CoolApi.Database.Models;
using System;

namespace CoolApi.Database.Options
{
    public class CreateChatOptions : BaseCreateOptions<Chat>
    {
        public Guid Id { get; init; }
    }
}
