using IMS.BusinessRules.Enum;
using System;

namespace IMS.BusinessModel.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Status Status { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreationDate { get; set; }
        public long ModifyBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int Rank { get; set; }
        public int? VersionNumber { get; set; }
        public string BusinessId { get; set; }
    }
}
