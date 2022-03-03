using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Invoice.InvoiceSetting
{
    public class InvoiceSettings : ObservableCollection<InvoiceSetting>
    {
        public InvoiceSettings()
        {
        }

        public void Init()
        {
            Clear();
            var table = InvoiceSettingDb.Init();
            foreach (DataRow r in table.Rows)
            {
                Add(new InvoiceSetting(r));
            }
        }

        public void Update()
        {
            InvoiceSettingDb.Update(this);
        }
    }
}