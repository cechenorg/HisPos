using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.BalanceSheet
{
    public class StrikeData : ObservableObject
    {
        #region ----- Define Variables -----
        private string selectedType;

        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                RaisePropertyChanged(nameof(SelectedType));
            }
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public StrikeTypeEnum Type { get{ return SelectedType.Equals("銀行")? StrikeTypeEnum.Bank : StrikeTypeEnum.Cash;} }
        public double Value { get; set; }
        public string StrikeValue { get; set; }

        public string[] StrikeTypes { get; } = {"銀行", "現金"};
        #endregion

        public StrikeData(DataRow row)
        {
            ID = row.Field<string>("ID");
            Name = row.Field<string>("HEADER");
            Value = (double)row.Field<decimal>("VALUE");

            SelectedType = StrikeTypes[0];
        }
    }
}
