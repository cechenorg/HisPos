using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Prescription.Declare.DeclareFilePreview;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
// ReSharper disable InconsistentNaming

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage
{
    public class DeclareFileManageViewModel:TabBase
    {
        #region Variables
        public override TabBase getTab()
        {
            return this;
        }
        private CollectionViewSource decFilePreViewSource;
        private CollectionViewSource DecFilePreViewSource
        {
            get => decFilePreViewSource;
            set
            {
                Set(() => DecFilePreViewSource, ref decFilePreViewSource, value);
            }
        }

        private ICollectionView decFilePreViewCollectionView;
        public ICollectionView DecFilePreViewCollectionView
        {
            get => decFilePreViewCollectionView;
            private set
            {
                Set(() => DecFilePreViewCollectionView, ref decFilePreViewCollectionView, value);
            }
        }

        private DeclareFilePreviews decFilePreViews;

        public DeclareFilePreviews DecFilePreViews
        {
            get => decFilePreViews;
            private set
            {
                Set(() => DecFilePreViews, ref decFilePreViews, value);
            }
        }
        public MedicalPersonnels MedicalPersonnels { get; set; }
        public AdjustCases AdjustCases { get; set; }
        private DateTime? decStart;
        public DateTime? DecStart
        {
            get => decStart;
            set
            {
                Set(() => DecStart, ref decStart, value);
            }
        }
        private DateTime? decEnd;
        public DateTime? DecEnd
        {
            get => decEnd;
            set
            {
                Set(() => DecEnd, ref decEnd, value);
            }
        }
        #endregion
        public DeclareFileManageViewModel()
        {
            DecEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DecStart = new DateTime(((DateTime)DecEnd).Year, ((DateTime)DecEnd).Month, 1).AddMonths(-3);
        }
    }
}
