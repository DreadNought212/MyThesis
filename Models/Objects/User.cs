﻿using Artium.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Models.Objects
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public int? UserInfoId { get; set; }
        public UserInfo? UserInfo { get; set; }
    }
}
