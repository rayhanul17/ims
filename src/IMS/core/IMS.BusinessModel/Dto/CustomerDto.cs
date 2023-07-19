namespace IMS.BusinessModel.Dto
{
    public class CustomerDto : BaseDto<long>
    {
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
    }
}
