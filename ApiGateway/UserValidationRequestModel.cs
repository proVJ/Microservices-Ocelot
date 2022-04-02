using System.ComponentModel.DataAnnotations;

namespace ApiGateway
{
  internal class UserValidationRequestModel
  {
    [Required]
    public string UserName;
    [Required]
    public string Password;
  }
}
