using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class Deleted
    {
        public Guid Id { get; set; }

        public DateTime TimeUtc { get; set; }

        public IEnumerable<DeletedMessage> DeletedMessages { get; set; }

        public IEnumerable<ChatMemberDeleted> ChatMemberDeleteds { get; set; }
    }
}
