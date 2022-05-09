using System;

namespace CoolApi.Database.Models
{
    public class ChatMemberDeleted
    {
        public Guid ChatMemberId { get; set; }

        public ChatMember ChatMember { get; set; }

        public Guid DeletedId { get; set; }

        public Deleted Deleted { get; set; }
    }
}
