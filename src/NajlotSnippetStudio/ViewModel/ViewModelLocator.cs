
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace NajlotSnippetStudio.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			if (ViewModelBase.IsInDesignModeStatic)
			{

			}

			SimpleIoc.Default.Register<MainWindow>();
        }

		public MainWindow MainWindow
		{
			get
			{
				// DataContext="{Binding Source={StaticResource Locator}, Path=MainWindow}"
				return ServiceLocator.Current.GetInstance<MainWindow>();
			}
		}
		
        public static void Cleanup()
        {
            // Clear the ViewModels
        }
    }
}