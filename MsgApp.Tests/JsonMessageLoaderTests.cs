using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Models;
using MsgApp.Services;

namespace MsgApp.Tests
{
    [TestFixture]
    public class JsonMessageLoaderTests
    {
        [Test]
        public void LoadMessagesFromJson_ValidFile_ReturnsList()
        {
            // Arrange
            // Instanz des NullLogger:
            var nullLogger = NullLogger<JsonMessageLoader>.Instance;
            // Gravatar Service Instanz
            var gravatarService = new GravatarService();
            // HttpClient Service Instanz
            var httpClientService = new HttpClientService();
            // Und gibst sie in den Konstruktor:
            var loader = new JsonMessageLoader(nullLogger, gravatarService, httpClientService);

            var path = "../MsgApp/Data/sample-messages.json"; // Pfad zu JSON-Datei

            // Act
            var messages = loader.LoadMessagesFromJson(path);

            // Assert
            Assert.That(messages, Is.Not.Null);

            foreach (var message in messages)
            {
                Assert.That(message.AvatarBitmap, Is.Not.Null);
            }
        }

        [Test]
        public void LoadMessagesFromJson_InvalidFile_ReturnsEmptyList()
        {
            var nullLogger = NullLogger<JsonMessageLoader>.Instance;
            var gravatarService = new GravatarService();
            var httpClientService = new HttpClientService();

            var loader = new JsonMessageLoader(nullLogger, gravatarService, httpClientService);

            var path = "Data/nichtVorhanden.json";

            var messages = loader.LoadMessagesFromJson(path);

            Assert.That(messages, Is.Not.Null);
            Assert.That(messages, Is.Empty);
        }
    }
}
