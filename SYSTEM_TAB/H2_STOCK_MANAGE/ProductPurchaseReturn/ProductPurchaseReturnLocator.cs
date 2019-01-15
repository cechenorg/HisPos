using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class ProductPurchaseReturnLocator
    {
        public ProductPurchaseReturnLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<ProductPurchaseReturnViewModel>();
            Console.WriteLine("GOGO");
        }
        
        public ProductPurchaseReturnViewModel ProductPurchaseReturn{ get { return ServiceLocator.Current.GetInstance<ProductPurchaseReturnViewModel>(); } }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
