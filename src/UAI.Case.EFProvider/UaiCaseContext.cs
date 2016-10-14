using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAI.Case.Domain.Common;

namespace UAI.Case.EFProvider
{
    public class UaiCaseContext : DbContext, IDbContext
    {


        public void Commit()
        {
            SaveChanges();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }
            
            base.OnModelCreating(builder);
        }
    }
}
