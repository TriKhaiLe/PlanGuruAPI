﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MembershipHistory
    {
        [Key]
        public int Id { get; set; }     
        public Guid UserId { get; set; }
        public virtual User User { get; set; }  
        public string PackageName { get; set; }
        public double PackagePrice { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
