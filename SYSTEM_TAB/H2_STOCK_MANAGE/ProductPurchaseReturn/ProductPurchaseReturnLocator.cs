using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnLocator
    {
        public ProductPurchaseReturnLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<ProductPurchaseReturnViewModel>();
        }
        
        public ProductPurchaseReturnViewModel ProductPurchaseReturn{ get { return ServiceLocator.Current.GetInstance<ProductPurchaseReturnViewModel>(); } }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
