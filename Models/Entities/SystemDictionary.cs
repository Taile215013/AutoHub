namespace AutoHub.Models.Entities
{
    public class SystemDictionary : BaseEntity
    {
        public string Type { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        // Bổ sung: Liên kết với Code của từ điển cha (ví dụ: Auto, Motorbike)
        public string? ParentCode { get; set; }
    }
}
