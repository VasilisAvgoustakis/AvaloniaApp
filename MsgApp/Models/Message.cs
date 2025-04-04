using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;


namespace MsgApp.Models
{
    public class Message
    {
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? Subject { get; set; }
        public DateTime? SentDate { get; set; }
        public string? Content { get; set; }
        public bool? IsRead{get; set;}
        public Bitmap? AvatarBitmap{get; set;}

    }
}