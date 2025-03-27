using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Services;
using MsgApp.Models;

namespace MsgApp.Tests
{
 [TestFixture]
public class MessageStateServiceTests
{
    private MessageStateService _service;

    [SetUp]
    public void Setup()
    {
        _service = new MessageStateService(NullLogger<MessageStateService>.Instance);
    }

    [Test]
    public async Task MarkAsReadAfterDelay_ReturnsTrue_WhenNotCancelled_AndSelectedMessageIsSame()
    {
        // Arrange
        var msg = new Message { IsRead = false };
        var selected = msg; // same reference
        var cts = new CancellationTokenSource();

        // Act
        bool result = await _service.MarkAsReadAfterDelay(msg, selected, cts.Token);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task MarkAsReadAfterDelay_ReturnsFalse_WhenCancelled()
    {
        // Arrange
        var msg = new Message { IsRead = false };
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act
        bool result = await _service.MarkAsReadAfterDelay(msg, msg, cts.Token);

        // Assert
        Assert.That(result, Is.False, "Should return false if token was canceled.");
    }

    [Test]
    public async Task MarkAsReadAfterDelay_ReturnsFalse_WhenSelectedMessageDiffers()
    {
        // Arrange
        var msg = new Message { IsRead = false };
        var otherMsg = new Message(); // different reference
        var cts = new CancellationTokenSource();

        // Act
        bool result = await _service.MarkAsReadAfterDelay(msg, otherMsg, cts.Token);

        // Assert
        Assert.That(result, Is.False, "Should return false if selectedMessage != msg.");
    }
}

}