using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Avalonia.Media;


namespace MsgApp.Services
{
  public class BoolToBrushConverter : IValueConverter
  {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
      if (value is bool isRead)
      {
        return isRead ? Brushes.Transparent : Brushes.Yellow;
      }
      // Fallback
      return Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
      throw new NotImplementedException();
    }
  }
}