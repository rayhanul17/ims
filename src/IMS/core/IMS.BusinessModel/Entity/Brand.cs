﻿namespace IMS.BusinessModel.Entity
{
    public class Brand : BaseEntity<long>
    {        
        public virtual string Description { get; set; }

        //public virtual IList<Product> Products { get; set; } = new List<Product>();
    }
}
