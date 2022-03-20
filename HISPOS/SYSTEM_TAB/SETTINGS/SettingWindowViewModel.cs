using GalaSoft.MvvmLight;
using His_Pos.NewClass.Setting;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace His_Pos.SYSTEM_TAB.SETTINGS
{
    public class SettingWindowViewModel : ViewModelBase
    {
        #region ----- Define Variables -----

        private SettingTabData selectedSettingTab;

        public string Version { get; set; }
        public SettingTabDatas SettingTabCollection { get; set; }

        public SettingTabData SelectedSettingTab
        {
            get => selectedSettingTab;
            set { Set(() => SelectedSettingTab, ref selectedSettingTab, value); }
        }

        #endregion ----- Define Variables -----

        public SettingWindowViewModel()
        {
            InitSettingCollections();
            InitSettingDatas();

            Version = "系統版本  " + Assembly.GetExecutingAssembly().GetName().Version;
        }



        #region ----- Define Functions -----

        private void InitSettingCollections()
        {
            SettingTabCollection = new SettingTabDatas();

            SettingTabCollection.Add(new SettingTabData(SettingTabs.MyPharmacy, "藥局設定", "/Images/pharmacy.png"));
            SettingTabCollection.Add(new SettingTabData(SettingTabs.Invoice, "發票設定", "/Images/pharmacy.png"));
            SettingTabCollection.Add(new SettingTabData(SettingTabs.Printer, "印表機設定", "/Images/Printer.png"));
            SettingTabCollection.Add(new SettingTabData(SettingTabs.CooperativeClinic, "合作診所設定", "/Images/Cooperate.png"));
            SettingTabCollection.Add(new SettingTabData(SettingTabs.WareHouse, "庫別設定", "/Images/StockTaking.png"));

            SelectedSettingTab = SettingTabCollection[0];
        }

        private void InitSettingDatas()
        {
            Regex medReg = new Regex(@"M (.*)");
            Regex recReg = new Regex(@"Rc (.*)");
            Regex recRegWithForm = new Regex(@"Rc (.*)[$](.*)");
            Regex repReg = new Regex(@"Rp (.*)");

            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";

            using (StreamReader fileReader = new StreamReader(filePath))
            {
                fileReader.ReadLine();

                string newLine = fileReader.ReadLine();
                Match match = medReg.Match(newLine);
                Properties.Settings.Default.MedBagPrinter = match.Groups[1].Value;

                newLine = fileReader.ReadLine();
                if (newLine.Contains("$"))
                {
                    match = recRegWithForm.Match(newLine);
                    Properties.Settings.Default.ReceiptPrinter = match.Groups[1].Value;
                    Properties.Settings.Default.ReceiptForm = match.Groups[2].Value;
                }
                else
                {
                    match = recReg.Match(newLine);
                    Properties.Settings.Default.ReceiptPrinter = match.Groups[1].Value;
                    Properties.Settings.Default.ReceiptForm = "點陣";
                }

                newLine = fileReader.ReadLine();
                match = repReg.Match(newLine);
                Properties.Settings.Default.ReportPrinter = match.Groups[1].Value;

                Properties.Settings.Default.Save();
            }
        }

        #endregion ----- Define Functions -----
    }
}