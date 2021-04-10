using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SnippetStudio.Wpf.Converter
{
	public class HighlightingDefinitionConverter : IValueConverter
	{
		private static readonly HighlightingDefinitionTypeConverter _converter = new HighlightingDefinitionTypeConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return _converter.ConvertFrom(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return _converter.ConvertToString(value);
		}
	}
}
