using NUnit.Framework;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Services;
using System.Text;


namespace MsgApp.Tests
{
  [TestFixture]
  public class GravatarServiceTests
  {

    private GravatarService _gravatarService;

    [SetUp]
    public void SetUp()
    {
      var logger = NullLogger<GravatarService>.Instance;
      var httpClient = new MockHttpClientService();

      _gravatarService = new GravatarService(logger, httpClient);
    }
    

    [Test]
    public void BuildGravatarUrlWithHash_ValidEMail()
    {
      string testEmail = "test@test.com";

      string normalizedTestEmail = testEmail.Trim().ToLowerInvariant();

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
      serviceFormedUrl =  _gravatarService.GetGravatarUrl(testEmail);

      // Assert
      Assert.That(serviceFormedUrl, Is.Not.Null);
      Assert.That(testUrl, Is.EqualTo(serviceFormedUrl));
    }


    [Test]
    public void TestLoadAvatarAsync()
    {
      
    }
  }

}