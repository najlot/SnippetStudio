using System;
using System.Linq;
using SnippetStudio.ClientBase.Validation;
using Xamarin.Forms;

namespace SnippetStudio.Mobile.Controls
{
	public class ValidationLabel : Label
	{
		public static BindableProperty PropertyNameProperty =
			BindableProperty.Create(nameof(PropertyName), typeof(string), typeof(ValidationLabel));

		public static BindableProperty HasErrorsProperty =
			BindableProperty.Create(nameof(HasErrors), typeof(bool), typeof(ValidationLabel));

		public string PropertyName
		{
			get => (string)GetValue(PropertyNameProperty);
			set => SetValue(PropertyNameProperty, value);
		}

		public bool HasErrors
		{
			get => (bool)GetValue(HasErrorsProperty);
			set => SetValue(HasErrorsProperty, value);
		}

		public ValidationLabel()
		{
			IsVisible = false;
			Margin = new Thickness
			{
				Bottom = 15
			};
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == nameof(HasErrors))
			{
				if (BindingContext is AbstractValidationModel model)
				{
					if (model.HasErrors)
					{
						var results = model.Errors.Where(err => err.PropertyName == PropertyName);

						var errors = results
							.Where(e => e.Severity == ValidationSeverity.Error)
							.Select(e => e.Text)
							.ToList();

						if (errors.Any())
						{
							var errorText = string.Join(Environment.NewLine, errors);
							Text = errorText;
							TextColor = Color.Red;
							IsVisible = true;
							return;
						}

						var warnings = results
							.Where(e => e.Severity == ValidationSeverity.Warning)
							.Select(w => w.Text)
							.ToList();

						if (warnings.Any())
						{
							var warningText = string.Join(Environment.NewLine, warnings);
							Text = warningText;
							TextColor = Color.Orange;
							IsVisible = true;
							return;
						}

						var infos = results
							.Where(e => e.Severity == ValidationSeverity.Info)
							.Select(i => i.Text)
							.ToList();

						if (infos.Any())
						{
							var infoText = string.Join(Environment.NewLine, infos);
							Text = infoText;
							TextColor = Color.DarkCyan;
							IsVisible = true;
							return;
						}
					}

					IsVisible = false;
				}
			}
		}
	}
}
