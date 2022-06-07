using GalaSoft.MvvmLight;
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
            DoneDateTime = row.Field<string>("DoneDateString");
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

        private string doneDateTime;
        public string DoneDateTime
        {
            get { return doneDateTime; }
            set { Set(() => DoneDateTime, ref doneDateTime, value); }
        }
    }
}