using E2E.Core.Model.Request;
using E2E.Core.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E2E.Core.BL.Interface
{
  public interface IEntityTypeCatalog
  {
    Task<IEnumerable<EntityTypeResponseModel>> GetEntityTypesAsync();
    bool SaveEntityType(EntityTypeRequestModel entityTypeRequestModel);
  }
}
