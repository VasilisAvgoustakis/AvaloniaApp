using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MsgApp.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using MsgApp.Services;
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
        //Basisverzeichnis
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        // Pfad zur Bilddatei.
        string messagesDatabPath = Path.Combine(baseDir, "Data", "sample-messages.json");

        var messages = _messageLoader.LoadMessagesFromJson(messagesDatabPath);
        Messages = new ObservableCollection<Message>(messages ?? new List<Message>());

        // berechne Gravatar Urls und setze AvatarUrl property of Messages
        SetMessageAvatars();
        // nach Datum Sortieren on Startup
        SortByDate();
        // Erste Nachricht als beim default selektierte Nachricht auswählen
        SelectedMessage = Messages.First();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Fehler beim Laden der Messages in ViewModel! ");
      }

    }

    private void SetMessageAvatars()
    {
      _logger.LogInformation($"Lade und setzte Avatars!");

      try
      {
        // berechne Gravatar Urls und setze AvatarUrl property of Messages
        if (Messages is not null) // Weg mit der CS8602 Warning
        {
          foreach (var msg in Messages)
          {
            // Bewusst Fire-and-Forget: Avatare laden im Hintergrund
            var _ = _gravatarService.LoadAvatarAsync(msg);
          }
        }

        _logger.LogInformation($"Avatars erfolgreich geladen!");
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Fehler beim Laden und Setzten von Avatars in MainWindowViewModel!");
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