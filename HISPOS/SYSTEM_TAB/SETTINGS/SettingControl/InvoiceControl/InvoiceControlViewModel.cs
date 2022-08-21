using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using System;
using System.IO;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.InvoiceControl
{
    public class InvoiceControlViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private bool isDataChanged;
        private string invoiceCom;
        private string invoiceNumber;
        private string invoiceNumberCount;
        private string invoiceNumberStart;
        private string invoiceNumberEng;
        private bool invoiceCheck;
        public int invoiceComPick;
        private int invoiceNumberNowCount;
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
        public string InvoiceNumberStart
        {
            get => invoiceNumberStart;
            set { Set(() => InvoiceNumberStart, ref invoiceNumberStart, value); }
        }

        public string InvoiceNumberEng
        {
            get => invoiceNumberEng;
            set { Set(() => InvoiceNumberEng, ref invoiceNumberEng, value); }
        }
        public string InvoiceNumberCount
        {
            get => invoiceNumberCount;
            set { Set(() => InvoiceNumberCount, ref invoiceNumberCount, value); }
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
        public int InvoiceNumberNowCount
        {
            get => invoiceNumberNowCount;
            set { Set(() => InvoiceNumberNowCount, ref invoiceNumberNowCount, value); }
        }

        #endregion ----- Define Variables -----

        public InvoiceControlViewModel()
        {
            int num;
            int Snum;

            if (Properties.Settings.Default.InvoiceNumber == "" || Properties.Settings.Default.InvoiceNumberStart == "") { }
            else
            {
                num = Int32.Parse(Properties.Settings.Default.InvoiceNumber);
                Snum = Int32.Parse(Properties.Settings.Default.InvoiceNumberStart);
                InvoiceNumberNowCount = (num - Snum);
            }

            RegisterCommands();
            InitSavedPrinter();
        }

        #region ----- Define Actions -----
        public bool Is_Number(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]+$");
            return reg1.IsMatch(str);
        }
        public bool IsNatural_Number(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(str);
        }
        private void ConfirmChangeAction()
        {

            string eng;
            int num;
            string invnum;

            string Seng;
            int Snum;
            int Count;



            if (Properties.Settings.Default.InvoiceNumber == "" || Properties.Settings.Default.InvoiceNumberStart == "") { }
            else
            {
                num = Int32.Parse(Properties.Settings.Default.InvoiceNumber);
                Snum = Int32.Parse(Properties.Settings.Default.InvoiceNumberStart);
                eng = Properties.Settings.Default.InvoiceNumberEng;

                Count = Int32.Parse(Properties.Settings.Default.InvoiceNumberCount);
                InvoiceNumberNowCount = (num - Snum);
                
            }


            string ic;
            if (!InvoiceCheck)
            {
                ic = "0";
            }
            else
            {
                ic = "1";
            }

            if (!Is_Number(InvoiceNumber))
            {
                MessageWindow.ShowMessage("發票號碼為八位數字！", MessageType.ERROR);
                return;
            }
            if (!Is_Number(InvoiceNumberStart))
            {
                MessageWindow.ShowMessage("起始發票號碼為八位數字！", MessageType.ERROR);
                return;
            }
            if (!IsNatural_Number(InvoiceNumberEng))
            {
                MessageWindow.ShowMessage("發票字軌為英數2位！", MessageType.ERROR);
                return;
            }


            if (InvoiceNumber.Length != 8)
            {
                MessageWindow.ShowMessage("發票號碼為八位數字！", MessageType.ERROR);
                return;
            }
            if (InvoiceNumberStart.Length !=8)
            {
                MessageWindow.ShowMessage("起始發票號碼為八位數字！", MessageType.ERROR);
                return;
            }
            if (InvoiceNumberEng.Length != 2)
            {
                MessageWindow.ShowMessage("發票字軌為兩位英文！", MessageType.ERROR);
                return;
            }

          

            if (String.IsNullOrEmpty(InvoiceNumberCount)) {
                MessageWindow.ShowMessage("請輸入發票張數", MessageType.ERROR);
                return;
            }


            if (Int32.Parse(InvoiceNumberStart) > Int32.Parse(InvoiceNumber))
            {
                MessageWindow.ShowMessage("當前發票號碼小於起始！", MessageType.ERROR);
                return;
            }
            if (Int32.Parse(InvoiceNumberStart)+Int32.Parse(InvoiceNumberCount)-1<Int32.Parse(InvoiceNumber))
            {
                MessageWindow.ShowMessage("當前發票號碼大於總張數！", MessageType.ERROR);
                return;
            }

            InvoiceNumberNowCount = Int32.Parse(InvoiceNumber) - Int32.Parse(InvoiceNumberStart);

            Properties.Settings.Default.InvoiceNumber = InvoiceNumber.ToString();
            Properties.Settings.Default.InvoiceComPort = InvoiceCom.ToString();
            Properties.Settings.Default.InvoiceNumberCount = InvoiceNumberCount.ToString();
            Properties.Settings.Default.InvoiceNumberStart = InvoiceNumberStart.ToString();
            Properties.Settings.Default.InvoiceNumberEng = InvoiceNumberEng.ToString();
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
                fileWriter.WriteLine("INumS " + Properties.Settings.Default.InvoiceNumberStart);
                fileWriter.WriteLine("INumC " + Properties.Settings.Default.InvoiceNumberCount);
                fileWriter.WriteLine("INumE " + Properties.Settings.Default.InvoiceNumberEng);
                fileWriter.WriteLine("PP " + Properties.Settings.Default.PrePrint);
                fileWriter.WriteLine("RPF " + Properties.Settings.Default.ReportFormat);
            }

            IsDataChanged = false;
        }

        public void InvoiceNumPlusOneAction()
        {
            string eng;
            int num;
            string invnum;

            string Seng;
            int Snum;
            int Count;

            eng = Properties.Settings.Default.InvoiceNumberEng;
            num = Int32.Parse(Properties.Settings.Default.InvoiceNumber);
            Snum = Int32.Parse(Properties.Settings.Default.InvoiceNumberStart);
            Count = Int32.Parse(Properties.Settings.Default.InvoiceNumberCount);
            if ((Count - (num - Snum)) < 20)
            {
                MessageWindow.ShowMessage("發票即將耗盡！剩餘："+(Count-(num - Snum)-1).ToString()+"張", MessageType.ERROR);
            }



            num = num + 1;
            invnum = eng + num.ToString();

            Properties.Settings.Default.InvoiceNumber = num.ToString().PadLeft(8,'0');
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
                fileWriter.WriteLine("INumS " + Properties.Settings.Default.InvoiceNumberStart);
                fileWriter.WriteLine("INumC " + Properties.Settings.Default.InvoiceNumberCount);
                fileWriter.WriteLine("INumE " + Properties.Settings.Default.InvoiceNumberEng);
                fileWriter.WriteLine("PP " + Properties.Settings.Default.InvoiceNumberEng);
                fileWriter.WriteLine("RPF " + Properties.Settings.Default.ReportFormat);
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
                fileWriter.WriteLine("INumS " + Properties.Settings.Default.InvoiceNumberStart);
                fileWriter.WriteLine("INumC " + Properties.Settings.Default.InvoiceNumberCount);
                fileWriter.WriteLine("INumE " + Properties.Settings.Default.InvoiceNumberEng);
                fileWriter.WriteLine("PP " + Properties.Settings.Default.PrePrint);
                fileWriter.WriteLine("RPF " + Properties.Settings.Default.ReportFormat);
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

        #endregion ----- Define Actions -----

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
            InvoiceNumberCount = Properties.Settings.Default.InvoiceNumberCount;
            InvoiceNumberEng = Properties.Settings.Default.InvoiceNumberEng;
            InvoiceNumberStart = Properties.Settings.Default.InvoiceNumberStart;
            if (Properties.Settings.Default.InvoiceCheck == "1")
            {
                InvoiceCheck = true;
            }
            else
            {
                InvoiceCheck = false;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.InvoiceComPort) && Properties.Settings.Default.InvoiceComPort != "0")
            {
                InvoiceComPick = int.Parse(Properties.Settings.Default.InvoiceComPort.Substring(3));
            }
            else
            {
                return;
            }
        }

        #endregion ----- Define Functions -----
    }
}