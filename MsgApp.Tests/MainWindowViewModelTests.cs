using System.Collections.ObjectModel;
using MsgApp.Models;
using MsgApp.Services;
using MsgApp.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;

namespace MsgApp.Tests
{
    [TestFixture]
    public class MainWindowViewModelTests
    {
      private MainWindowViewModel _viewModel;
      private MockTimer _fakeTimer;

        [SetUp]
        public void Setup()
        {
            _fakeTimer = new MockTimer();
            var httpClientService = new MockHttpClientService();
            var messageStateService = new MessageStateService(NullLogger<MessageStateService>.Instance, _fakeTimer);
            var messageLoader = new JsonMessageLoader(NullLogger<JsonMessageLoader>.Instance);
            var gravatarService = new GravatarService(NullLogger<GravatarService>.Instance, httpClientService);
            _viewModel = new MainWindowViewModel(messageLoader, NullLogger<MainWindowViewModel>.Instance, messageStateService, gravatarService);
        }

        [Test]
        public void TestSortBySender_SortierungNachSenderName()
        {
            // Arrange: unsortierte Nachrichtenliste
            _viewModel.Messages = new ObservableCollection<Message>
            {
                new Message { SenderName = "Zoe" },
                new Message { SenderName = "Anna" },
                new Message { SenderName = "Mike" }
            };

            // Act
            _viewModel.SortBySender();

            // Assert
            var sorted = _viewModel.Messages.ToList();
            Assert.That(sorted[0].SenderName, Is.EqualTo("Anna"));
            Assert.That(sorted[1].SenderName, Is.EqualTo("Mike"));
            Assert.That(sorted[2].SenderName, Is.EqualTo("Zoe"));
        }

        [Test]
        public void TestSortByDate_SortierungNachDatum()
        {
            // Arrange: unsortierte Nachrichten Datums
            _viewModel.Messages = new ObservableCollection<Message>
            {
                new Message { SentDate = new DateTime(2025, 3, 2) },
                new Message { SentDate = new DateTime(2025, 3, 1) },
                new Message { SentDate = new DateTime(2025, 3, 3) }
            };

            // Act
            _viewModel.SortByDate();

            // Assert: Überprüfe die chronologische Sortierung
            var sorted = _viewModel.Messages.ToList();
            Assert.That(sorted[0].SentDate, Is.EqualTo(new DateTime(2025, 3, 3)));
            Assert.That(sorted[1].SentDate, Is.EqualTo(new DateTime(2025, 3, 2)));
            Assert.That(sorted[2].SentDate, Is.EqualTo(new DateTime(2025, 3, 1)));
        }

        [Test]
        public void TestSelectedMessage_StartetMarkAsReadTimer()
        {
            // Arrange: Erstelle eine Testnachricht
            var msg = new Message { SenderName = "Test", IsRead = false };
            _viewModel.Messages = new ObservableCollection<Message> { msg };

            // Act: Setze SelectedMessage
            _viewModel.SelectedMessage = msg;

            // Assert: Überprüfe, dass unser FakeTimerService (als Teil des MessageStateService)
            // den DelayAsync-Aufruf registriert hat.
            Assert.That(_fakeTimer.CallCount, Is.EqualTo(2)); // 2 weil im Konstruktor von MainWindowViewModel schon einmal der erste Msg als selected gesetzt wird
        }
    }
}
