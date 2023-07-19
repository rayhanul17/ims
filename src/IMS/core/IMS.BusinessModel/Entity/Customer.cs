﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.BusinessModel.Entity
{
    public class Customer : BaseEntity<long>
    {
        public virtual string Address { get; set; }
        public virtual string ContactNumber { get; set; }
        public virtual string Email { get; set; }
    }
}