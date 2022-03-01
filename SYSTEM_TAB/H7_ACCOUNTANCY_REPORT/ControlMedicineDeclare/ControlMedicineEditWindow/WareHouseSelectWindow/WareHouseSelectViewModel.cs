using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.ControlMedicineDetail;
using His_Pos.NewClass.WareHouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditWindow.WareHouseSelectWindow
{
    public class WareHouseSelectViewModel : ViewModelBase
    {
        public DateTime SDateTime { get; set; }
        public DateTime EDateTime { get; set; }
        public WareHouses WareHouseCollection { get; set; } = WareHouses.GetWareHouses();
        public RelayCommand SubmitCommand { get; set; }

        public WareHouseSelectViewModel(DateTime sDate, DateTime eDate)
        {
            SDateTime = sDate;
            EDateTime = eDate;
            SubmitCommand = new RelayCommand(SubmitAction);
        }

        private void SubmitAction()
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "管制藥品批次申報檔";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Txt檔案|*.txt";
            fdlg.FileName = SDateTime.ToString("yyyy") + ViewModelMainWindow.CurrentPharmacy.Name + "管制藥品批次申報檔";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.DeclareXmlPath = fdlg.FileName;
                Properties.Settings.Default.Save();
                try
                {
                    using (var file = new StreamWriter(fdlg.FileName, false, Encoding.UTF8))
                    {
                        List<string> warList = new List<string>();
                        foreach (var w in WareHouseCollection)
                        {
                            if (w.IsSelected)
                                warList.Add(w.ID);
                        }
                        ControlMedicineDetails controlMedicineDetails = ControlMedicineDetails.GetDeclareData(SDateTime, EDateTime, warList);

                        int index = 1;
                        foreach (var c in controlMedicineDetails)
                        {
                            string isgetpay = controlMedicineDetails.Count(ta => ta.MedID == c.MedID) > 1 ? "Y" : "N";
                            file.WriteLine($"" +
                                 $"{index}," +
                                 $"P," +
                                 $"{c.MedID}," +
                                 $"{c.BatchNumber}," +
                                 $"{isgetpay}," +
                                 $"{c.PackageName}," +
                                 $"{c.Date.AddYears(-1911).ToString("yyyMMdd")},{c.TypeName}," +
                                 $"{c.InputAmount},{c.InputAmount},,{c.ManufactoryControlMedicinesID},{c.ManufactoryName},,,,,,,,,,,,,,,");
                            index++;
                        }
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);

                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseWareHouseSelectWindow"));
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
    }
}