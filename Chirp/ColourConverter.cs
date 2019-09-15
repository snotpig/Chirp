using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Chirp
{
	public class ColourConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var show = (value as Show);
			if (show != null && show.UseDate && show.Date == null)
				return new SolidColorBrush(Color.FromRgb(255, 160, 160));
			else
				return new SolidColorBrush(Color.FromRgb(255, 255, 255));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
		}
	}
}
