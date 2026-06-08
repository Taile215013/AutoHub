using System;

namespace AutoHub.Models.Entities;

public class SocialPost : BaseEntity
{
    public int UserId { get; set; }
    
    public string Content { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
    
    public User User { get; set; } = null!;
}
