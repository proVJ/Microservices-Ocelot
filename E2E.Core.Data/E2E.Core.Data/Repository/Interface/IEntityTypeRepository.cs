using E2E.Core.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E2E.Core.Data.Repository.Interface
{
  public interface IEntityTypeRepository
  {
    Task<IEnumerable<EntityType>> GetEntityTypesAsync();
    void SaveEntityType(EntityType entityType);
  }
}
