using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public DateTime SendingTimeUtc { get; set; }

        public DateTime? ModificationTimeUtc { get; set; }

        public bool IsViewed { get; set; }

        public string Text { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }
    }
}
