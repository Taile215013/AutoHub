using System;
using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;

    public int? UserId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public User? User { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
