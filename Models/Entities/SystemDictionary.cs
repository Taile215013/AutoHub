namespace AutoHub.Models.Entities
{
    public class SystemDictionary : BaseEntity
    {
        public string Type { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}
