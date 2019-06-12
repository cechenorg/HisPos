using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.AccountReport.InstitutionDeclarePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.InstitutionDeclarePointReport
{
   public class InstitutionDeclarePointReportViewModel : TabBase
    {
        #region Var
        public override TabBase getTab()
        {
            return this;
        }

        private DateTime searchDate ;
        public DateTime SearchDate
        {
            get => searchDate;
            set
            {
                Set(() => SearchDate, ref searchDate, value);
            }
        }
        private InstitutionDeclarePoints institutionDeclarePointCollection = new InstitutionDeclarePoints();
        public InstitutionDeclarePoints InstitutionDeclarePointCollection
        {
            get => institutionDeclarePointCollection;
            set
            {
                Set(() => InstitutionDeclarePointCollection, ref institutionDeclarePointCollection, value);
            }
        }
        public RelayCommand SearchCommand { get; set; }
        #endregion
        public InstitutionDeclarePointReportViewModel() {
            SearchDate = DateTime.Today.AddMonths(-1);
            InstitutionDeclarePointCollection.GetDataByDate(SearchDate);
            SearchCommand = new RelayCommand(SearchAction);
        }
        #region Function
        private void SearchAction() { 
            InstitutionDeclarePointCollection.GetDataByDate(SearchDate);
        }
        #endregion
    }
}
