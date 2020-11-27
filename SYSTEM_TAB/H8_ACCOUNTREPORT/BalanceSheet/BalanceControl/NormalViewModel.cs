using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class NormalViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand InsertCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string transferValue;
        private string target;
        public string IDClone;
        public double MaxValue { get; set; } = 0;
        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                RaisePropertyChanged(nameof(Target));
            }
        }
        public string TransferValue
        {
            get { return transferValue; }
            set
            {
                transferValue = value;
                RaisePropertyChanged(nameof(TransferValue));
            }
        }
        #endregion
        public NormalViewModel(string ID)
        {
            IDClone = ID;
            Init();
            InsertCommand = new RelayCommand(InsertAction);
        }
        public NormalViewModel()
        {
            InsertCommand = new RelayCommand(InsertAction);
        }
        public void Init()
        {
           
        }
        public void InsertAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", IDClone));
            parameters.Add(new SqlParameter("NAME", TransferValue));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertAccounts]", parameters);
            MainWindow.ServerConnection.CloseConnection();
        }


    }
}
