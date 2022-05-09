using System;

namespace CoolApi.Database.Models
{
    public class Deleted
    {
        public Guid Id { get; set; }

        public DateTime TimeUtc { get; set; }
    }
}
