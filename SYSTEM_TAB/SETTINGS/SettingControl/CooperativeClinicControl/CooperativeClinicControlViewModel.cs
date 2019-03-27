using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.Cooperative.CooperativeClinicSetting;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.CooperativeClinicControl
{
    public class CooperativeClinicControlViewModel : ViewModelBase
    {
        #region Var
        public Institutions Institutions { get; set; }
        public CooperativeClinicSetting selectItem; 
        public CooperativeClinicSetting SelectItem {
            get { return selectItem; }
            set { Set(() => SelectItem, ref selectItem, value); }
        }
        public CooperativeClinicSettings cooperativeClinicSettingCollection = new CooperativeClinicSettings();
        public CooperativeClinicSettings CooperativeClinicSettingCollection
        {
            get { return cooperativeClinicSettingCollection; }
            set { Set(() => CooperativeClinicSettingCollection, ref cooperativeClinicSettingCollection, value); }
        }
        #endregion
        #region Command
        public RelayCommand<string> ShowInstitutionSelectionWindow { get; set; }
        #endregion
        public CooperativeClinicControlViewModel() {
            Institutions = VM.Institutions;
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
        }
        #region Action
        private void ShowInsSelectionWindowAction(string search)
        {
            if (CooperativeClinicSettingCollection.Count == 0 && SelectItem is null) {
                CooperativeClinicSettingCollection.Add(new CooperativeClinicSetting());
                SelectItem = CooperativeClinicSettingCollection[0];
            }
           
            var result = Institutions.Where(i => i.ID.Contains(search) || i.Name.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    SelectItem.CooperavieClinic = result[0]; 
                    break;
                default:
                    Messenger.Default.Register<Institution>(this,nameof(CooperativeClinicControlViewModel) + "InsSelected", GetSelectedInstitution);
                    var institutionSelectionWindow = new InstitutionSelectionWindow(search, ViewModelEnum.CooperativeClinicControl);
                    institutionSelectionWindow.ShowDialog();
                    break;
            }
        }
        private void GetSelectedInstitution(Institution receiveSelectedInstitution)
        {
            Messenger.Default.Unregister<Institution>(this, nameof(CooperativeClinicControlViewModel) + "InsSelected", GetSelectedInstitution);
            SelectItem.CooperavieClinic = receiveSelectedInstitution; 
        }
        #endregion

    }
}
