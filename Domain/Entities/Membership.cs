﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Membership : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }       

    }
}