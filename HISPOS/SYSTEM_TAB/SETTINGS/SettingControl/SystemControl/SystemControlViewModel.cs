using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
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
        private bool isCheck;
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
        public bool IsCheck
        {
            get => isCheck;
            set { Set(() => IsCheck, ref isCheck, value); }
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        private bool IsPrinterDataChanged()
        {
            return IsDataChanged;
        }
        public SystemControlViewModel()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsPrinterDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsPrinterDataChanged);
            DataChangedCommand = new RelayCommand(DataChangedAction);
        }
        private void ConfirmChangeAction()
        {
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            IsDataChanged = false;
        }
    }
}
