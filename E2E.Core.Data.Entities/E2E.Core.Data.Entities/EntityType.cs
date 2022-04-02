using System;
using System.Collections.Generic;
using System.Text;

namespace E2E.Core.Data.Entities
{
  public class EntityType
  {
    // <summary> EntityTypeID </summary>	
    public int EntityTypeID { get; set; }

    // <summary> EntityTypeName </summary>	
    public string EntityTypeName { get; set; }

    // <summary> CreatedDate </summary>	
    public DateTime? CreatedDate { get; set; }

    // <summary> UpdateDate </summary>	
    public DateTime? UpdateDate { get; set; }

    // <summary> CreatedUser </summary>	
    public int? CreatedUser { get; set; }

    // <summary> UpdatedUser </summary>	
    public int? UpdatedUser { get; set; }

    // <summary> IsActive </summary>	
    public bool IsActive { get; set; }


  }
}
