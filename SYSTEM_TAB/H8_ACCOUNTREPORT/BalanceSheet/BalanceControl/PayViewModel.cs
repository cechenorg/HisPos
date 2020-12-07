using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.BalanceSheet;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Report.CashReport;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    public class PayViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand<RelayCommand> StrikeCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private StrikeDatas strikeDatas;
        private StrikeData selectedData;
        private Institution target;
        private ICollectionView strikeDataCollectionView;

        public Institution Target
        {
            get { return target; }
            set
            {
                target = value;
                RaisePropertyChanged(nameof(Target));

                FilterData();
            }
        }
        public StrikeDatas StrikeDatas
        {
            get { return strikeDatas; }
            set
            {
                strikeDatas = value;
                RaisePropertyChanged(nameof(StrikeDatas));
            }
        }
        public StrikeData SelectedData
        {
            get { return selectedData; }
            set
            {
                selectedData = value;
                RaisePropertyChanged(nameof(SelectedData));
            }
        }
        public ICollectionView StrikeDataCollectionView
        {
            get => strikeDataCollectionView;
            set
            {
                Set(() => StrikeDataCollectionView, ref strikeDataCollectionView, value);
            }
        }
        #endregion

        public PayViewModel()
        {
            StrikeCommand = new RelayCommand<RelayCommand>(StrikeAction);
        }

        #region ----- Define Actions -----
        private void StrikeAction(RelayCommand command)
        {
            if (!StrikeValueIsValid()) return;
            if (SelectedData.SelectedType.ID == null) { MessageWindow.ShowMessage("請選擇正確的沖帳對象", MessageType.ERROR); return; }
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = CashReportDb.StrikeBalanceSheet(SelectedData.SelectedType.ID, BalanceSheetTypeEnum.Pay, Double.Parse(SelectedData.StrikeValue), SelectedData.Name, SelectedData.ID);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("沖帳成功", MessageType.SUCCESS);
            }
            else
            {
                MessageWindow.ShowMessage("沖帳失敗", MessageType.ERROR);
            }

            command.Execute(null);
        }
        #endregion

        #region ----- Define Functions -----
        private bool StrikeValueIsValid()
        {
            double temp;
            if (double.TryParse(SelectedData.StrikeValue, out temp))
            {
                if (temp <= 0)
                {
                    MessageWindow.ShowMessage("不可小於等於0!", MessageType.ERROR);
                    return false;
                }

                if (temp > SelectedData.Value)
                {
                    MessageWindow.ShowMessage("不可大於原金額!", MessageType.ERROR);
                    return false;
                }
            }
            else
            {
                MessageWindow.ShowMessage("輸入金額非數字", MessageType.ERROR);
                return false;
            }

            return true;
        }
        private void FilterData()
        {
            StrikeDataCollectionView = CollectionViewSource.GetDefaultView(StrikeDatas);
            StrikeDataCollectionView.Filter += DataFilter;

            RaisePropertyChanged(nameof(StrikeDataCollectionView));
        }
        private bool DataFilter(object data)
        {
            StrikeData d = data as StrikeData;

            return d.ID.Equals(Target.ID);
        }
        #endregion
    }
}
