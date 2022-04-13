using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public List<ChatMember> ChatMemberments { get; set; }
    }
}
