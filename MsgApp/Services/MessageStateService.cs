using System;
using MsgApp.Models;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MsgApp.Services
{ 
  public class MessageStateService
  {

    private readonly ILogger<MessageStateService> _logger;
    public MessageStateService(ILogger<MessageStateService>  logger)
    {
      _logger = logger;
    }

    public async Task<bool> MarkAsReadAfterDelay(Message? msg, Message? selectedMessage, CancellationToken token)
    {
      try
      {
        //await Task.Delay(3000, token);
        await Task.Delay(3000, token);

        // Falls nicht gecancelled und noch ausgewählt
        if (!token.IsCancellationRequested && selectedMessage == msg && msg is not null)
        {
          return true;
        }

        return false;
      }
      catch
      {
        // Timer abgebrochen also nichts tun
        return false;
      }
    }
  }
}