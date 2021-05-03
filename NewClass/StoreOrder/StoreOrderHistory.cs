using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Interface;
using His_Pos.NewClass.Product.PurchaseReturn;
using System;
using System.Data;
using System.Windows;

namespace His_Pos.NewClass.StoreOrder
{
    public  class StoreOrderHistory : ObservableObject
    {

        public StoreOrderHistory()
        {
        }

        public StoreOrderHistory(DataRow row)
        {
            ID = row.Field<string>("ID");
            CreateDateTime = row.Field<DateTime>("CreateDateTime");
            DoneDateTime = (DateTime)row.Field<DateTime?>("DoneDateTime");
        }
        


        private string iD;
        public string ID
        {
            get { return iD; }
            set { Set(() => ID, ref iD, value); }
        }



        private DateTime createDateTime;
        public DateTime CreateDateTime
        {
            get { return createDateTime; }
            set { Set(() => CreateDateTime, ref createDateTime ,value); }
        }

        private DateTime doneDateTime;
        public DateTime DoneDateTime
        {
            get { return doneDateTime; }
            set { Set(() => DoneDateTime, ref doneDateTime, value); }
        }
    }
}