using IMS.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Models.Dtos.Categories
{
    public class CategoryAdd
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public Status Status { get; set; }        
    }
}
