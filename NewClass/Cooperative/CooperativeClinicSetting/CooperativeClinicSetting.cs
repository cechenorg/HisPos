﻿using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicSetting
{
   public class CooperativeClinicSetting : ObservableObject
    {
        public CooperativeClinicSetting(DataRow r)
        {
            CooperavieClinic = VM.GetInstitution(r.Field<string>("CooCli_ID"));
            TypeName = r.Field<string>("CooCli_Type");
            IsPurge = r.Field<bool>("CooCli_IsPurge");
            IsBuckle = r.Field<bool>("CooCli_IsBuckle");
            FilePath = r.Field<string>("CooCli_FolderPath");
            var tempsplit = FilePath.Split('\\');
            DisplayFilePath = tempsplit[tempsplit.Length - 1];
            IsInstitutionEdit = true;
        }
        public CooperativeClinicSetting() {

        }
         
        private Institution cooperavieClinic;
        public Institution CooperavieClinic {
            get { return cooperavieClinic; }
            set { Set(() => CooperavieClinic, ref cooperavieClinic, value); }
        }
        private bool isInstitutionEdit = false;
        public bool IsInstitutionEdit
        {
            get { return isInstitutionEdit; }
            set { Set(() => IsInstitutionEdit, ref isInstitutionEdit, value); }
        }
        private bool isPurge;
        public bool IsPurge
        {
            get { return isPurge; }
            set { Set(() => IsPurge, ref isPurge, value); }
        }
        private bool isBuckle;
        public bool IsBuckle
        {
            get { return isBuckle; }
            set { Set(() => IsBuckle, ref isBuckle, value); }
        }
        private string typeName;
        public string TypeName
        {
            get { return typeName; }
            set { Set(() => TypeName, ref typeName, value); }
        }
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { Set(() => FilePath, ref filePath, value); }
        }
        private string displayFilePath;
        public string DisplayFilePath
        {
            get { return displayFilePath; }
            set { Set(() => DisplayFilePath, ref displayFilePath, value); }
        }
    }
}
