﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.PrinterControl
{
    public class PrinterControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isDataChanged;
        private string medBagPrinter;
        private string receiptPrinter;
        private string reportPrinter;
        private string receiptForm;
        public Collection<string> Printers { get; set; }
        public Collection<string> PrintForms { get; set; }
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
        public string MedBagPrinter
        {
            get => medBagPrinter;
            set { Set(() => MedBagPrinter, ref medBagPrinter, value); }
        }
        public string ReceiptPrinter
        {
            get => receiptPrinter;
            set { Set(() => ReceiptPrinter, ref receiptPrinter, value); }
        }
        public string ReceiptForm
        {
            get => receiptForm;
            set { Set(() => ReceiptForm, ref receiptForm, value); }
        }
        public string ReportPrinter
        {
            get => reportPrinter;
            set { Set(() => ReportPrinter, ref reportPrinter, value); }
        }
        #endregion

        public PrinterControlViewModel()
        {
            RegisterCommands();
            InitPrinters();
            InitSavedPrinter();
        }

        #region ----- Define Actions -----
        private void ConfirmChangeAction()
        {

            Properties.Settings.Default.MedBagPrinter = MedBagPrinter;
            Properties.Settings.Default.ReceiptPrinter = ReceiptPrinter;
            Properties.Settings.Default.ReportPrinter = ReportPrinter;
            Properties.Settings.Default.ReceiptForm = ReceiptForm;
            Properties.Settings.Default.Save();

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine();
            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);

                fileWriter.WriteLine("M " + Properties.Settings.Default.MedBagPrinter);
                fileWriter.WriteLine("Rc " + Properties.Settings.Default.ReceiptPrinter + "$" + ReceiptForm);
                fileWriter.WriteLine("Rp " + Properties.Settings.Default.ReportPrinter);
                fileWriter.WriteLine("Com " + Properties.Settings.Default.ReaderComPort);
            }

            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            InitSavedPrinter();
            IsDataChanged = false;
        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        #endregion

        #region ----- Define Functions -----
        private void InitPrinters()
        {
            Printers = new Collection<string>();

            for (int i = 0; i<PrinterSettings.InstalledPrinters.Count; i++)
            {
                Printers.Add(PrinterSettings.InstalledPrinters[i]);
            }

            PrintForms = new BindingList<string> { "點陣" , "一般" };
        }
        private void RegisterCommands()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsPrinterDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsPrinterDataChanged);
            DataChangedCommand = new RelayCommand(DataChangedAction);
        }
        private bool IsPrinterDataChanged()
        {
            return IsDataChanged;
        }
        private void InitSavedPrinter()
        {
            MedBagPrinter = Properties.Settings.Default.MedBagPrinter;
            ReceiptPrinter = Properties.Settings.Default.ReceiptPrinter;
            ReportPrinter = Properties.Settings.Default.ReportPrinter;
            ReceiptForm = Properties.Settings.Default.ReceiptForm;
        }
        #endregion
    }
}
