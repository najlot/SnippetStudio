using System;
using System.Windows.Interactivity;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace NajlotSnippetStudio
{
	public sealed class TextEditorBehaviour : Behavior<TextEditor>
	{
		public static readonly DependencyProperty BindedTextProperty =
			DependencyProperty.Register(nameof(BindedText), typeof(string), typeof(TextEditorBehaviour),
			new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

		public string BindedText
		{
			get { return (string) GetValue(BindedTextProperty); }
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
			var textEditor = sender as TextEditor;
			if (textEditor != null)
			{
				if (textEditor.Document != null)
				{
					BindedText = textEditor.Document.Text;
				}
			}
		}

		private static void PropertyChangedCallback(
			DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var behavior = dependencyObject as TextEditorBehaviour;
			if (behavior.AssociatedObject != null)
			{
				var editor = behavior.AssociatedObject as TextEditor;
				if (editor.Document != null)
				{
					var caretOffset = editor.CaretOffset;
					var text = dependencyPropertyChangedEventArgs.NewValue?.ToString();
					editor.Document.Text = text == null ? "": text;
					editor.CaretOffset = caretOffset;
				}
			}
		}
	}
}
