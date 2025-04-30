using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using MalyFarmar.Api.DAL.Models.Shared;

namespace MalyFarmar.Api.DAL.Models;

public class Order : BaseModel
{
    public DateTime? PickUpAt { get; set; }

    [Required]
    public int BuyerId { get; set; }

    [ForeignKey("BuyerId")]
    public virtual User? Buyer { get; set; }

    [Required]
    public OrderStatusEnum StatusId { get; set; }

    [ForeignKey("StatusId")]
    public virtual OrderStatus? Status { get; set; }

    public virtual ICollection<OrderItem>? Items { get; set; } = new List<OrderItem>();

    [NotMapped]
    public decimal TotalPrice => Items?.Sum(i => i.Amount * i.PricePerUnit) ?? 0;
}