using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class Deleted
    {
        public Guid Id { get; set; }

        public DateTime TimeUtc { get; set; }

        public virtual ICollection<DeletedMessage> DeletedMessages { get; set; }

        public virtual ICollection<ChatMemberDeleted> ChatMemberDeleteds { get; set; }
    }
}
