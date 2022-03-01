using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryManageDetails : ObservableCollection<ManufactoryManageDetail>
    {
        private ManufactoryManageDetails(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ManufactoryManageDetail(row));
            }
        }

        internal static ManufactoryManageDetails GetManufactoryManageDetailsBySearchCondition(string searchManufactoryName, string searchPrincipalName)
        {
            return new ManufactoryManageDetails(ManufactoryDB.GetManufactoryManageDetailsBySearchCondition(searchManufactoryName, searchPrincipalName));
        }
    }
}