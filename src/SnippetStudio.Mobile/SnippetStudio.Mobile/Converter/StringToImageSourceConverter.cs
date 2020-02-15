using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SnippetStudio.Mobile.Converter
{
	public class StringToImageSourceConverter : IValueConverter
	{
		public static ConcurrentDictionary<string, StreamImageSource> _cache = new ConcurrentDictionary<string, StreamImageSource>();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var p = (string)parameter;

			if (_cache.TryGetValue(p, out var val))
			{
				return val;
			}

			var scale = 1.0f;
			var text = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"48\" height=\"48\" viewBox=\"0 0 24 24\"><path d=\"" + p + "\" fill=\"#ffffff\"/><path d=\"M0 0h24v24H0z\" fill=\"none\"/></svg>";
			var data = Encoding.UTF8.GetBytes(text);
			using (var str = new MemoryStream(data))
			{
				var svg = new SkiaSharp.Extended.Svg.SKSvg();
				var pict = svg.Load(str);
				var dimen = new SKSizeI(
					(int)(Math.Ceiling(pict.CullRect.Width) * scale),
					(int)(Math.Ceiling(pict.CullRect.Height) * scale)
				);

				var matrix = SKMatrix.MakeScale(scale, scale);
				var img = SKImage.FromPicture(pict, dimen, matrix);

				// convert to PNG
				var skdata = img.Encode(SKEncodedImageFormat.Png, 75);
				var source = new StreamImageSource()
				{
					Stream = t => Task.FromResult(skdata.AsStream())
				};

				_cache.TryAdd(p, source);

				return source;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
