using System;

namespace CoolApi.Database.Models
{
    public class DeletedMessage
    {
        public Guid MessageId { get; set; }

        public virtual Message Message { get; set; }

        public Guid DeletedId { get; set; }

        public virtual Deleted Deleted { get; set; }
    }
}
