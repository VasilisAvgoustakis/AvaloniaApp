using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using MsgApp.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MsgApp.Services;

namespace MsgApp.ViewModels
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private readonly JsonMessageLoader _messageLoader;
    private readonly ILogger<MainWindowViewModel> _logger;

    private readonly ITimerService _timerService;

    private CancellationTokenSource? _readCancellation;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<Message>? Messages { get; set; }

    private Message? _selectedMessage;
    public Message? SelectedMessage 
    { 
      get => _selectedMessage;
      set
      {
        if (_selectedMessage == value) return;

        _selectedMessage = value;
        OnPropertyChanged();

        // 3 sec timer
        // abrrechen falls schon ein Timer läuft
        _readCancellation?.Cancel();
        _readCancellation = new CancellationTokenSource();

        if (_selectedMessage != null)
        {
          // Async
          MarkAsReadAfterDelay(_selectedMessage, _readCancellation.Token);
        }
      }
    }

    // Konstruktor fürs MainWindow
    public MainWindowViewModel(JsonMessageLoader messageLoader, ILogger<MainWindowViewModel> logger, ITimerService timerService)
    {
      _logger = logger;

      _timerService = timerService;

      // Messages laden
      _messageLoader = messageLoader;
      try
      {
        var messages = _messageLoader.LoadMessagesFromJson("../MsgApp/Data/sample-messages.json");
        Messages = new ObservableCollection<Message>(messages ?? new List<Message>());
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Fehler beim Laden der Messages in ViewModel! ");
      }

    }
    

    public void SortBySender()
    {
        if (Messages == null) return;

        var sorted = Messages.OrderBy(m => m.SenderName ?? string.Empty);

        Messages = new ObservableCollection<Message>(sorted);
        OnPropertyChanged(nameof(Messages));
    }

    public void SortByDate()
    {
        if (Messages == null) return;

        var sorted = Messages.OrderBy(m => m.SentDate);

        Messages = new ObservableCollection<Message>(sorted);

        OnPropertyChanged(nameof(Messages));
    }

    private async void MarkAsReadAfterDelay(Message? msg, CancellationToken token)
    {
      try
      {
        //await Task.Delay(3000, token);
        await _timerService.DelayAsync(TimeSpan.FromMilliseconds(3000), token);

        // Falls nicht gecancelled und noch ausgewählt
        if (!token.IsCancellationRequested && SelectedMessage == msg && msg is not null)
        {
          msg.IsRead = true;
          OnPropertyChanged("IsRead");

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