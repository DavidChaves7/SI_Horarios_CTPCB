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

        //public DbSet<CPC_PUNTAJE_CAPACIDAD> puntaje { get; set; }

        public DbSet<Materia> Materias { get; set; }

        public DbSet<Profesor> Profesor { get; set; }



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

            //modelBuilder.Entity<CPC_PUNTAJE_CAPACIDAD>().ToTable(nameof(CPC_PUNTAJE_CAPACIDAD), "CPC");
            //modelBuilder.Entity<CPC_PUNTAJE_CAPACIDAD>().HasKey(x => new { /*x.COD_COMPANIA, */ x.PUNTAJE });
            //Table Materia 
            modelBuilder.Entity<Materia>().ToTable(nameof(Materia));
            modelBuilder.Entity<Materia>().HasKey(x => new { x.Id_Materia });

            //Table Profesor
            modelBuilder.Entity<Profesor>().ToTable(nameof(Profesor));
            modelBuilder.Entity<Profesor>().HasKey(x => new { x.Id_Profesor });

            //Table Nivel academico
            modelBuilder.Entity<Nivel_Academico>().ToTable(nameof(Nivel_Academico));
            modelBuilder.Entity<Nivel_Academico>().HasKey(x => new { x.Id_Nivel_Academico });

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
