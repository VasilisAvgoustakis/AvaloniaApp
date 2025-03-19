using System;
using System.Collections.Generic;
using MsgApp.Models;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MsgApp.Services
{ 
  public class MessageStateService
  {

    private readonly ILogger<MessageStateService> _logger;
    private readonly ITimerService _timerService;

    public MessageStateService(ILogger<MessageStateService>  logger, ITimerService timerService)
    {
      _logger = logger;
      _timerService = timerService;
    }

    public async void MarkAsReadAfterDelay(Message? msg, Message? selectedMessage, CancellationToken token)
    {
      try
      {
        //await Task.Delay(3000, token);
        await _timerService.DelayAsync(TimeSpan.FromMilliseconds(3000), token);

        // Falls nicht gecancelled und noch ausgewählt
        if (!token.IsCancellationRequested && selectedMessage == msg && msg is not null)
        {
          msg.IsRead = true;
          //OnPropertyChanged("IsRead");

          _logger.LogInformation("Nachricht von {msg.SenderName} erfolgreich vom Nutzer gelesen!", msg.SenderName);
        }

        
      }
      catch (Exception ex)
      {
        // Timer abgebrochen also nichts tun
        _logger.LogError(ex, "Fehler beim 'MarkASReadAfterDelay'!");
      }
    }
  }
}