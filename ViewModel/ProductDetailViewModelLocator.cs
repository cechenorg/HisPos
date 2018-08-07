using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace His_Pos.ViewModel
{
    public class ProductDetailViewModelLocator
    {
        public ProductDetailViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Create run time view services and models
            SimpleIoc.Default.Register<IViewModelProdcutDetailWindow, ViewModelProductDetailWindow>();

        }

        public IViewModelProdcutDetailWindow ViewModelProductDetailWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IViewModelProdcutDetailWindow>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}

