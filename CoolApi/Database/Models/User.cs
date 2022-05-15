using System;
using System.Collections.Generic;

namespace CoolApi.Database.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public IEnumerable<ChatMember> ChatMemberments { get; set; }
    }
}
