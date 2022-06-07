using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport.InstitutionDeclarePoint;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.InstitutionDeclarePointReport
{
    public class InstitutionDeclarePointReportViewModel : TabBase
    {
        #region Var

        public override TabBase getTab()
        {
            return this;
        }

        private DateTime searchDate;

        public DateTime SearchDate
        {
            get => searchDate;
            set
            {
                Set(() => SearchDate, ref searchDate, value);
            }
        }

        private InstitutionDeclarePoints institutionDeclarePointCollection = new InstitutionDeclarePoints();

        public InstitutionDeclarePoints InstitutionDeclarePointCollection
        {
            get => institutionDeclarePointCollection;
            set
            {
                Set(() => InstitutionDeclarePointCollection, ref institutionDeclarePointCollection, value);
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ExportExcelCommand { get; set; }

        #endregion Var

        public InstitutionDeclarePointReportViewModel()
        {
            SearchDate = DateTime.Today.AddMonths(-1);
            //InstitutionDeclarePointCollection.GetDataByDate(SearchDate);
            SearchCommand = new RelayCommand(SearchAction);
            ExportExcelCommand = new RelayCommand(ExportExcelAction);
        }

        #region Function

        private void SearchAction()
        {
            //InstitutionDeclarePointCollection.GetDataByDate(SearchDate);
        }

        private void ExportExcelAction()
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            fdlg.Title = "院所申報點數統計表";
            fdlg.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath;   //@是取消转义字符的意思
            fdlg.Filter = "Csv檔案|*.csv";
            fdlg.FileName = SearchDate.Month.ToString() + "月" + ViewModelMainWindow.CurrentPharmacy.Name + "院所申報統計表";
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
                        file.WriteLine(ViewModelMainWindow.CurrentPharmacy.Name);
                        file.WriteLine(ViewModelMainWindow.CurrentPharmacy.Name);
                        file.WriteLine("院所申報統計表");
                        file.WriteLine("月份 " + SearchDate.Month.ToString() + "月");
                        file.WriteLine("院所,藥品點,特材點,藥服費,小計,部分負擔,申報額,筆數");
                        foreach (InstitutionDeclarePoint ins in InstitutionDeclarePointCollection)
                        {
                            file.WriteLine($"{ins.InsName},{ins.MedicinePoint},{ins.SpecialMedPoint},{ins.MedicalServicePoint},{ins.SubTotal},{ins.CopayMentPoint},{ins.DeclarePoint},{ins.PrescriptionCount}");
                        }
                        InstitutionDeclarePoint sum = new InstitutionDeclarePoint();
                        sum.InsName = "總計";
                        sum.MedicinePoint = InstitutionDeclarePointCollection.Sum(ins => ins.MedicinePoint);
                        sum.SpecialMedPoint = InstitutionDeclarePointCollection.Sum(ins => ins.SpecialMedPoint);
                        sum.MedicalServicePoint = InstitutionDeclarePointCollection.Sum(ins => ins.MedicalServicePoint);
                        sum.SubTotal = InstitutionDeclarePointCollection.Sum(ins => ins.SubTotal);
                        sum.CopayMentPoint = InstitutionDeclarePointCollection.Sum(ins => ins.CopayMentPoint);
                        sum.DeclarePoint = InstitutionDeclarePointCollection.Sum(ins => ins.DeclarePoint);
                        sum.PrescriptionCount = InstitutionDeclarePointCollection.Sum(ins => ins.PrescriptionCount);
                        file.WriteLine($"{sum.InsName},{sum.MedicinePoint},{sum.SpecialMedPoint},{sum.MedicalServicePoint},{sum.SubTotal},{sum.CopayMentPoint},{sum.DeclarePoint},{sum.PrescriptionCount}");
                        file.Close();
                        file.Dispose();
                    }
                    MessageWindow.ShowMessage("匯出Excel", MessageType.SUCCESS);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }

        #endregion Function
    }
}