using GalaSoft.MvvmLight;
using His_Pos.NewClass.Report.Accounts;
using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.BalanceSheet
{
    public class StrikeData : ObservableObject
    {
        #region ----- Define Variables -----

        private AccountsReports selectedType;

        public AccountsReports SelectedType
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
        public StrikeTypeEnum Type { get { return SelectedType.Equals("銀行") ? StrikeTypeEnum.Bank : StrikeTypeEnum.Cash; } }
        public double Value { get; set; }
        public string StrikeValue { get; set; }

        public List<AccountsReports> StrikeTypes { get; set; } = new List<AccountsReports>();

        #endregion ----- Define Variables -----

        public StrikeData(DataRow row)
        {
            ID = row.Field<string>("ID");
            Name = row.Field<string>("HEADER");
            Value = (double)row.Field<decimal>("VALUE");
            SelectedType = new AccountsReports();
            StrikeTypes.Add(new AccountsReports("現金", 0, "001001"));
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[BankByAccountsID]");
            MainWindow.ServerConnection.CloseConnection();
            foreach (DataRow c in result.Rows)
            {
                StrikeTypes.Add(new AccountsReports(c["Name"].ToString(), 0, c["ID"].ToString()));
            }
            //SelectedType.ID = StrikeTypes[0].ID.ToString();
        }
    }
}