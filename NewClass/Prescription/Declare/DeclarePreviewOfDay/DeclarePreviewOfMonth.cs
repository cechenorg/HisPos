using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePreviewOfDay
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

        private int totalCount;

        public int TotalCount
        {
            get => totalCount;
            set { Set(() => TotalCount, ref totalCount, value); }
        }

        private int totalPoint;

        public int TotalPoint
        {
            get => totalPoint;
            set { Set(() => TotalPoint, ref totalPoint, value); }
        }
        public DateTime DeclareDate { get; set; }

        internal void GetSearchPrescriptions(DateTime sDate, DateTime eDate,string pharmacyID)
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
            DecPreOfDaysViewSource = new CollectionViewSource {Source = DeclarePreviews};
            DecPreOfDaysCollectionView = DecPreOfDaysViewSource.View;
        }

        internal void SetSummary()
        {
            var normal = DeclarePres.Where(p => p.AdjustCase.ID.Equals("1") || p.AdjustCase.ID.Equals("3"));
            var chronic = DeclarePres.Where(p => p.AdjustCase.ID.Equals("2"));
            NotDeclareCount = DeclarePres.Count(p => !p.IsDeclare);
            NormalCount = normal.Count();
            NormalPoint = normal.Sum(p => p.ApplyPoint);
            ChronicCount = chronic.Count();
            ChronicPoint = chronic.Sum(p => p.ApplyPoint);
            TotalCount = DeclarePres.Count;
            TotalPoint = DeclarePres.Sum(p => p.ApplyPoint);
            foreach (var d in DeclarePreviews)
            {
                d.NormalCount = d.PresOfDay.Count(p => p.AdjustCase.ID.Equals("1"));
                d.ChronicCount = d.PresOfDay.Count(p => p.AdjustCase.ID.Equals("2"));
                d.SimpleFormCount = d.PresOfDay.Count(p => p.AdjustCase.ID.Equals("3"));
                d.DeclareCount = d.PresOfDay.Count(p => p.IsDeclare);
                d.NotDeclareCount = d.PresOfDay.Count(p => !p.IsDeclare);
                d.CheckNotDeclareCount();
            }
            
        }
        public void CreateDeclareFile(DeclareFile.DeclareFile doc)
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
            Function.ExportXml(result, "匯出申報XML檔案");
        }
    }
}
