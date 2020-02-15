using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace SnippetStudio.Mobile.Converter
{
	public class EnumToTranslationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			var resourceManager = new System.Resources.ResourceManager(parameter as Type);

			if (value.GetType().IsConstructedGenericType)
			{
				var collection = value as ICollection;
				var newCollection = new List<object>();

				foreach (var item in collection)
				{
					newCollection.Add(Translate((Enum)item, resourceManager));
				}

				return newCollection;
			}

			return Translate((Enum)value, resourceManager);
		}

		private string Translate(Enum value, System.Resources.ResourceManager resourceManager)
		{
			return resourceManager.GetString(value.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			var resourceManager = new System.Resources.ResourceManager(parameter as Type);

			if (targetType.IsConstructedGenericType)
			{
				var collection = value as ICollection;
				var newCollection = new List<object>();

				foreach (var item in collection)
				{
					newCollection.Add(TranslateBack(item.ToString(), resourceManager, culture, targetType));
				}

				return newCollection;
			}

			return TranslateBack(value.ToString(), resourceManager, culture, targetType);
		}

		private object TranslateBack(string value, System.Resources.ResourceManager resourceManager, CultureInfo culture, Type targetType)
		{
			var resourceSet = resourceManager.GetResourceSet(culture, true, true);

			foreach (DictionaryEntry entry in resourceSet)
			{
				if (entry.Value.ToString() == value)
				{
					return Enum.Parse(targetType, entry.Key.ToString());
				}
			}

			return value;
		}
	}
}
