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
using AdjustCase = His_Pos.NewClass.Prescription.Treatment.AdjustCase.AdjustCase;
using MedicalPersonnel = His_Pos.NewClass.Person.MedicalPerson.MedicalPersonnel;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFilePreview
{
    public class DeclareFilePreview:ObservableObject
    {
        public DeclareFilePreview()
        {
            DeclarePrescriptions = new DeclarePrescriptions();
        }
        public int DeclareYear { get; set; }
        public int DeclareMonth { get; set; }
        public int NormalCount { get; set; }
        public int ChronicCount { get; set; }
        public int SimpleFormCount { get; set; }
        public int TotalPoint { get; set; }
        public string PharmacyID { get; set; }
        public int DeclareCount { get; set; }
        public int NotDeclareCount { get; set; }
        public int NotGetCardCount { get; set; }
        public int TotalCount { get; set; }
        private int? startDate;
        public int? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
                DecPresViewSource.Filter += Filter;
            }
        }

        private int? endDate;
        public int? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
                DecPresViewSource.Filter += Filter;
            }
        }

        private MedicalPersonnel selectedPharmacist;
        public MedicalPersonnel SelectedPharmacist
        {
            get => selectedPharmacist;
            set
            {
                Set(() => SelectedPharmacist, ref selectedPharmacist, value);
                DecPresViewSource.Filter += Filter;
            }
        }

        private AdjustCase selectedAdjustCase;
        public AdjustCase SelectedAdjustCase
        {
            get => selectedAdjustCase;
            set
            {
                Set(() => SelectedAdjustCase, ref selectedAdjustCase, value);
                DecPresViewSource.Filter += Filter;
            }
        }

        private Institution selectedInstitution;
        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set
            {
                Set(() => SelectedInstitution, ref selectedInstitution, value);
                DecPresViewSource.Filter += Filter;
            }
        }
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
            DeclareYear = DeclarePrescriptions[0].AdjustDate.Year;
            DeclareMonth = DeclarePrescriptions[0].AdjustDate.Month;
            PharmacyID = DeclarePrescriptions[0].PharmacyID;
            NormalCount = DeclarePrescriptions.Count(p =>
                (p.AdjustCase.Id.Equals("1") || p.AdjustCase.Id.Equals("5") || p.AdjustCase.Id.Equals("D")) && p.IsDeclare);
            SimpleFormCount = DeclarePrescriptions.Count(p=>p.AdjustCase.Id.Equals("3") && p.IsDeclare);
            ChronicCount = DeclarePrescriptions.Count(p => p.AdjustCase.Id.Equals("2") && p.IsDeclare);
            TotalPoint = DeclarePrescriptions.Where(p=>p.IsDeclare).Sum(p => p.TotalPoint);
            DeclareCount = DeclarePrescriptions.Count(p => p.IsDeclare);
            NotDeclareCount = DeclarePrescriptions.Count(p => !p.IsDeclare);
            NotGetCardCount = DeclarePrescriptions.Count(p => !p.IsGetCard);
            TotalCount = DeclarePrescriptions.Count;
            var firstDay = new DateTime(DeclareYear, DeclareMonth, 1);
            var lastDay = DeclareMonth == DateTime.Today.Month ? DateTime.Today : new DateTime(DeclareYear, DeclareMonth + 1, 1).AddDays(-1);
            StartDate = firstDay.Day;
            EndDate = lastDay.Day;
        }
        private void Filter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is DeclarePrescription.DeclarePrescription src))
                e.Accepted = false;
            else if (StartDate is null || src.AdjustDate.Date.Day >= StartDate)
            {
                if (EndDate is null || src.AdjustDate.Date.Day <= EndDate)
                {
                    if (SelectedPharmacist is null || src.Pharmacist.IdNumber.Equals(SelectedPharmacist.IdNumber))
                    {
                        if (SelectedAdjustCase is null || src.AdjustCase.Id.Equals(SelectedAdjustCase.Id))
                        {
                            if (SelectedInstitution is null || src.Institution.Id.Equals(SelectedInstitution.Id))
                                e.Accepted = true;
                            else
                                e.Accepted = false;
                        }
                        else
                            e.Accepted = false;
                    }
                    else
                        e.Accepted = false;
                }
                else
                    e.Accepted = false;
            }
            else
                e.Accepted = false;
        }

        public void CreateDeclareFile(DeclareFile.DeclareFile doc)
        {
            XDocument result;
            var xmlSerializer = new XmlSerializer(doc.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, doc);
                var document = XDocument.Parse(ReportService.PrettyXml(textWriter));
                var root = XElement.Parse(document.ToString());
                root.Element("ddata")?.Element("decId")?.Remove();
                document = XDocument.Load(root.CreateReader());
                document.Root?.RemoveAttributes();
                result = document;
            }
            //var declareFileId = DeclareFileDb.InsertDeclareFile(result, this).Rows[0].Field<int>("DecFile_ID");
            var presIDList = new List<int>();
            foreach (var p in DeclarePrescriptions)
            {
                presIDList.Add(p.ID);
            }
            //DeclarePrescriptionDb.UpdateDeclareFileID(declareFileId,presIDList);
            //匯出xml檔案
            Function.ExportXml(result, "匯出申報XML檔案");
        }

        public bool CheckFileExist()
        {
            return DeclareFileDb.CheckFileExist(PharmacyID,new DateTime(DeclareYear,DeclareMonth,1)).Rows[0].Field<bool>("");
        }
    }
}
