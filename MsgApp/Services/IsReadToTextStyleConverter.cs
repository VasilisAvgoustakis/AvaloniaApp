using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace MsgApp.Services
{
    public class IsReadToTextStyleConverter : IValueConverter
    {
        // Parameter: "Title" oder "Sender".
        // - Wenn Zieltyp IBrush erwartet wird: Für ungelesen:
        //   - "Subject": Blau, "Sender": Schwarz; für gelesene Nachrichten: Standard (z. B. Schwarz).
        // - Wenn als Zieltyp FontWeight erwartet wird: Für ungelesen: Bold, für gelesen: Normal.
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            // value ist der isRead-Zustand
            if (value is bool isRead)
            {
                // Parameter als string
                string param = parameter as string ?? string.Empty;

                // Wenn der Zieltyp ein Brush ist:
                if (targetType == typeof(IBrush))
                {
                    if (!isRead)
                    {
                        // Ungelesen
                        if (param.Equals("Title", StringComparison.OrdinalIgnoreCase))
                            return Brushes.Blue;
                        else if (param.Equals("Sender", StringComparison.OrdinalIgnoreCase))
                            return Brushes.Black;
                        else
                            return Brushes.Black;
                    }
                    else
                    {
                        // Gelesen: beide in Schwarz, oder du passt es an
                        return Brushes.Black;
                    }
                }
                // Wenn der Zieltyp FontWeight ist:
                else if (targetType == typeof(FontWeight))
                {
                    return !isRead ? FontWeight.Bold : FontWeight.Normal;
                }
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
            => throw new NotImplementedException();
    }
}
