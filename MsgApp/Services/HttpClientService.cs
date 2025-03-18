using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;


namespace MsgApp.Services
{
  public class HttpClientService
  {
    public HttpClient? Client { get; }

    public HttpClientService()
    {
      Client = new HttpClient();
    }
  }
}