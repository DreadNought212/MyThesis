﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Models.Objects
{
    public class Admin
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}