using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Cooperative.CooperativeClinicSetting
{
    public class CooperativeClinicSettings : ObservableCollection<CooperativeClinicSetting>
    {
        public CooperativeClinicSettings() {

        }
        public void Init() {
            Clear();
            var table = CooperativeClinicSettingDb.Init();
            foreach (DataRow r in table.Rows) {
                Add(new CooperativeClinicSetting(r));
            } 
        }
        public void Update() {
            CooperativeClinicSettingDb.Update(this);
        }
    }
}
