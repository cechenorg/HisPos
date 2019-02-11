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
            
            SimpleIoc.Default.Register<ViewModelMainWindow>();
            SimpleIoc.Default.Register<ViewModelProductDetailWindow>();

        }
     
        public ViewModelMainWindow ViewModelMainWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModelMainWindow>();
            }
        }

        public ViewModelProductDetailWindow ViewModelProductDetailWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModelProductDetailWindow>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}