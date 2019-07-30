using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Prescription.MedBagManage;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.MedBagManage
{
    public class MedBagViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isBusy;
        private string busyContent;
        private MedBagPrescriptionStructs reserveCollection;
        private MedBagPrescriptionStructs registerCollection;
        private BackgroundWorker initBackgroundWorker;

        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }
        public string BusyContent
        {
            get => busyContent;
            set { Set(() => BusyContent, ref busyContent, value); }
        }
        public MedBagPrescriptionStructs ReserveCollection
        {
            get => reserveCollection;
            set { Set(() => ReserveCollection, ref reserveCollection, value); }
        }
        public MedBagPrescriptionStructs RegisterCollection
        {
            get => registerCollection;
            set { Set(() => RegisterCollection, ref registerCollection, value); }
        }
        public double TotalStockValue => (ReserveCollection is null || RegisterCollection is null)? 0 : ReserveCollection.TotalStockValue + RegisterCollection.TotalStockValue;
        #endregion

        public MedBagViewModel()
        {
            InitBackgroundWorker();
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void InitBackgroundWorker()
        {
            initBackgroundWorker = new BackgroundWorker();

            initBackgroundWorker.DoWork += (sender, args) =>
            {
                IsBusy = true;
                BusyContent = "藥袋資料查詢中";

                MainWindow.ServerConnection.OpenConnection();
                ReserveCollection = MedBagPrescriptionStructs.GetReserveMedBagPrescriptions();
                RegisterCollection = MedBagPrescriptionStructs.GetRegisterMedBagPrescriptions();
                MainWindow.ServerConnection.CloseConnection();
            };

            initBackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                RaisePropertyChanged(nameof(TotalStockValue));

                IsBusy = false;
            };
        }
        private void ExportAction()
        {

        }
        private void ReloadAction()
        {
            initBackgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ExportCommand = new RelayCommand(ExportAction);
        }
        #endregion
    }
}
