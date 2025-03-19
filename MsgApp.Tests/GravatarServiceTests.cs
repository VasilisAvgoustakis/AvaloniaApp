using System.Security.Cryptography;
using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Services;
using System.Text;
using MsgApp.Models;
using Avalonia.Media.Imaging;
using Avalonia;


namespace MsgApp.Tests
{
  [TestFixture]
  public class GravatarServiceTests
  {

    private GravatarService _gravatarService;
    private string _testEmail;

    [SetUp]
    public void SetUp()
    {
      var logger = NullLogger<GravatarService>.Instance;
      var httpClient = new MockHttpClientService();

      _gravatarService = new GravatarService(logger, httpClient);
      _testEmail = "test@test.com";
    }
    

    [Test]
    public void BuildGravatarUrlWithHash_ValidEMail()
    {
      string normalizedTestEmail = _testEmail.Trim().ToLowerInvariant();

      string testUrl;
      string serviceFormedUrl;

      // Act
      // Hash test email manuel
      using (var testMd5 = MD5.Create())
      {
        byte[] testEmailBytes = Encoding.UTF8.GetBytes(normalizedTestEmail);
        byte[] testHashBytes = testMd5.ComputeHash(testEmailBytes);

        StringBuilder sb = new StringBuilder();

        foreach (byte b in testHashBytes) sb.Append(b.ToString("x2"));
        
        string testHash = sb.ToString();

        testUrl = $"https://www.gravatar.com/avatar/{testHash}?d=mp";
      }

      // Hash test email mit dem Service
      serviceFormedUrl =  _gravatarService.GetGravatarUrl(_testEmail);

      // Assert
      Assert.That(serviceFormedUrl, Is.Not.Null);
      Assert.That(testUrl, Is.EqualTo(serviceFormedUrl));
    }


    [Test]
    public void TestLoadAvatarAsync_ValidUrl()
    {
      //Arrange 
      //Basisverzeichnis
      string baseDir = AppDomain.CurrentDomain.BaseDirectory;
      // Pfad zur Bilddatei.
      string imagePath = Path.Combine(baseDir, "Assets", "offlinePlaceholder.png");
      
      byte[] expectedImageBytes = File.ReadAllBytes(imagePath);

      var msg = new Message();
      msg.SenderEmail = _testEmail;

      // Act
      var _ = _gravatarService.LoadAvatarAsync(msg);

      // Assert
      if(msg.AvatarBitmap != null)
      {
        byte[] actualBytes = BitmapToByteArray(msg.AvatarBitmap);
        Assert.That(expectedImageBytes, Is.EqualTo(actualBytes));
      }
    }
    
    // Helper Methode
    public byte[] BitmapToByteArray(Bitmap bitmap)
    {
      using var memoryStream = new MemoryStream();
      // Speichert die Bitmap als PNG im MemoryStream.
      bitmap.Save(memoryStream);
      return memoryStream.ToArray();
    }
  }

}