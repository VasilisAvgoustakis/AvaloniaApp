using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using MsgApp.Services;
using System.IO;
using System.Threading.Tasks;

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
    public ObservableCollection<MessageViewModel>? Messages { get; set; }
    private MessageViewModel? _selectedMessage;
    public MessageViewModel? SelectedMessage
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
            // Start the async read-delay in a separate (async) method
            _ = MarkAsReadWithDelayAsync(_selectedMessage, _readCancellation.Token);
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
        Messages = new ObservableCollection<MessageViewModel>(messages.Select(m => new MessageViewModel(m)));

        // berechne Gravatar Urls und setze AvatarUrl property of Messages
        var _ = SetMessageAvatarsAsync();
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

    // Then an async helper method:
    private async Task MarkAsReadWithDelayAsync(MessageViewModel vm, CancellationToken token)
    {
        var success = await _messageStateService
            .MarkAsReadAfterDelay(vm.Message, vm.Message, token);

        // If MarkAsReadAfterDelay returned e.g. a bool, we can check
        if (success)
        {
            // Because we are on the VM side, we should set the 
            // IsRead property on the *MessageViewModel*, not the model.
            vm.IsRead = true;
            _logger.LogInformation("Set VM.IsRead = true on {Message}", vm.Subject);
        }
    }

    public async Task SetMessageAvatarsAsync()
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
            var bmp = await _gravatarService.LoadAvatarAsync(msg.Message);
            msg.AvatarBitmap = bmp;
          }
        }

        _logger.LogInformation($"Avatars erfolgreich geladen!");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Fehler beim Laden und Setzten von Avatars in MainWindowViewModel!");
      }

    }

    public void SortBySender()
    {
      if (Messages == null) return;

      var sorted = Messages.OrderBy(m => m.SenderName ?? string.Empty);

      Messages = new ObservableCollection<MessageViewModel>(sorted);
      OnPropertyChanged(nameof(Messages));
    }

    public void SortByDate()
    {
      if (Messages == null) return;

      var sorted = Messages.OrderByDescending(m => m.Message.SentDate);

      Messages = new ObservableCollection<MessageViewModel>(sorted);

      OnPropertyChanged(nameof(Messages));
    }
  }
}