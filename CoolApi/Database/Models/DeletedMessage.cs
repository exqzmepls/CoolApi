using System;

namespace CoolApi.Database.Models
{
    public class DeletedMessage
    {
        public Guid MessageId { get; set; }

        public Message Message { get; set; }

        public Guid DeletedId { get; set; }

        public Deleted Deleted { get; set; }
    }
}
