﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Models.Database
{
    public class BaseItem
    {
        public const int InvalidId = -1;

        public int Id { get; set; } = InvalidId;
    }
}
