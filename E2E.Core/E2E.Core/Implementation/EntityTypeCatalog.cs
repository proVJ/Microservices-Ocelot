using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using E2E.Core.BL.Interface;
using E2E.Core.Data.Entities;
using E2E.Core.Model.Response;
using E2E.Core.Data.Repository.Interface;
using E2E.Core.Model.Request;

namespace E2E.Core.BL.Implementation
{
  public class EntityTypeCatalog : IEntityTypeCatalog
  {
    private readonly IMapper _mapper;
    private IEntityTypeRepository _iEntityTypeRepository;
    public EntityTypeCatalog(IEntityTypeRepository iEntityTypeRepository, IMapper mapper)
    {
      _mapper = mapper;
      _iEntityTypeRepository = iEntityTypeRepository;
    }
    public async Task<IEnumerable<EntityTypeResponseModel>> GetEntityTypesAsync()
    {
      var data = await _iEntityTypeRepository.GetEntityTypesAsync();

      #region AutoMapperConfig
      var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<EntityType, EntityTypeResponseModel>()
                    //.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.EntityTypeID))
                    //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.EntityTypeName))
                    //.ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreatedDate))
                );
      var mapper = new Mapper(config);
      #endregion

      var entityTypeModels = mapper.Map<IEnumerable<EntityTypeResponseModel>>(data);

      return entityTypeModels;

    }

    public bool SaveEntityType(EntityTypeRequestModel entityTypeRequestModel)
    {
      //Initialization
      EntityType entityType = new EntityType();
      entityType.EntityTypeName = entityTypeRequestModel.EntityTypeName;
      entityType.CreatedDate = DateTime.UtcNow;
      entityType.IsActive = true;

      try
      {
         _iEntityTypeRepository.SaveEntityType(entityType);
      }
      catch (Exception ex)
      {
        return false;
      }
       
      return true;
    }
  }
}
