using His_Pos.NewClass.Prescription.Treatment.Institution;
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
        public WareHouse.WareHouse GetWareHouseByPrescription(Institution ins, string adjcaseID) {
            if(ins is null) return ChromeTabViewModel.ViewModelMainWindow.GetWareHouse("0");
            var temp =  Items.SingleOrDefault(w => w.CooperavieClinic.ID == ins.ID);
            if (temp is null)
                return ChromeTabViewModel.ViewModelMainWindow.GetWareHouse("0");

            switch (adjcaseID) {
                case "2":
                    return temp.ChronicIsBuckle ? temp.ChronicWareHouse : null;
                default:
                    return temp.NormalIsBuckle ? temp.NormalWareHouse : null; 
            } 
        } 
    }
}
