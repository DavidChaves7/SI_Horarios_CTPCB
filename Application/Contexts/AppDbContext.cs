using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contexts
{
    public sealed class AppDbContext : BaseCustomizedDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = true;
        }


    }
}
