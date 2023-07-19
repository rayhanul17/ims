using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.Dto
{
    public class CategoryDto : BaseDto<long>
    {        
        public virtual string Description { get; set; }        
    }
}
