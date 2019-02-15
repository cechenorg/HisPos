using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.PurchaseReturn;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.ChooseBatchWindow
{
    public class ChooseBatchWindowViewModel
    {
        #region ----- Define Commands -----
        public RelayCommand ConfirmReturnAmountCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string productID { get; }
        public ChooseBatchProducts ChooseBatchProductCollection { get; set; }
        #endregion

        public ChooseBatchWindowViewModel(string iD)
        {
            productID = iD;

            RegisterCommand();
            ChooseBatchProductCollection = ChooseBatchProducts.GetChooseBatchProductsByID(productID);

            if (ChooseBatchProductCollection.Count == 1)
                ConfirmReturnAmountAction();
        }

        #region ----- Define Actions -----
        private void ConfirmReturnAmountAction()
        {
            Messenger.Default.Send(new NotificationMessage<ChooseBatchProducts>(this, ChooseBatchProductCollection, productID));
            Messenger.Default.Send(new NotificationMessage(this, "CloseWindow"));
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            ConfirmReturnAmountCommand = new RelayCommand(ConfirmReturnAmountAction);
        }
        #endregion
    }
}
