using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.DAL;

public class MalyFarmarDbContext : DbContext
{
    public MalyFarmarDbContext(DbContextOptions<MalyFarmarDbContext> options) : base(options)
    {
    }
}