//using Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;
using Domain.Entities;

namespace Application.Contexts
{
    public abstract class BaseCustomizedDbContext : DbContext
    {

        #region DbSets

        //public DbSet<SR_USUARIOS> Users { get; set; }
              

        #endregion

        #region Ctor

        protected BaseCustomizedDbContext(
            DbContextOptions options
        ) : base(options)
        {
        }

        #endregion


        #region Public Methods override

        public int SaveChanges(string codUsuario)
            => SaveChangesAsync(codUsuario, CancellationToken.None).Result;

        public async Task<int> SaveChangesAsync(string codUsuario, CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges()
            => SaveChangesAsync(CancellationToken.None).Result;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CPC_PUNTAJE_CAPACIDAD>().ToTable(nameof(CPC_PUNTAJE_CAPACIDAD), "CPC");
            modelBuilder.Entity<CPC_PUNTAJE_CAPACIDAD>().HasKey(x => new { /*x.COD_COMPANIA, */ x.PUNTAJE });

            base.OnModelCreating(modelBuilder);
        }

        #endregion


        public void Delete<TEntity>(TEntity entity)
        {
            base.Remove(entity!);
        }

        public int Commit() => base.SaveChanges();

        public Task<int> CommitAsync() => SaveChangesAsync();
    }
}
