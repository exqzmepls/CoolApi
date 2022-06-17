using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class Chat
    {
        public Guid Id { get; set; }

        public DateTime CreationTimeUtc { get; set; }

        public virtual ICollection<ChatMember> ChatMembers { get; set; }
    }
}
