using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePreviewOfDay
{
    public class DeclarePreviewOfDay:ObservableObject
    {
        public DeclarePreviewOfDay()
        {
            PresOfDay = new DeclarePrescriptions();
        }
        #region Variables
        private string day;
        public string Day
        {
            get => day;
            set
            {
                Set(() => Day, ref day, value);
            }
        }
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
        #endregion

        public void AddPresOfDay(List<DeclarePrescription.DeclarePrescription> pres)
        {
            PresOfDay.Clear();
            foreach (var pre in pres)
            {
                PresOfDay.Add(pre);
            }
            PresOfDayViewSource = new CollectionViewSource { Source = PresOfDay };
            PresOfDayCollectionView = PresOfDayViewSource.View;
            Day = pres[0].AdjustDate.Day.ToString().PadLeft(2,'0');
            NormalCount = PresOfDay.Count(p => p.AdjustCase.ID.Equals("1"));
            SimpleFormCount = PresOfDay.Count(p => p.AdjustCase.ID.Equals("3"));
            ChronicCount = PresOfDay.Count(p => p.AdjustCase.ID.Equals("2"));
            DeclareCount = PresOfDay.Count(p => p.IsDeclare);
            NotDeclareCount = PresOfDay.Count(p => !p.IsDeclare);
        }
    }
}
