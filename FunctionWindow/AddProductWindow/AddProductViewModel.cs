using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    public class AddProductViewModel : ViewModelBase
    {
        #region ----- Define Command -----
        public RelayCommand GetRelatedDataCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public bool HideDisableProduct { get; set; }
        public bool ShowOnlyThisManufactory { get; set; }
        public string SearchString { get; set; }
        public ProductStruct SelectedProductStruct { get; set; }
        public ProductStructs ProductStructCollection { get; set; }
        #endregion

        public AddProductViewModel()
        {
            GetRelatedDataCommand = new RelayCommand(GetRelatedDataAction);
            
            InitCollection();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "AddProduct")
                {
                    FilterCommand = new RelayCommand(ProductPurchaseFilterAction);
                }
            });
        }

        #region ----- Define Actions -----
        private void GetRelatedDataAction()
        {

        }
        private void ProductPurchaseFilterAction()
        {

        }
        #endregion

        #region ----- Define Functions -----
        private void InitCollection()
        {
            ProductStructCollection = ProductStructs.GetProductStructs();
        }
        #endregion

    }
}
