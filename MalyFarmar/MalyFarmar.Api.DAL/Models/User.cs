using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Geolocation;
using MalyFarmar.Api.DAL.Models.Shared;

namespace MalyFarmar.Api.DAL.Models;

public class User : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; }

    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
