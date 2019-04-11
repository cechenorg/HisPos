﻿using GalaSoft.MvvmLight;
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
using System.Windows.Forms;
using His_Pos.FunctionWindow;
using System.Collections.ObjectModel;
using His_Pos.NewClass.WareHouse;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.CooperativeClinicControl
{
    public class CooperativeClinicControlViewModel : ViewModelBase
    {
        #region Var
        public Institutions Institutions { get; set; }
        private WareHouses wareHouses;
        public WareHouses WareHouses
        {
            get { return wareHouses; }
            set { Set(() => WareHouses, ref wareHouses, value); }
        }
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
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand DeleteInstitutionCommand { get; set; }
        #endregion

        public CooperativeClinicControlViewModel() { 
            MainWindow.ServerConnection.OpenConnection();
            CooperativeClinicSettingCollection.Init();
            MainWindow.ServerConnection.CloseConnection();
            Institutions = VM.Institutions;
            WareHouses = VM.WareHouses;
            ShowInstitutionSelectionWindow = new RelayCommand<string>(ShowInsSelectionWindowAction);
            OpenFileCommand = new RelayCommand(OpenFileAction);
            UpdateCommand = new RelayCommand(UpdateAction);
            DeleteInstitutionCommand = new RelayCommand(DeleteInstitutionAction);
        }
        #region Action
       
        private void DeleteInstitutionAction() {
            CooperativeClinicSettingCollection.Remove(SelectItem);
        }
        public void Init() {
            MainWindow.ServerConnection.OpenConnection();
            CooperativeClinicSettingCollection.Init();
            MainWindow.ServerConnection.CloseConnection(); 
        }
        private void UpdateAction() {
            var dailyUploadConfirm = new ConfirmWindow("更新後會影響處方資料串接與扣庫 是否更新合作診所? ", "合作診所更新確認", false);
            if (dailyUploadConfirm.DialogResult == true) {
                CooperativeClinicSettingCollection.Update();
                MessageWindow.ShowMessage("新增成功!", Class.MessageType.SUCCESS);
            }
                
        }
        private void OpenFileAction() {
            if (SelectItem is null)
                MessageWindow.ShowMessage("請先填寫院所戴碼",Class.MessageType.WARNING);
            FolderBrowserDialog fdlg = new FolderBrowserDialog(); 
            if (fdlg.ShowDialog() == DialogResult.OK) {
                SelectItem.FilePath = fdlg.SelectedPath;
                var tempsplit = SelectItem.FilePath.Split('\\');
                SelectItem.DisplayFilePath = tempsplit[tempsplit.Length - 1]; 
            }
        }
        private void ShowInsSelectionWindowAction(string search)
        {  
            var result = Institutions.Where(i => i.ID.Contains(search) || i.Name.Contains(search)).ToList();
            switch (result.Count)
            {
                case 0:
                    return;
                case 1:
                    if (CheckInsSame(result[0])) {
                        CooperativeClinicSetting temp = new CooperativeClinicSetting(); 
                        CooperativeClinicSettingCollection.Add(new CooperativeClinicSetting());
                        SelectItem = CooperativeClinicSettingCollection[CooperativeClinicSettingCollection.Count - 1];
                        SelectItem.CooperavieClinic = result[0];
                        SelectItem.IsInstitutionEdit = true;
                    } 
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
            if (CheckInsSame(receiveSelectedInstitution)) {
                CooperativeClinicSettingCollection.Add(new CooperativeClinicSetting());
                SelectItem = CooperativeClinicSettingCollection[CooperativeClinicSettingCollection.Count - 1];
                SelectItem.CooperavieClinic = receiveSelectedInstitution;
                SelectItem.IsInstitutionEdit = true;
            } 
        }
        private bool CheckInsSame(Institution receiveSelectedInstitution) {
            if (CooperativeClinicSettingCollection.Where(c => !(c.CooperavieClinic is null)).Count(c => c.CooperavieClinic.ID == receiveSelectedInstitution.ID) > 0)
            {
                MessageWindow.ShowMessage("合作院所重複新增", Class.MessageType.ERROR);
                return false;
            }
            else
                return true;
        }
        #endregion

    }
}
