using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.InsertProductWindow
{
    public class InsertProductWindowViewModel : ViewModelBase
    {
        #region Var

        private string proTypeName;

        public string ProTypeName
        {
            get => proTypeName;
            set
            {
                Set(() => ProTypeName, ref proTypeName, value);
                switch (ProTypeName)
                {
                    case "OTC藥品":
                        ProID = ProductDB.GetProductNewID(2);
                        break;
                }
            }
        }

        private string proID;

        public string ProID
        {
            get => proID;
            set
            {
                Set(() => ProID, ref proID, value);
            }
        }

        private string proChineseName;

        public string ProChineseName
        {
            get => proChineseName;
            set
            {
                Set(() => ProChineseName, ref proChineseName, value);
            }
        }

        private string proEnglishName;

        public string ProEnglishName
        {
            get => proEnglishName;
            set
            {
                Set(() => ProEnglishName, ref proEnglishName, value);
            }
        }

        public RelayCommand InsertProductCommand { get; set; }

        #endregion Var

        public InsertProductWindowViewModel()
        {
            InsertProductCommand = new RelayCommand(InsertProductAction);
        }

        #region Action

        private void InsertProductAction()
        {
            string typeID = "";
            switch (ProTypeName)
            {
                case "健保品":
                    typeID = "1";
                    break;

                case "門市商品":
                    typeID = "2";
                    break;
            }
            if (!string.IsNullOrEmpty(typeID))
            {
                DataTable dataTable = ProductDB.InsertProduct(typeID, ProID, ProChineseName, ProEnglishName);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows[0].Field<string>("RESULT") == "SUCCESS")
                    {
                        MessageWindow.ShowMessage("新增成功", Class.MessageType.SUCCESS);
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseInsertProductWindow"));
                        ProductDetailWindow.ShowProductDetailWindow();
                        Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { proID, "0" }, "ShowProductDetail"));
                    }
                    else if (dataTable.Rows[0].Field<string>("RESULT") == "EXISTED")
                        MessageWindow.ShowMessage($"商品條碼{ProID}已存在，新增失敗", Class.MessageType.ERROR);
                    else
                        MessageWindow.ShowMessage("OUT", Class.MessageType.SUCCESS);
                }
                else
                    MessageWindow.ShowMessage("OUTSIDE", Class.MessageType.SUCCESS);
            }
            else
                MessageWindow.ShowMessage("新增失敗", Class.MessageType.ERROR);
        }

        #endregion Action
    }
}