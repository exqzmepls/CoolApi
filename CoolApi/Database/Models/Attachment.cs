using System;

namespace CoolApi.Database.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public virtual Message Message { get; set; }

        public string Content { get; set; }
    }
}
