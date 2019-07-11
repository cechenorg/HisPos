﻿using His_Pos.NewClass.StoreOrder;
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
            string newstoordId = StoreOrderDB.InsertPrescriptionOrder(pSendData, p).Rows[0].Field<string>("newStoordId");
            try
            {
                if (PrescriptionDb.SendDeclareOrderToSingde(newstoordId, p, pSendData))
                {
                    StoreOrderDB.StoreOrderToWaiting(newstoordId);
                    return true;
                }
                StoreOrderDB.RemoveStoreOrderByID(newstoordId);
                MessageWindow.ShowMessage("傳送藥健康失敗 請稍後再帶出處方傳送", MessageType.ERROR);
                return false;
            }
            catch (Exception ex)
            {
                StoreOrderDB.RemoveStoreOrderByID(newstoordId);
                MessageWindow.ShowMessage("傳送藥健康失敗 請稍後再帶出處方傳送", MessageType.ERROR);
                return false;
            } 
        }
    }
}
