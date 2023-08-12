namespace IMS.BusinessModel.Dto
{
    public abstract class BaseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Rank { get; set; }
        public string CreateBy { get; set; }
        public string CreationDate { get; set; }
    }
}
