/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:His_Pos"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace His_Pos.ChromeTabViewModel
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
            
                // Create run time view services and models
                SimpleIoc.Default.Register<IViewModelMainWindow, ViewModelMainWindow>();

        }
     
        public IViewModelMainWindow ViewModelMainWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IViewModelMainWindow>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}