using System.ComponentModel.DataAnnotations;
using MalyFarmar.Api.DAL.Enums;

namespace MalyFarmar.Api.DAL.Models;

public class OrderStatus
{
    [Key]
    public OrderStatusEnum Id { get; set; }

    [Required]
    public string Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}