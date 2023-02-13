using Dapper;
using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceDTO;

namespace His_Pos.InfraStructure
{
    internal class CommonDataRepository
    {
        private readonly string schemeName = Properties.Settings.Default.SystemSerialNumber;
        public CommonDataRepository()
        {

        }
        public void SyncNHISpecialMedicine(List<NHISpecialMedicineDTO> data)
        {
            SQLServerConnection.DapperQuery( (conn) =>
            {
                try
                {
                    conn.Execute($"{schemeName}.[DataSource].[SyncSpecialMedicine]",
                        param: new { SpecialMedicine = ConvertToDataTable(data) },
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
              
            });
        }

        private async Task SyncSmokeMedicines()
        {

        }

        private async Task SyncMedicines()
        {
        }

        private async Task SyncInstitutions()
        {
        }

        private async Task SyncDivisions()
        {
        }

        private async Task SyncDiseasesCode()
        {
        }

        private async Task SyncAdjustCase()
        {
        }

        public System.Data.DataTable ConvertToDataTable<T>(IList<T> data)

        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            System.Data.DataTable table = new System.Data.DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)

            {

                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                }

                table.Rows.Add(row);
            }

            return table;

        }
    }
}
