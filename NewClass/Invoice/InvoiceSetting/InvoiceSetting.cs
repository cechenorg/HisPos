﻿using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Invoice.InvoiceSetting
{
   public class InvoiceSetting : ObservableObject
    {
        public InvoiceSetting(DataRow r)
        {
            CooperavieClinic = VM.GetInstitution(r.Field<string>("CooCli_ID")); 
            TypeName = r.Field<string>("CooCli_Type");
            IsPurge = r.Field<bool>("CooCli_IsPurge"); 
            FilePath = r.Field<string>("CooCli_FolderPath");
            NormalIsBuckle = r.Field<bool>("CooCli_NorIsBuckle");
            NormalWareHouse = VM.GetWareHouse(r.Field<int>("CooCli_NorWareHouseID").ToString());
            ChronicIsBuckle = r.Field<bool>("CooCli_ChiIsBuckle");
            ChronicWareHouse = VM.GetWareHouse(r.Field<int>("CooCli_ChiWareHouseID").ToString());
     
            if (!(FilePath is null)) {
                var tempsplit = FilePath.Split('\\');
                DisplayFilePath = tempsplit[tempsplit.Length - 1];
            }
            IsInstitutionEdit = true;
        }
        public InvoiceSetting() {
            NormalWareHouse = VM.GetWareHouse("0");
            ChronicWareHouse = VM.GetWareHouse("0");
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
        private bool normalIsBuckle;
        public bool NormalIsBuckle
        {
            get { return normalIsBuckle; }
            set { Set(() => NormalIsBuckle, ref normalIsBuckle, value); }
        }
        private WareHouse.WareHouse normalWareHouse;
        public WareHouse.WareHouse NormalWareHouse
        {
            get { return normalWareHouse; }
            set { Set(() => NormalWareHouse, ref normalWareHouse, value); }
        }
        private bool chronicIsBuckle;
        public bool ChronicIsBuckle
        {
            get { return chronicIsBuckle; }
            set { Set(() => ChronicIsBuckle, ref chronicIsBuckle, value); }
        }
        private WareHouse.WareHouse chronicWareHouse;
        public WareHouse.WareHouse ChronicWareHouse
        {
            get { return chronicWareHouse; }
            set { Set(() => ChronicWareHouse, ref chronicWareHouse, value); }
        }
      
    }
}
