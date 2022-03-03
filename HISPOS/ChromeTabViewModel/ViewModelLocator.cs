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
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.CustomerManage;

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
            SimpleIoc.Default.Register<ProductPurchaseReturnViewModel>();
            SimpleIoc.Default.Register<ProductPurchaseRecordViewModel>();
            SimpleIoc.Default.Register<ViewModelOfflineOperationWindow>();
            SimpleIoc.Default.Register<CustomerManageViewModel>();
        }

        public ViewModelMainWindow ViewModelMainWindow => ServiceLocator.Current.GetInstance<ViewModelMainWindow>();
        public ViewModelProductDetailWindow ViewModelProductDetailWindow => ServiceLocator.Current.GetInstance<ViewModelProductDetailWindow>();
        public ProductPurchaseReturnViewModel ProductPurchaseReturn => ServiceLocator.Current.GetInstance<ProductPurchaseReturnViewModel>();
        public ProductPurchaseRecordViewModel ProductPurchaseRecord => ServiceLocator.Current.GetInstance<ProductPurchaseRecordViewModel>();

        public ViewModelOfflineOperationWindow ViewModelOfflineOperationWindow => ServiceLocator.Current.GetInstance<ViewModelOfflineOperationWindow>();
        public CustomerManageViewModel CustomerManageView => ServiceLocator.Current.GetInstance<CustomerManageViewModel>();

        public static void Cleanup()
        {
        }
    }
}