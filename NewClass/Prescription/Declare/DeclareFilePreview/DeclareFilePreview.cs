using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.RDLC;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFilePreview
{
    public class DeclareFilePreview:ObservableObject
    {
        public DeclareFilePreview()
        {
            DeclarePrescriptions = new DeclarePrescriptions();
        }
        public DateTime Date { get; set; }
        public int NormalCount { get; set; }
        public int ChronicCount { get; set; }
        public int SimpleFormCount { get; set; }
        public int DeclareCount { get; set; }
        public int NotDeclareCount { get; set; }
        public int Day => Date.Day;
        private CollectionViewSource decPresViewSource;
        private CollectionViewSource DecPresViewSource
        {
            get => decPresViewSource;
            set
            {
                Set(() => DecPresViewSource, ref decPresViewSource, value);
            }
        }
        private ICollectionView decPresViewCollectionView;
        public ICollectionView DecPresViewCollectionView
        {
            get => decPresViewCollectionView;
            set
            {
                Set(() => DecPresViewCollectionView, ref decPresViewCollectionView, value);
            }
        }

        private DeclarePrescription.DeclarePrescription selectedPrescription;

        public DeclarePrescription.DeclarePrescription SelectedPrescription
        {
            get => selectedPrescription;
            set
            {
                Set(() => SelectedPrescription, ref selectedPrescription, value);
            }
        }
        public DeclarePrescriptions DeclarePrescriptions { get; set; }

        public void SetSummary()
        {
            DecPresViewSource = new CollectionViewSource { Source = DeclarePrescriptions };
            DecPresViewCollectionView = DecPresViewSource.View;
            if (DecPresViewCollectionView.Cast<DeclarePrescription.DeclarePrescription>().ToList().Count > 0)
            {
                DecPresViewCollectionView.MoveCurrentToFirst();
                SelectedPrescription = DecPresViewCollectionView.CurrentItem.Cast<DeclarePrescription.DeclarePrescription>();
            }
            Date = DeclarePrescriptions[0].AdjustDate;
            NormalCount = DeclarePrescriptions.Count(p =>
                (p.AdjustCase.ID.Equals("1") || p.AdjustCase.ID.Equals("5") || p.AdjustCase.ID.Equals("D")) && p.IsDeclare);
            SimpleFormCount = DeclarePrescriptions.Count(p=>p.AdjustCase.ID.Equals("3") && p.IsDeclare);
            ChronicCount = DeclarePrescriptions.Count(p => p.AdjustCase.ID.Equals("2") && p.IsDeclare);
            DeclareCount = DeclarePrescriptions.Count(p => p.IsDeclare);
            NotDeclareCount = DeclarePrescriptions.Count(p => !p.IsDeclare);
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
            var declareFileId = DeclareFileDb.InsertDeclareFile(result, this).Rows[0].Field<int>("DecFile_ID");
            var declareList = DeclarePrescriptions.Where(p => p.IsDeclare).Select(p => p.ID).ToList();
            DeclarePrescriptionDb.UpdateDeclareFileID(declareFileId, declareList);
            //匯出xml檔案
            Function.ExportXml(result, "匯出申報XML檔案");
        }
    }
}
