using E2E.Core.BL.Interface;
using E2E.Core.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2E.Core.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EntityTypeController : ControllerBase
  {
    private readonly ILogger<EntityTypeController> _logger;
    private IEntityTypeCatalog _entityTypeCatalog;

    public EntityTypeController(ILogger<EntityTypeController> logger, IEntityTypeCatalog entityTypeCatalog)
    {
      _logger = logger;
      _entityTypeCatalog = entityTypeCatalog;
    }


    /// <summary>
    /// Get the list of Entity Type
    /// </summary>
    /// <returns>Returns EntityTypeResponseModel List</returns>
    [HttpGet]
    //[Route("GET")]
    public async Task<IActionResult> Get()
    {
      var data = await _entityTypeCatalog.GetEntityTypesAsync();
      return Ok(data);
    }

    /// <summary>
    /// Get the list of Entity Type
    /// </summary>
    /// <returns>Returns EntityTypeResponseModel List</returns>
    [HttpGet]
    [Route("GetData")]
    public async Task<IActionResult> GetData()
    {
      var data = await _entityTypeCatalog.GetEntityTypesAsync();
      return Ok(data);
    }

    /// <summary>
    /// Save the Entity Type
    /// </summary>
    /// <param name="entityTypeRequestModel"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("SaveEntityType")]
    public IActionResult SaveEntityType([FromForm] EntityTypeRequestModel entityTypeRequestModel)
    {
      if(ModelState.IsValid)
      {
        if(!string.IsNullOrEmpty(entityTypeRequestModel.EntityTypeName.Trim()))
        {
          bool result = _entityTypeCatalog.SaveEntityType(entityTypeRequestModel);
          return Ok(result);
        }        
      }
      return BadRequest();
    }
  }
}
