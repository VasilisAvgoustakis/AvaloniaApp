using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Models;

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
            // Und gibst sie in den Konstruktor:
            var loader = new JsonMessageLoader(nullLogger);

            var path = "../MsgApp/Data/sample-messages.json"; // Pfad zu deiner JSON-Datei

            // Act
            var messages = loader.LoadMessagesFromJson(path);

            // Assert
            Assert.IsNotNull(messages);
            Assert.IsNotEmpty(messages);
        }

        [Test]
        public void LoadMessagesFromJson_InvalidFile_ReturnsEmptyList()
        {
            var nullLogger = NullLogger<JsonMessageLoader>.Instance;
            var loader = new JsonMessageLoader(nullLogger);

            var path = "Data/nichtVorhanden.json";

            var messages = loader.LoadMessagesFromJson(path);

            Assert.IsNotNull(messages);
            Assert.IsEmpty(messages);
        }
    }
}
