using NUnit.Framework;
using System.Globalization;
using Avalonia.Media;
using Avalonia;
using MsgApp.Services;

namespace MsgApp.Tests
{
    [TestFixture]
    public class IsReadToTextStyleConverterTests
    {
        private IsReadToTextStyleConverter _converter;
        private CultureInfo _culture;

        [SetUp]
        public void Setup()
        {
            _converter = new IsReadToTextStyleConverter();
            _culture = CultureInfo.InvariantCulture;
        }

        [Test]
        public void Convert_ReturnsBlueBrush_ForUnreadTitle()
        {
            // Arrange: Ungelesene Nachricht, Parameter "Title"
            bool isRead = false;
            string parameter = "Title";

            // Act
            var result = _converter.Convert(isRead, typeof(IBrush), parameter, _culture);

            // Assert: Erwartet wird der blaue Brush
            Assert.That(result, Is.EqualTo(Brushes.Blue));
        }

        [Test]
        public void Convert_ReturnsBlackBrush_ForReadTitle()
        {
            // Arrange: Gelesene Nachricht, Parameter "Title"
            bool isRead = true;
            string parameter = "Title";

            // Act
            var result = _converter.Convert(isRead, typeof(IBrush), parameter, _culture);

            // Assert: Erwartet wird der schwarze Brush
            Assert.That(result, Is.EqualTo(Brushes.Black));
        }

        [Test]
        public void Convert_ReturnsBold_ForUnreadFontWeight()
        {
            // Arrange: Ungelesene Nachricht, Zieltyp FontWeight
            bool isRead = false;
            string parameter = "irgendeinParameter";

            // Act
            var result = _converter.Convert(isRead, typeof(FontWeight), parameter, _culture);

            // Assert: Für ungelesene Nachrichten wird Bold erwartet
            Assert.That(result, Is.EqualTo(FontWeight.Bold));
        }

        [Test]
        public void Convert_ReturnsNormal_ForReadFontWeight()
        {
            // Arrange: Gelesene Nachricht, Zieltyp FontWeight
            bool isRead = true;
            string parameter = "irgendeinParameter";

            // Act
            var result = _converter.Convert(isRead, typeof(FontWeight), parameter, _culture);

            // Assert: Für gelesene Nachrichten wird Normal erwartet
            Assert.That(result, Is.EqualTo(FontWeight.Normal));
        }
    }
}
