using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolApi.Database.Models
{
    public class Message // todo deleting
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public ChatMember Sender { get; set; }

        public DateTime SendingTimeUtc { get; set; }

        public bool IsModified { get; set; }

        public DateTime? ModificationTimeUtc { get; set; }

        public bool IsViewed { get; set; }

        public string Text { get; set; }

        public List<Attachment> Attachments { get; set; }
    }
}
