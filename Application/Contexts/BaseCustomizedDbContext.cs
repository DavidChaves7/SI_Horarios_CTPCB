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

        public DbSet<CPC_PUNTAJE_CAPACIDAD> puntaje { get; set; }

        public DbSet<Materia> Materias { get; set; }



        #endregion

        #region Ctor

        protected BaseCustomizedDbContext(
            DbContextOptions options
        ) : base(options)
        {
        }

        #endregion


        #region Public Methods override

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CPC_PUNTAJE_CAPACIDAD>().ToTable(nameof(CPC_PUNTAJE_CAPACIDAD), "CPC");
            modelBuilder.Entity<CPC_PUNTAJE_CAPACIDAD>().HasKey(x => new { /*x.COD_COMPANIA, */ x.PUNTAJE });
            modelBuilder.Entity<Materia>().ToTable(nameof(Materia));
            modelBuilder.Entity<Materia>().HasKey(x => new { x.Id_Materia });

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
