using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.AccountReport.InstitutionDeclarePoint;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePreview
{
    public class DeclarePreviewOfMonth : ObservableObject
    {
        public DeclarePreviewOfMonth()
        {
            DeclarePreviews = new ObservableCollection<DeclarePreviewOfDay>();
            DeclarePres = new DeclarePrescriptions();
        }

        public DeclarePrescriptions DeclarePres { get; set; }
        public ObservableCollection<DeclarePreviewOfDay> DeclarePreviews { get; set; }
        private CollectionViewSource decPreOfDaysViewSource;

        private CollectionViewSource DecPreOfDaysViewSource
        {
            get => decPreOfDaysViewSource;
            set { Set(() => DecPreOfDaysViewSource, ref decPreOfDaysViewSource, value); }
        }

        private ICollectionView decPreOfDaysCollectionView;

        public ICollectionView DecPreOfDaysCollectionView
        {
            get => decPreOfDaysCollectionView;
            set { Set(() => DecPreOfDaysCollectionView, ref decPreOfDaysCollectionView, value); }
        }

        private DeclarePreviewOfDay selectedDayPreview;

        public DeclarePreviewOfDay SelectedDayPreview
        {
            get => selectedDayPreview;
            set { Set(() => SelectedDayPreview, ref selectedDayPreview, value); }
        }

        private int notDeclareCount;

        public int NotDeclareCount
        {
            get => notDeclareCount;
            set { Set(() => NotDeclareCount, ref notDeclareCount, value); }
        }

        private int normalCount;

        public int NormalCount
        {
            get => normalCount;
            set { Set(() => NormalCount, ref normalCount, value); }
        }

        private int normalPoint;

        public int NormalPoint
        {
            get => normalPoint;
            set { Set(() => NormalPoint, ref normalPoint, value); }
        }

        private int chronicCount;

        public int ChronicCount
        {
            get => chronicCount;
            set { Set(() => ChronicCount, ref chronicCount, value); }
        }

        private int chronicPoint;

        public int ChronicPoint
        {
            get => chronicPoint;
            set { Set(() => ChronicPoint, ref chronicPoint, value); }
        }

        private int declareCount;

        public int DeclareCount
        {
            get => declareCount;
            set { Set(() => DeclareCount, ref declareCount, value); }
        }

        private int totalDeclarePoint;

        public int TotalDeclarePoint
        {
            get => totalDeclarePoint;
            set { Set(() => TotalDeclarePoint, ref totalDeclarePoint, value); }
        }

        public DateTime DeclareDate { get; set; }

        internal void GetSearchPrescriptions(DateTime sDate, DateTime eDate, string pharmacyID)
        {
            DeclarePreviews = new ObservableCollection<DeclarePreviewOfDay>();
            DeclarePres = new DeclarePrescriptions();
            DeclarePres.GetSearchPrescriptions(sDate, eDate, pharmacyID);
            foreach (var pres in DeclarePres.OrderBy(p => p.AdjustDate).GroupBy(p => p.AdjustDate)
                .Select(grp => grp.ToList()).ToList())
            {
                var preview = new DeclarePreviewOfDay();
                preview.AddPresOfDay(pres);
                DeclarePreviews.Add(preview);
            }
            DecPreOfDaysViewSource = new CollectionViewSource { Source = DeclarePreviews };
            DecPreOfDaysCollectionView = DecPreOfDaysViewSource.View;
        }

        internal void SetSummary()
        {
            var declareList = DeclarePres.Where(p => p.IsDeclare).ToList();
            var normal = declareList.Where(p => p.AdjustCase.ID.Equals("1") || p.AdjustCase.ID.Equals("3")).ToList();
            var chronic = declareList.Where(p => p.AdjustCase.ID.Equals("2")).ToList();
            NotDeclareCount = DeclarePres.Count(p => !p.IsDeclare);
            NormalCount = normal.Count;
            NormalPoint = normal.Sum(p => p.ApplyPoint);
            ChronicCount = chronic.Count;
            ChronicPoint = chronic.Sum(p => p.ApplyPoint);
            DeclareCount = declareList.Count;
            TotalDeclarePoint = declareList.Sum(p => p.ApplyPoint);
            foreach (var d in DeclarePreviews)
            {
                var presOfDayDeclareList = d.PresOfDay.Where(p => p.IsDeclare).ToList();
                d.NormalCount = presOfDayDeclareList.Count(p => p.AdjustCase.ID.Equals("1"));
                d.ChronicCount = presOfDayDeclareList.Count(p => p.AdjustCase.ID.Equals("2"));
                d.SimpleFormCount = presOfDayDeclareList.Count(p => p.AdjustCase.ID.Equals("3"));
                d.DeclareCount = presOfDayDeclareList.Count;
                d.NotDeclareCount = d.PresOfDay.Count(p => !p.IsDeclare);
                d.CheckNotDeclareCount();
            }
        }

        public void CreateDeclareFile(DeclareFile.DeclareFile doc, DateTime declareStart, DateTime declareEnd)
        {
            XDocument result;
            var xmlSerializer = new XmlSerializer(doc.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, doc);
                var document = XDocument.Parse(XmlService.PrettyXml(textWriter));
                var root = XElement.Parse(document.ToString());
                root.Element("ddata")?.Element("decId")?.Remove();
                document = XDocument.Load(root.CreateReader());
                document.Root?.RemoveAttributes();
                document.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
                result = document;
            }
            //var declareFileId = DeclareFileDb.InsertDeclareFile(result, this).Rows[0].Field<int>("DecFile_ID");
            var declareList = DeclarePres.Where(p => p.IsDeclare).Select(p => p.ID).ToList();
            //DeclarePrescriptionDb.UpdateDeclareFileID(declareFileId, declareList);
            //匯出xml檔案
            var fileName = ViewModelMainWindow.CurrentPharmacy.Name + declareStart.Date.Month + "月申報檔";
            var filePath = Function.ExportXml(result, "每月申報檔", declareStart, fileName);
            ExportExcelAction(declareStart, declareEnd, filePath.Replace("\\DRUGT", ""));
        }

        public void GetNotAdjustPrescriptionCount(DateTime? start, DateTime? end, string pharmacyID)
        {
            Debug.Assert(start != null, nameof(start) + " != null");
            Debug.Assert(end != null, nameof(end) + " != null");
            var sDate = (DateTime)start;
            var eDate = (DateTime)end;
            var table = PrescriptionDb.GetNotAdjustPrescriptionCount(sDate, eDate, pharmacyID);
            if (table.Rows.Count > 0)
            {
                var count = table.Rows[0].Field<int>("NotAdjustCount");
                var declareDateStr = sDate.Year - 1911 + " 年 " + sDate.Month.ToString().PadLeft(2, '0') + " 月 ";
                if (count > 0)
                    MessageWindow.ShowMessage(declareDateStr + "尚有 " + count + " 張慢箋未調劑結案", MessageType.WARNING);
            }
        }

        private void ExportExcelAction(DateTime declareStart, DateTime declareEnd, string filePath)
        {
            var institutionDeclarePoints = new InstitutionDeclarePoints();
            institutionDeclarePoints.GetDataByDate(declareStart, declareEnd);
            try
            {
                CreateInstitutionSummaryFile(institutionDeclarePoints, declareStart, filePath);
            }
            catch (Exception ex)
            {
                NewFunction.ShowMessageFromDispatcher(ex.Message, MessageType.ERROR);
            }
        }

        private void CreateInstitutionSummaryFile(InstitutionDeclarePoints institutionDeclarePoints, DateTime decDate, string filePath)
        {
            using (var file = new StreamWriter(filePath + "\\" + ViewModelMainWindow.CurrentPharmacy.Name + decDate.Month + "月院所申報統計表.csv", false, Encoding.UTF8))
            {
                file.WriteLine(ViewModelMainWindow.CurrentPharmacy.Name);
                file.WriteLine("院所申報統計表");
                file.WriteLine("月份 " + decDate.Month + "月");
                file.WriteLine("院所,藥品點,特材點,藥服費,小計,部分負擔,申報額,筆數");
                foreach (var ins in institutionDeclarePoints)
                {
                    file.WriteLine($"{ins.InsName},{ins.MedicinePoint},{ins.SpecialMedPoint},{ins.MedicalServicePoint},{ins.SubTotal},{ins.CopayMentPoint},{ins.DeclarePoint},{ins.PrescriptionCount}");
                }
                var sum = new InstitutionDeclarePoint
                {
                    InsName = "總計",
                    MedicinePoint = institutionDeclarePoints.Sum(ins => ins.MedicinePoint),
                    SpecialMedPoint = institutionDeclarePoints.Sum(ins => ins.SpecialMedPoint),
                    MedicalServicePoint = institutionDeclarePoints.Sum(ins => ins.MedicalServicePoint),
                    SubTotal = institutionDeclarePoints.Sum(ins => ins.SubTotal),
                    CopayMentPoint = institutionDeclarePoints.Sum(ins => ins.CopayMentPoint),
                    DeclarePoint = institutionDeclarePoints.Sum(ins => ins.DeclarePoint),
                    PrescriptionCount = institutionDeclarePoints.Sum(ins => ins.PrescriptionCount)
                };
                file.WriteLine($"{sum.InsName},{sum.MedicinePoint},{sum.SpecialMedPoint},{sum.MedicalServicePoint},{sum.SubTotal},{sum.CopayMentPoint},{sum.DeclarePoint},{sum.PrescriptionCount}");
                file.Close();
                file.Dispose();
            }
        }
    }
}