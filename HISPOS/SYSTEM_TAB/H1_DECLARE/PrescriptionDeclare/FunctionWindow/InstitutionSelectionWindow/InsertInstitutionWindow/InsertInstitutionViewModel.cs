using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Treatment.Institution;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow.InsertInstitutionWindow
{
    internal class InsertInstitutionViewModel :ViewModelBase
    {
        public delegate void InsertDataCallback(string insID);

        private InsertDataCallback _insertDataCallback;


        private string insID;

        public string InsID
        {
            get { return insID; }
            set { Set(() => InsID, ref insID, value); }
        }

        private string insName;

        public string InsName
        {
            get { return insName; }
            set { Set(() => InsName, ref insName, value); }
        }

        private string insPhone;

        public string InsPhone
        {
            get { return insPhone; }
            set { Set(() => InsPhone, ref insPhone, value); }
        }


        private string insAddress;

        public string InsAddress
        {
            get { return insAddress; }
            set { Set(() => InsAddress, ref insAddress, value); }
        }

        public RelayCommand<Window> CannelCommand { get; set; }

        public RelayCommand<Window> InsertCommand { get; set; }

        public InsertInstitutionViewModel()
        {
            CannelCommand = new RelayCommand<Window>(CannelAction);
            InsertCommand = new RelayCommand<Window>(InsertAction);
        }

        public void SetInsertDataCallback(InsertDataCallback callback)
        {
            _insertDataCallback = callback;
        }

        private void CannelAction(Window window)
        {
            window.Close();
        }

        public void InsertAction(Window window)
        {
            if(CheckInsInfo() == false)
                return;
              
            InstitutionDb.InsertInstitution(insID,insName,insAddress,insPhone, ViewModelMainWindow.CurrentPharmacy.VerifyKey);
            MessageWindow.ShowMessage("新增成功",MessageType.SUCCESS);
            _insertDataCallback?.Invoke(InsID);
            window.Close();
        }

        private bool CheckInsInfo()
        {

            if (InsID.Length != 10)
            {
                MessageWindow.ShowMessage("院所代碼須為10碼", MessageType.ERROR);
                return false;
            }
              

            else if (string.IsNullOrEmpty(insName) || insName.Length < 4)
            {
                MessageWindow.ShowMessage("院所名稱不可為空或小於4個字", MessageType.ERROR);
                return false;
            }

            return true;
        }
    }
}
