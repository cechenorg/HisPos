using Dapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.SystemControl
{
    public class SystemControlViewModel : ViewModelBase
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
        private Dictionary<int, string> disItem;
        public Dictionary<int, string> DisItem
        {
            get => disItem;
            set => Set(() => DisItem, ref disItem, value);
        }
        private int selectItem;
        public int SelectItem
        {
            get => selectItem;
            set
            {
                Set(() => SelectItem, ref selectItem, value);
            }
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        public SystemControlViewModel()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsDataChanged);
            DataChangedCommand = new RelayCommand(DataChangedAction);
            DisItem = new Dictionary<int, string>
            {
                { 0, "先進先出" },
                { 1, "移動平均" }
            };
            Init();
        }
        private void ConfirmChangeAction()
        {
            string sql = string.Format(
                @"Update [{0}].[SystemInfo].[SystemParameters] Set SysPar_Value = {1} Where SysPar_Name = 'AvgCost'",
                Properties.Settings.Default.SystemSerialNumber,
                SelectItem);
            SQLServerConnection.DapperQuery((conn) =>
            {
                _ = conn.Query<int>(sql, commandType: CommandType.Text);
            });
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            IsDataChanged = false;
        }
        private void Init()
        {
            string sql = string.Format(@"Select SysPar_Value From [{0}].[SystemInfo].[SystemParameters] Where SysPar_Name = 'AvgCost'",
                Properties.Settings.Default.SystemSerialNumber);
            int calculate = 0;
            SQLServerConnection.DapperQuery((conn) =>
            {
                calculate = conn.Query<int>(sql, commandType: CommandType.Text).First();
            });
            SelectItem = calculate;
        }
    }
}
