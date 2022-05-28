using System;

namespace CoolApi.Database.Options
{
    public class ReadPortionChatOptions : BaseReadPortionOptions
    {
        public Guid UserId { get; init; }
    }
}
