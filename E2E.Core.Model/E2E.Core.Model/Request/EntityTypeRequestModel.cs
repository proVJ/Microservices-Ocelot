using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E2E.Core.Model.Request
{
  public class EntityTypeRequestModel
  {    

    // <summary> EntityTypeName </summary>
    [Required]
    public string EntityTypeName { get; set; }
    
  }
}
