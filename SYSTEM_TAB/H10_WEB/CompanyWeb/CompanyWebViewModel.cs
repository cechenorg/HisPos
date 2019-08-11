using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H10_WEB.CompanyWeb
{
    public class CompanyWebViewModel : TabBase {
        public override TabBase getTab()
        {
            return this;
        }
        public string Url { get; set; }
        public CompanyWebViewModel() {
            Url = $"http://singde.com.tw?id=" + ViewModelMainWindow.CurrentPharmacy.ID + "&pwd=" + ViewModelMainWindow.CurrentPharmacy.ID;
        }
    }
}
