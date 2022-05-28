using System;

namespace CoolApi.Database.Options
{
    public class DeleteChatOptions : BaseDeleteOptions
    {
        public Guid UserId { get; set; }
        
        public bool IsForAll { get; set; }
    }
}
