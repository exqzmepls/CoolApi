using System;

namespace CoolApi.Database.Models
{
    public class ChatMemberMessage
    {
        public Guid MessageId { get; set; }

        public Message Message { get; set; }

        public Guid ChatMemberId { get; set; }

        public ChatMember ChatMember { get; set; }
    }
}
