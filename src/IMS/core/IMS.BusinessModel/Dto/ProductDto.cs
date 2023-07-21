using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.Dto
{
    public class ProductDto : BaseDto<long>
    {        
        public virtual string Description { get; set; }  
        public virtual decimal? Price { get; set; }
        public virtual string Category { get; set; } 
    }
}
