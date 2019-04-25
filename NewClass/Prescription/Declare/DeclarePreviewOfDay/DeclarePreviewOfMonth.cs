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
    public class DeclarePreviewOfMonth:ObservableObject
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
            set
            {
                Set(() => DecPreOfDaysViewSource, ref decPreOfDaysViewSource, value);
            }
        }
        private ICollectionView decPreOfDaysCollectionView;
        public ICollectionView DecPreOfDaysCollectionView
        {
            get => decPreOfDaysCollectionView;
            set
            {
                Set(() => DecPreOfDaysCollectionView, ref decPreOfDaysCollectionView, value);
            }
        }
        private DeclarePreviewOfDay selectedDayPreview;
        private DeclarePreviewOfDay SelectedDayPreview
        {
            get => selectedDayPreview;
            set
            {
                Set(() => SelectedDayPreview, ref selectedDayPreview, value);
            }
        }
        public void GetSearchPrescriptions(DateTime sDate, DateTime eDate)
        {
            DeclarePreviews = new ObservableCollection<DeclarePreviewOfDay>();
            DeclarePres = new DeclarePrescriptions();
            DeclarePres.GetSearchPrescriptions(sDate,eDate);
            foreach (var pres in DeclarePres.OrderBy(p => p.AdjustDate).GroupBy(p => p.AdjustDate).Select(grp => grp.ToList()).ToList())
            {
                var preview = new DeclarePreviewOfDay();
                preview.AddPresOfDay(pres);
                DeclarePreviews.Add(preview);
            }
            DecPreOfDaysViewSource = new CollectionViewSource {Source = DeclarePreviews};
            DecPreOfDaysCollectionView = DecPreOfDaysViewSource.View;
        }
    }
}
