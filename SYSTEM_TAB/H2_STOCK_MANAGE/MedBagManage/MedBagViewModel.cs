using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.MedBagManage;
using His_Pos.Service.ExportService;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private bool isBusy;
        private string busyContent;
        private MedBagPrescriptionStructs reserveCollection;
        private MedBagPrescriptionStructs registerCollection;
        private MedBagPrescriptionStructs pastReserveCollection;
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

        public MedBagPrescriptionStructs PastReserveCollection
        {
            get => pastReserveCollection;
            set { Set(() => PastReserveCollection, ref pastReserveCollection, value); }
        }

        public double TotalStockValue => (ReserveCollection is null || RegisterCollection is null) ? 0 : ReserveCollection.TotalStockValue + RegisterCollection.TotalStockValue;

        #endregion ----- Define Variables -----

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
                PastReserveCollection = MedBagPrescriptionStructs.GetPastReserveMedBagPrescriptions();
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
            IsBusy = true;
            BusyContent = "匯出資料";

            bool isSuccess = false;

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += (sender, args) =>
            {
                Collection<object> tempCollection = new Collection<object>() { ReserveCollection, RegisterCollection };

                MainWindow.ServerConnection.OpenConnection();
                ExportExcelService service = new ExportExcelService(tempCollection, new ExportMedBagTemplate());
                isSuccess = service.Export($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\藥袋資料{DateTime.Now:yyyyMMdd-hhmmss}.xlsx");
                MainWindow.ServerConnection.CloseConnection();
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                if (isSuccess)
                    MessageWindow.ShowMessage("匯出成功!", MessageType.SUCCESS);
                else
                    MessageWindow.ShowMessage("匯出失敗 請稍後再試", MessageType.ERROR);

                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
        }

        private void ReloadAction()
        {
            initBackgroundWorker.RunWorkerAsync();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            ReloadCommand = new RelayCommand(ReloadAction);
            ExportCommand = new RelayCommand(ExportAction);
        }

        #endregion ----- Define Functions -----
    }
}