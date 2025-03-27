using System.ComponentModel;
using Avalonia.Media.Imaging;
using MsgApp.Models;
using System.Runtime.CompilerServices;
using System;


namespace MsgApp.ViewModels
{
  public class MessageViewModel : INotifyPropertyChanged
  {
    private readonly Message _message;

    public MessageViewModel (Message message)
    {
      _message = message;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SenderName => _message.SenderName ?? string.Empty;

    public Message Message
    {
      get => _message;
    }

    public bool? IsRead
    {
      get => _message.IsRead;
      set
      {
        if (_message.IsRead != value)
        {
          _message.IsRead = value;
          OnPropertyChanged();
        }
      }
    }

    public DateTime? SentDate
    {
      get => _message.SentDate;
    }

    public string? Subject
    {
      get => _message.Subject;
    }

    public Bitmap? AvatarBitmap
    {
      get => _message.AvatarBitmap;
      set
      {
        if (_message.AvatarBitmap != value)
        {
          _message.AvatarBitmap = value;
        }
        _message.AvatarBitmap = value;
        OnPropertyChanged();
      }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}



