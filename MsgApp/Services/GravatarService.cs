using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MsgApp.Models;
using System.IO;
using Avalonia.Media.Imaging;
using System.Net.Http;



namespace MsgApp.Services
{
  public class GravatarService
  {
    private readonly ILogger<GravatarService> _logger;
    private readonly HttpClient _httpClient;

    public GravatarService(ILogger<GravatarService> logger, HttpClient httpClient)
    {
      _logger = logger;
      _httpClient = httpClient;
    }


    public string GetGravatarUrl(string? email)
    {
      if (string.IsNullOrWhiteSpace(email)) return "https://www.gravatar.com/avatar/?d=mp";

      // Kleinbuchstaben und trimmen
      string normalizedEmail = email.Trim().ToLowerInvariant();

      // MD5-Hash berechnen
      using (var md5 = MD5.Create()) // using-Block: Objekt muss IDisposable implementieren
      {
        byte[] emailBytes = Encoding.UTF8.GetBytes(normalizedEmail);
        byte[] hashBytes = md5.ComputeHash(emailBytes);
        StringBuilder sb = new StringBuilder();

        foreach (byte b in hashBytes) sb.Append(b.ToString("x2"));
        string hash = sb.ToString();

        // URL zusammenbauen, d=mp bedeutet default image
        return $"https://www.gravatar.com/avatar/{hash}?d=mp";

      }
    }

    public async Task<Bitmap?> LoadAvatarAsync(Message msg)
    {
      try
      {
        //if (string.IsNullOrEmpty(msg.SenderEmail)) return;

        // Gravatar Url bauen
        string url = GetGravatarUrl(msg.SenderEmail);

        // Bytes herunterladen
        var bytes = await _httpClient.GetByteArrayAsync(url);

        // image aus Bytes bauen
        using var ms = new MemoryStream(bytes);
        var bmp = new Bitmap(ms);

        // Property of Message Objekt setzen
        msg.AvatarBitmap = bmp;
        // return Type for the
        return bmp;
        
      }
      catch (Exception ex)
      {
        //Basisverzeichnis
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        // Pfad zur Bilddatei.
        string imagePath = Path.Combine(baseDir, "Assets", "offlinePlaceholder.png");
        // Falls auch die Gravatar Platzhalter versagt
        msg.AvatarBitmap = new Bitmap(imagePath);

        _logger.LogError(ex, "Fehler beim Laden des Avatars für {Email}", msg.SenderEmail);

        return msg.AvatarBitmap;
      }

    }
  }

}