﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAI.Case.Domain.Common;
using System.Data.SqlClient;

namespace UAI.Case.EFProvider
{
    public class UaiCaseContext : DbContext, IDbContext
    {

        

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
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                //entity.Relational().TableName = entity.DisplayName();
            }
            
            base.OnModelCreating(builder);
        }
    }
}
