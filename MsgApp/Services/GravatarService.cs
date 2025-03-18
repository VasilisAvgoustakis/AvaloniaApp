using System.Security.Cryptography;
using System.Text;

namespace MsgApp.Services
{
  public class GravatarService
  {
    public string GetGravatarUrl(string email)
    {
      if (string.IsNullOrWhiteSpace(email)) return "https://www.gravatar.com/avatar/?d=mp";

      // Normalisieren: in Kleinbuchstaben und trimmen
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
  }
}