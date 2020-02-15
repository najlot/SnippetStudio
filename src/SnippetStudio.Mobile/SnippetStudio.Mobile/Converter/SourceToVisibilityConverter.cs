using System;
using System.Globalization;
using Xamarin.Forms;

namespace SnippetStudio.Mobile.Converter
{
	public class SourceToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.ToString() == parameter.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
