using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolApi.Database.Models
{
    public class Chat
    {
        public Guid Id { get; set; }

        public DateTime CreationTimeUtc { get; set; }

        public IEnumerable<ChatMember> ChatMembers { get; set; }
    }
}
