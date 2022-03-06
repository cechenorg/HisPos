using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryManageDetails : ObservableCollection<ManufactoryManageDetail>
    {
        public ManufactoryManageDetails(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ManufactoryManageDetail(row));
            }
        }
         
    }
}