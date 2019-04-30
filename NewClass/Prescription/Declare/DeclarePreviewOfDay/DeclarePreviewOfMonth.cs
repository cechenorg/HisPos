using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Declare.DeclarePrescription;

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

        internal void GetSearchPrescriptions(DateTime sDate, DateTime eDate)
        {
            DeclarePreviews = new ObservableCollection<DeclarePreviewOfDay>();
            DeclarePres = new DeclarePrescriptions();
            DeclarePres.GetSearchPrescriptions(sDate, eDate);
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
        }
    }
}
