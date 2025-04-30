using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MalyFarmar.Api.DAL.Models.Shared;

namespace MalyFarmar.Api.DAL.Models;

public class OrderItem : BaseModel
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public decimal PricePerUnit { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}