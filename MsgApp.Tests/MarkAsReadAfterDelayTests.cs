using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using MsgApp.Models;
using MsgApp.Services;
using MsgApp.ViewModels;
using System.Threading;

namespace MsgApp.Tests
{
  [TestFixture]
  class MarkAsReadAfterDelayTests
  {
    [Test]
    public void MarkAsReadAfterDelay_SetsIsReadImmediately()
    {
      // Arrange
      var messageLoaderNullLogger = NullLogger<JsonMessageLoader>.Instance;
      var gravatarService = new GravatarService();
      var httpClientService = new HttpClientService();
      var messageLoader = new JsonMessageLoader(messageLoaderNullLogger, gravatarService, httpClientService);

      var viewModelNullLogger = NullLogger<MainWindowViewModel>.Instance;
      var fakeTimer = new MockTimer();
      var viewModel = new MainWindowViewModel(messageLoader, viewModelNullLogger, fakeTimer);

      var messages = viewModel.Messages;

      // Act
      if (messages != null)
      {
        foreach (var msg in messages)
        {
          // Nachrichten nacheinander selektieren
          viewModel.SelectedMessage = msg; 
          
          // Kontrolierre ob IsRead was set to true
          Assert.That(msg.IsRead, Is.True);
        }
      }
      
    }

  }
}