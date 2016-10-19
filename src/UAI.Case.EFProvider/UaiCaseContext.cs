using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAI.Case.Domain.Common;
using Npgsql.EntityFrameworkCore.PostgreSQL;



namespace UAI.Case.EFProvider
{
    public class UaiCaseContext : DbContext, IDbContext
    {

        //public UaiCaseContext(DbContextOptions<UaiCaseContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }


        String _cs;
        public UaiCaseContext(String cs)
        {
            _cs = cs;
        }

        public void Commit()
        {
            SaveChanges();
            
        }

        public IQueryable<T> FromSQL<T>(string qry, params string[] parameters) where T : class
        {
          return  this.Set<T>().FromSql(qry, parameters);
        }


        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {

            optionsBuilder.UseNpgsql(_cs);
            
            //optionsBuilder.UseSqlServer(_cs);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

           foreach (var entity in builder.Model.GetEntityTypes())
            {
               entity.Relational().TableName = entity.DisplayName();
            }
            
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("uuid-ossp");
        }
    }


   
}




