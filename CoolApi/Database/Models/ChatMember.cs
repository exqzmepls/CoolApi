using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class ChatMember
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }

        public virtual Chat Chat { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<ChatMemberDeleted> ChatMemberDeleteds { get; set; }
    }
}
