using System;
using System.Collections.Generic;
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
        private MedBagPrescriptionStructs reserveCollection;
        private MedBagPrescriptionStructs registerCollection;

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
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void ExportAction()
        {

        }
        private void ReloadAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            ReserveCollection = MedBagPrescriptionStructs.GetReserveMedBagPrescriptions();
            RegisterCollection = MedBagPrescriptionStructs.GetRegisterMedBagPrescriptions();
            MainWindow.ServerConnection.CloseConnection();

            RaisePropertyChanged(nameof(TotalStockValue));
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
