using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace MalyFarmar.Api.DAL.Models;

public class OrderStatus
{
    [Key]
    public OrderStatusEnum Id { get; set; }

    [Required]
    public string Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}