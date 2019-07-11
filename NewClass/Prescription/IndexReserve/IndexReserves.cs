using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
   public class IndexReserves : ObservableCollection<IndexReserve>
    {
        public IndexReserves() { }

        public void GetDataByDate(DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = IndexReserveDb.GetDataByDate( sDate, eDate);
            foreach (DataRow r in table.Rows) {
                IndexReserve indexReserve = new IndexReserve(r);
                Add(indexReserve);
            }
        }
        public void StoreOrderToSingde() {
            int count = StoreOrderDB.GetStoOrdMasterCountByDate().Rows[0].Field<int>("Count");
            for (int i = 0; i < this.Count; i++) {
                count++;
                string newStoOrdID = "P" + DateTime.Today.ToString("yyyyMMdd") + "-" + count.ToString().PadLeft(2, '0');
                this[i].StoOrdID = newStoOrdID;
                for (int j = 0; j < this[i].IndexReserveDetailCollection.Count;j++) {
                    this[i].IndexReserveDetailCollection[j].StoOrdID = newStoOrdID;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            MainWindow.SingdeConnection.OpenConnection();
            StoreOrderDB.InsertIndexReserveOrder(this);
            foreach (var i in this) {
                if (StoreOrderDB.SendStoreOrderToSingde(i).Rows[0][0].ToString() == "SUCCESS")
                {
                    StoreOrderDB.StoreOrderToWaiting(i.StoOrdID);
                    i.IsSend = true;
                    i.SaveStatus();
                }
                else
                    MessageWindow.ShowMessage(i.StoOrdID + "傳送失敗",Class.MessageType.ERROR);
            }
            MainWindow.ServerConnection.CloseConnection();
            MainWindow.SingdeConnection.CloseConnection();
        }
    }
}
