using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Invoice.InvoiceSetting
{
    public class InvoiceSettings : ObservableCollection<InvoiceSetting>
    {
        public InvoiceSettings() {

        }
        public void Init() {
            Clear();
            var table = InvoiceSettingDb.Init();
            foreach (DataRow r in table.Rows) {
                Add(new InvoiceSetting(r));
            } 
        }
        public void Update() {
            InvoiceSettingDb.Update(this);
        } 
      
        
    }
}
