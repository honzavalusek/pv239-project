using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MalyFarmar.Api.DAL.Models.Shared;

namespace MalyFarmar.Api.DAL.Models;

public class Product : BaseModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public decimal RemainingAmount { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; }

    [Required]
    public decimal PricePerUnit { get; set; }

    // TODO: Probably add categories later
    /*[Required]
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public virtual ProductCategory? Category { get; set; }*/

    [Required]
    public int SellerId { get; set; }

    [ForeignKey("SellerId")]
    public virtual User? Seller { get; set; }

    public virtual ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
}
