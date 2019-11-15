using System;
using System.Collections.ObjectModel;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.NewClass.Medicine.NotEnoughMedicine
{
    public class NotEnoughMedicines: ObservableCollection<NotEnoughMedicine>
    {
        public string StoreOrderID { get; set; }

        public NotEnoughMedicines() { }

        private void Init()
        {
            MainWindow.ServerConnection.OpenConnection();
            var count = StoreOrderDB.GetStoOrdMasterCountByDate().Rows[0].Field<int>("Count");
            var newStoOrdID = "P" + DateTime.Today.ToString("yyyyMMdd") + "-" + count.ToString().PadLeft(2, '0');
            StoreOrderID = newStoOrdID;
            MainWindow.ServerConnection.CloseConnection();
        }

        public void ToWaitingStatus(string note)
        {
            var isSuccess = SendOrderToSingde(note);
            if (isSuccess)
            {
                StoreOrderDB.StoreOrderToWaiting(StoreOrderID);
            }
            else
                MessageWindow.ShowMessage("傳送杏德失敗 請稍後至進退或管理傳送採購單", MessageType.ERROR);
        }


        private bool SendOrderToSingde(string note)
        {
            DataTable dataTable = StoreOrderDB.SendStoreOrderToSingde(this,note);
            return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }

        public void CreateOrder(string note)
        {
            var result = StoreOrderDB.InsertNotEnoughPurchaseOrder(this, note);
            if (result.Rows.Count <= 0) return;
            StoreOrderID = result.Rows[0].Field<string>("newStoordId");
            ToWaitingStatus(note);
        }
    }
}
