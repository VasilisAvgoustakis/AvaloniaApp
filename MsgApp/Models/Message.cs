using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace MsgApp.Models
{
  public class Message : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
     => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set; }
    public string? GravatarPath { get; set; }
    public string? Subject { get; set; }
    public DateTime? SentDate { get; set; }
    public string? Content { get; set; }
    private bool? _isRead;
    public bool? IsRead
        {
            get => _isRead;
            set
            {
                if (_isRead != value)
                {
                    _isRead = value;
                    OnPropertyChanged();
                }
            }
        }

  }
}