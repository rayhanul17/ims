namespace IMS.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public ResponseTypes Type { get; set; }
    }
    public enum ResponseTypes
    {
        Success,
        Info,
        Warning,
        Danger
    }
}