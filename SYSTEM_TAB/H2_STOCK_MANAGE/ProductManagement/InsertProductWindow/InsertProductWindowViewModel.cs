using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.InsertProductWindow {
    public class InsertProductWindowViewModel : ViewModelBase{
        #region Var
        private string proTypeName;
        public string ProTypeName
        {
            get => proTypeName;
            set
            {
                Set(() => ProTypeName, ref proTypeName, value);
                switch (ProTypeName) {
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
        #endregion
        public InsertProductWindowViewModel() {
            InsertProductCommand = new RelayCommand(InsertProductAction); 
        }
        #region Action
        private void InsertProductAction() {
            string typeID = "";
            switch (ProTypeName)
            {
                case "健保品":
                    typeID = "1";
                    break;
                case "非健保品":
                    typeID = "2";
                    break;
            }
            if (!string.IsNullOrEmpty(typeID))
            {
                ProductDB.InsertProduct(typeID, ProID, ProChineseName, ProEnglishName);
                MessageWindow.ShowMessage("新增成功", Class.MessageType.SUCCESS);
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseInsertProductWindow"));
            }
            else {
                MessageWindow.ShowMessage("新增失敗", Class.MessageType.ERROR);
            }
        }
        #endregion
    }
}
