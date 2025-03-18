using NUnit.Framework;
using System.Security.Cryptography;
using MsgApp.Services;
using System.Text;


namespace MsgApp.Tests
{
  [TestFixture]
  public class GravatarServiceTests
  {
    [Test]
    public void BuildGravatarUrlWithHash_ValidEMail()
    {
      // Arrange
      var gravatarService = new GravatarService();

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
      serviceFormedUrl =  gravatarService.GetGravatarUrl(testEmail);

      Assert.That(serviceFormedUrl, Is.Not.Null);
      Assert.That(testUrl, Is.EqualTo(serviceFormedUrl));
    }






  }

}