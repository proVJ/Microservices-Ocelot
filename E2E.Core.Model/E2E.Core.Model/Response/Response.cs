using System;
using System.Collections.Generic;
using System.Text;

namespace E2E.Core.Model.Response
{
  public class Response<T>
  {
    public string Status { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }

    public T Data { get; set; }
    public Response()
    {
      IsSuccess = false;      
    }
  }
}
