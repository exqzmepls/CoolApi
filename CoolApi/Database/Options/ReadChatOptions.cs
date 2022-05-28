using System;

namespace CoolApi.Database.Options
{
    public class ReadChatOptions : BaseReadOptions
    {
        public Guid UserId { get; init; }
    }
}
