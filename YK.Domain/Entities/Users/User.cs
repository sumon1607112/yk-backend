using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace YK.Domain.Entities.Users
{
    public class User
    {
        public long Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
