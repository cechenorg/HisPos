using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.InvoiceControl
{
    public class InvoiceControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isDataChanged;
        private string invoiceCom;
        private string invoiceNumber;
        private bool invoiceCheck;
        public int invoiceComPick;

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
        public string InvoiceCom
        {
            get => invoiceCom;
            set { Set(() => InvoiceCom, ref invoiceCom, value); }
        }

        public string InvoiceNumber
        {
            get => invoiceNumber;
            set { Set(() => InvoiceNumber, ref invoiceNumber, value); }
        }
        public bool InvoiceCheck
        {
            get => invoiceCheck;
            set { Set(() => InvoiceCheck, ref invoiceCheck, value); }
        }
        public int InvoiceComPick
        {
            get => invoiceComPick;
            set { Set(() => InvoiceComPick, ref invoiceComPick, value); }
        }

        #endregion

        public InvoiceControlViewModel()
        {
            RegisterCommands();
            InitSavedPrinter();
        }

        #region ----- Define Actions -----
        private void ConfirmChangeAction()
        {
            string ic;
            if (!InvoiceCheck) {
                ic = "0";
            }
            else {
                ic = "1";
            }
            Properties.Settings.Default.InvoiceNumber = InvoiceNumber.ToString();
            Properties.Settings.Default.InvoiceComPort = InvoiceCom.ToString();
            Properties.Settings.Default.InvoiceCheck = ic;
            Properties.Settings.Default.Save();

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine();

            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);
                fileWriter.WriteLine("ICom " + Properties.Settings.Default.InvoiceComPort);
                fileWriter.WriteLine("INum " + Properties.Settings.Default.InvoiceNumber);
                fileWriter.WriteLine("IChk " + Properties.Settings.Default.InvoiceCheck);

            }

            IsDataChanged = false;
        }
        public void InvoiceNumPlusOneAction()
        {
            string eng;
            int num;
            string invnum;

            eng = Properties.Settings.Default.InvoiceNumber.Substring(0, 1);
            num = int.Parse(Properties.Settings.Default.InvoiceNumber.Substring(2, 7));
            num = num + 1;
            invnum = eng + num.ToString();


            Properties.Settings.Default.InvoiceNumber = invnum;
            Properties.Settings.Default.Save();

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine();

            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);
                fileWriter.WriteLine("INum " + Properties.Settings.Default.InvoiceNumber);
                fileWriter.WriteLine("IChk " + Properties.Settings.Default.InvoiceCheck);

            }

            IsDataChanged = false;
        }
        public void InvoiceNumAssignAction(string invnum)
        {

            Properties.Settings.Default.InvoiceNumber = invnum;
            Properties.Settings.Default.Save();

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            string leftLines = "";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine() + "\r\n";
                leftLines += fileReader.ReadLine();

            }

            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);
                fileWriter.WriteLine("INum " + Properties.Settings.Default.InvoiceNumber);
                fileWriter.WriteLine("IChk " + Properties.Settings.Default.InvoiceCheck);

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
            InvoiceNumber = Properties.Settings.Default.InvoiceNumber;
            if (Properties.Settings.Default.InvoiceCheck == "1")
            {
                InvoiceCheck = true;
            }
            else {
                InvoiceCheck = false;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.InvoiceComPort) && Properties.Settings.Default.InvoiceComPort != "0")
            {
                InvoiceComPick = int.Parse(Properties.Settings.Default.InvoiceComPort.Substring(3));
            }
            else {
                return;
            }
        }
        #endregion
    }
}
