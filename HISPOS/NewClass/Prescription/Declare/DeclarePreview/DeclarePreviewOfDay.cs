using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePreview
{
    public class DeclarePreviewOfDay : ObservableObject
    {
        public DeclarePreviewOfDay()
        {
            PresOfDay = new DeclarePrescriptions();
        }

        #region Variables

        public string Day => Date.Day.ToString().PadLeft(2, '0');
        private int normalCount;

        public int NormalCount
        {
            get => normalCount;
            set
            {
                Set(() => NormalCount, ref normalCount, value);
            }
        }

        private int simpleFormCount;

        public int SimpleFormCount
        {
            get => simpleFormCount;
            set
            {
                Set(() => SimpleFormCount, ref simpleFormCount, value);
            }
        }

        private int chronicCount;

        public int ChronicCount
        {
            get => chronicCount;
            set
            {
                Set(() => ChronicCount, ref chronicCount, value);
            }
        }

        private int declareCount;

        public int DeclareCount
        {
            get => declareCount;
            set
            {
                Set(() => DeclareCount, ref declareCount, value);
            }
        }

        private int notDeclareCount;

        public int NotDeclareCount
        {
            get => notDeclareCount;
            set
            {
                Set(() => NotDeclareCount, ref notDeclareCount, value);
            }
        }

        private CollectionViewSource presOfDayViewSource;

        private CollectionViewSource PresOfDayViewSource
        {
            get => presOfDayViewSource;
            set
            {
                Set(() => PresOfDayViewSource, ref presOfDayViewSource, value);
            }
        }

        private ICollectionView presOfDayCollectionView;

        public ICollectionView PresOfDayCollectionView
        {
            get => presOfDayCollectionView;
            set
            {
                Set(() => PresOfDayCollectionView, ref presOfDayCollectionView, value);
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

        public DeclarePrescriptions PresOfDay { get; set; }

        private DeclareMedicalPersonnels declarePharmacists;

        public DeclareMedicalPersonnels DeclarePharmacists
        {
            get => declarePharmacists;
            set
            {
                Set(() => DeclarePharmacists, ref declarePharmacists, value);
            }
        }

        private bool isAdjustOutOfRange;

        public bool IsAdjustOutOfRange
        {
            get => isAdjustOutOfRange;
            set
            {
                Set(() => IsAdjustOutOfRange, ref isAdjustOutOfRange, value);
            }
        }

        private bool hasNotDeclare;

        public bool HasNotDeclare
        {
            get => hasNotDeclare;
            set
            {
                Set(() => HasNotDeclare, ref hasNotDeclare, value);
            }
        }

        private DateTime date;

        public DateTime Date
        {
            get => date;
            set
            {
                Set(() => Date, ref date, value);
            }
        }

        #endregion Variables

        public void AddPresOfDay(List<DeclarePrescription.DeclarePrescription> pres)
        {
            PresOfDay.Clear();
            foreach (var pre in pres)
            {
                PresOfDay.Add(pre);
            }
            PresOfDayViewSource = new CollectionViewSource { Source = PresOfDay };
            PresOfDayCollectionView = PresOfDayViewSource.View;
            Date = pres[0].AdjustDate;
            NormalCount = PresOfDay.Count(p => p.AdjustCase.ID.Equals("1"));
            SimpleFormCount = PresOfDay.Count(p => p.AdjustCase.ID.Equals("3"));
            ChronicCount = PresOfDay.Count(p => p.AdjustCase.ID.Equals("2"));
            DeclareCount = PresOfDay.Count(p => p.IsDeclare);
            NotDeclareCount = PresOfDay.Count(p => !p.IsDeclare);
            CheckAdjustOutOfRange();
            CountPharmacistsPrescription();
        }

        public void CheckNotDeclareCount()
        {
            HasNotDeclare = NotDeclareCount > 0;
        }

        public void CheckAdjustOutOfRange()
        {
            var tempList = PresOfDay.GroupBy(d => d.Pharmacist.ID);
            foreach (var g in tempList.Select(group => group).ToList())
            {
                if (g.Count(p => p.IsDeclare) > 80)
                {
                    IsAdjustOutOfRange = true;
                    break;
                }
                IsAdjustOutOfRange = false;
            }
        }

        public void CountPharmacistsPrescription()
        {
            DeclarePharmacists = new DeclareMedicalPersonnels();
            var tempList = new DeclareMedicalPersonnels();
            foreach (var g in PresOfDay.GroupBy(p => p.Pharmacist.ID))
            {
                var m = ViewModelMainWindow.GetEmployeeByID(g.Key);
                var pharmacist = new DeclareMedicalPersonnel(m) { PrescriptionCount = g.Count() };
                tempList.Add(pharmacist);
            }
            foreach (var pharmacist in tempList.OrderBy(p => p.Name))
            {
                DeclarePharmacists.Add(pharmacist);
            }
        }
    }
}