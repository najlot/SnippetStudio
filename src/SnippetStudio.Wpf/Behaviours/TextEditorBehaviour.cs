using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace SnippetStudio.Wpf.Behaviours
{
	public sealed class TextEditorBehaviour : Behavior<TextEditor>
	{
		public static readonly DependencyProperty BindedTextProperty =
			DependencyProperty.Register(nameof(BindedText), typeof(string), typeof(TextEditorBehaviour),
			new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

		public string BindedText
		{
			get { return (string)GetValue(BindedTextProperty); }
			set { SetValue(BindedTextProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			if (AssociatedObject != null)
			{
				AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
			}
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			if (AssociatedObject != null)
			{
				AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
			}
		}

		private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
		{
			if (sender is TextEditor textEditor)
			{
				if (textEditor.Document != null)
				{
					if (BindedText != textEditor.Document.Text)
					{
						BindedText = textEditor.Document.Text;
					}
				}
			}
		}

		private static void PropertyChangedCallback(
			DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var behavior = dependencyObject as TextEditorBehaviour;
			if (behavior?.AssociatedObject != null)
			{
				var editor = behavior.AssociatedObject;
				if (editor.Document != null)
				{
					var caretOffset = editor.CaretOffset;
					var text = dependencyPropertyChangedEventArgs.NewValue?.ToString() ?? "";

					if (editor.Document.Text != text)
					{
						editor.Document.Text = text;
					}
					
					try 
					{
						editor.CaretOffset = caretOffset;
					}
					catch
					{
						// its okay... 
					} 
				}
			}
		}
	}
}
