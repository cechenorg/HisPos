using Dapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.ParamControl
{
    public class ParamControlViewModel : ViewModelBase
    {
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }

        private bool isDataChanged;
        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
                CancelChangeCommand.RaiseCanExecuteChanged();
                ConfirmChangeCommand.RaiseCanExecuteChanged();
            }
        }
        private int tradeReturnDay = ViewModelMainWindow.TradeReturnDays;
        public int TradeReturnDay
        {
            get { return tradeReturnDay; }
            set
            {
                Set(() => TradeReturnDay, ref tradeReturnDay, value);
            }
        }

        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        public ParamControlViewModel()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsDataChanged);
            DataChangedCommand = new RelayCommand(DataChangedAction);
        }
        private void ConfirmChangeAction()
        {
            string sql = string.Format(
                @"Update [{0}].[SystemInfo].[SystemParameters] Set SysPar_Value = {1} Where SysPar_Name = 'TradeReturnDays'",
                Properties.Settings.Default.SystemSerialNumber,
                TradeReturnDay);
            SQLServerConnection.DapperQuery((conn) =>
            {
                _ = conn.Query<int>(sql, commandType: CommandType.Text);
            });
            ViewModelMainWindow.TradeReturnDays = TradeReturnDay;
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            IsDataChanged = false;
        }
    }
}
