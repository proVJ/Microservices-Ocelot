using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E2E.Core.Data.DataContext;
using E2E.Core.Data.Entities;
using E2E.Core.Data.Repository.Interface;

namespace E2E.Core.Data.Repository.Implementation
{
  public class EntityTypeRepository : IEntityTypeRepository
  {
    private readonly E2EDbContext _dbContext;
    public EntityTypeRepository(E2EDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    public async Task<IEnumerable<EntityType>> GetEntityTypesAsync()
    {
      var query = from item in _dbContext.Entities
                  where item.IsActive == true
                  select item;

      //return await Task.FromResult(query);
      return await query.ToListAsync();
    }

    public void SaveEntityType(EntityType entityType)
    {
      _dbContext.Entities.Add(entityType);
      _dbContext.SaveChanges();
    }
  }
}
