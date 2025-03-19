using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Models;
using MsgApp.Services;
using MsgApp.ViewModels;
using System.Threading;

namespace MsgApp.Tests
{
  [TestFixture]
  class MessageStateServiceTests
  {

    private MainWindowViewModel _viewModel;
    private MockTimer _fakeTimer;

    [SetUp]
    public void Setup()
    {
        var messageLoaderLogger = NullLogger<JsonMessageLoader>.Instance;
        var viewModelLogger = NullLogger<MainWindowViewModel>.Instance;
        var gravatarServiceLogger = NullLogger<GravatarService>.Instance;
        var messageStateServiceLogger = NullLogger<MessageStateService>.Instance;
        
        var httpClientService = new HttpClientService();
        _fakeTimer = new MockTimer();
        var messageStateService = new MessageStateService(messageStateServiceLogger, _fakeTimer);
        var gravatarService = new GravatarService(gravatarServiceLogger, httpClientService);
        var messageLoader = new JsonMessageLoader(messageLoaderLogger);

        _viewModel = new MainWindowViewModel(messageLoader, viewModelLogger, messageStateService, gravatarService);
    }

    [Test]
    public void MarkAsReadAfterDelay_SetsIsReadImmediately()
    {
      var messages = _viewModel.Messages;
      Assert.That(messages, Is.Not.Null, "Messages should not be null");

      // Act
      if (messages != null)
      {
        foreach (var msg in messages)
        {
          // Nachrichten nacheinander selektieren
          _viewModel.SelectedMessage = msg; 
          
          // Kontrolierre ob IsRead was set to true
          Assert.That(msg.IsRead, Is.True, $"Message from {msg.SenderName} should be marked as read");
        }
      }
      
    }

  }
}