using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MsgApp.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MsgApp.Services;
using Avalonia.Media.Imaging;
using System.IO;

namespace MsgApp.ViewModels
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly JsonMessageLoader _messageLoader;
    private readonly GravatarService _gravatarService;
    private readonly MessageStateService _messageStateService;
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
          _messageStateService.MarkAsReadAfterDelay(_selectedMessage, _selectedMessage, _readCancellation.Token);
          OnPropertyChanged("IsRead");
        }
      }
    }

    // Konstruktor fürs MainWindow
    public MainWindowViewModel(JsonMessageLoader messageLoader, 
                              ILogger<MainWindowViewModel> logger, 
                              MessageStateService messageStateService,
                              GravatarService gravatarService)
    {
      _gravatarService = gravatarService;
      _logger = logger;
      _messageStateService = messageStateService;
      _messageLoader = messageLoader;
      
      
      try
      {
        var messages = _messageLoader.LoadMessagesFromJson("../MsgApp/Data/sample-messages.json");
        Messages = new ObservableCollection<Message>(messages ?? new List<Message>());

        // berechne Gravatar Urls und setze AvatarUrl property of Messages
        if (messages is not null) // Weg mit der CS8602 Warning
        {
            foreach (var msg in messages)
            {
                // Bewusst Fire-and-Forget: Avatare laden im Hintergrund
                var _ = _gravatarService.LoadAvatarAsync(msg);
            }
        }

        SelectedMessage = Messages.First();
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
  }
}