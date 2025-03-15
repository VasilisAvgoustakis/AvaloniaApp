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

namespace MsgApp.ViewModels
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private readonly JsonMessageLoader _messageLoader;
    private readonly ILogger<MainWindowViewModel> _logger;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<Message>? Messages { get; set; }
    public Message? SelectedMessage { get; set; }

    // Konstruktor fürs MainWindow
    public MainWindowViewModel(JsonMessageLoader messageLoader, ILogger<MainWindowViewModel> logger)
    {
      _logger = logger;
      // Messages laden
      _messageLoader = messageLoader;
      try
      {
        _logger.LogInformation("Versuchen Messages in MainViewModel zu laden!");
        var messages = _messageLoader.LoadMessagesFromJson("C:/Users/Vasilis Avgoustakis/Documents/Projects/AvaloniaApp/MsgApp/Data/sample-messages.json");
        Messages = new ObservableCollection<Message>(messages ?? new List<Message>());
        _logger.LogInformation("Messages erfolgreich in MainViewModel geladen!");
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Fehler beim Laden: ");
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

  }
}