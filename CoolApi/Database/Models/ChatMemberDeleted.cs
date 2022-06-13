using System;

namespace CoolApi.Database.Models
{
    public class ChatMemberDeleted
    {
        public Guid ChatMemberId { get; set; }

        public virtual ChatMember ChatMember { get; set; }

        public Guid DeletedId { get; set; }

        public virtual Deleted Deleted { get; set; }
    }
}
