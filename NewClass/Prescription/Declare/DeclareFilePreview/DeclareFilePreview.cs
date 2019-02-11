using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Person;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using AdjustCase = His_Pos.NewClass.Prescription.Treatment.AdjustCase.AdjustCase;
using MedicalPersonnel = His_Pos.NewClass.Person.MedicalPerson.MedicalPersonnel;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFilePreview
{
    public class DeclareFilePreview:ObservableObject
    {
        public DeclareFilePreview()
        {

        }
        public int DeclareYear { get; set; }
        public int DeclareMonth { get; set; }
        public int NormalCount { get; set; }
        public int ChronicCount { get; set; }
        public int SimpleFormCount { get; set; }
        public int TotalPoint { get; set; }
        public string PharmacyID { get; set; }
        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private Person.MedicalPerson.MedicalPersonnel selectedPharmacist;
        public MedicalPersonnel SelectedPharmacist
        {
            get => selectedPharmacist;
            set
            {
                Set(() => SelectedPharmacist, ref selectedPharmacist, value);
            }
        }

        private Treatment.AdjustCase.AdjustCase selectedAdjustCase;
        public AdjustCase SelectedAdjustCase
        {
            get => selectedAdjustCase;
            set
            {
                Set(() => SelectedAdjustCase, ref selectedAdjustCase, value);
            }
        }

        private Institution selectedInstitution;
        public Institution SelectedInstitution
        {
            get => selectedInstitution;
            set
            {
                Set(() => SelectedInstitution, ref selectedInstitution, value);
            }
        }
        private CollectionViewSource prescriptionsViewSource;
        private CollectionViewSource PrescriptionsViewSource
        {
            get => prescriptionsViewSource;
            set
            {
                Set(() => PrescriptionsViewSource, ref prescriptionsViewSource, value);
            }
        }

        private ICollectionView prescriptionsCollectionView;
        public ICollectionView PrescriptionsCollectionView
        {
            get => prescriptionsCollectionView;
            private set
            {
                Set(() => PrescriptionsCollectionView, ref prescriptionsCollectionView, value);
            }
        }
        public Prescriptions DeclarePrescriptions { get; set; }
    }
}
