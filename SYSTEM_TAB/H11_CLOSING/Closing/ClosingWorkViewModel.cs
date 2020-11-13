using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    public class ClosingWorkViewModel : TabBase


    {

        private DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private int trade ;
        public int Trade
        {
            get => trade;
            set
            {
                Set(() => Trade, ref trade, value);
            }
        }
        private int coop;
        public int Coop
        {
            get => coop;
            set
            {
                Set(() => Coop, ref coop, value);
            }
        }

        private int other;
        public int Other
        {
            get => other;
            set
            {
                Set(() => Other, ref other, value);
            }
        }
        private int self;
        public int Self
        {
            get => self;
            set
            {
                Set(() => Self, ref self, value);
            }
        }
        private int count;
        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand ReloadCommand { get; set; }
        #endregion



        public ClosingWorkViewModel()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ReloadAction();
        }

        private void ReloadAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("sDate", StartDate));
            parameters.Add(new SqlParameter("eDate", StartDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[ClosingWorkByDate]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            Trade = (int)result.Rows[0]["trade"];
            Coop= (int)result.Rows[0]["coop"];
            Self= (int)result.Rows[0]["selff"];
            Other = (int)result.Rows[0]["other"];
            Count= (int)result.Rows[0]["count"];

        }
    }
}
