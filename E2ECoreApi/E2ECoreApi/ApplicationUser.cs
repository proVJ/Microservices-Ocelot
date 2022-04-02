using Microsoft.AspNetCore.Identity;

namespace E2E.Core.Api
{
  public class ApplicationUsers : IdentityUser
  {
    public string DOB { get; set; }
    public string Note { get; set; }
  }
}
