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
using DomainModel;
using WebServiceDTO;
using DocumentFormat.OpenXml.Drawing.Charts;

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
            ExecProc(data, $"{schemeName}.[DataSource].[SyncSpecialMedicine]");
        }

        public void SyncSmokeMedicines(List<NHISmokeMedicineDTO> data)
        {
            ExecProc(data, $"{schemeName}.[DataSource].[SyncSmokeMedicines]");
        }

        public void SyncMedicines(List<NHIMedicineDTO> data)
        {
            ExecProc(data, $"{schemeName}.[DataSource].[SyncMedicines]");
        }

        public void SyncInstitutions(List<InstitutionDTO> data)
        {
            ExecProc(data, $"{schemeName}.[DataSource].[SyncInstitutions]");
        }

        public void SyncDivisions(List<DivisionDTO> data)
        {
            ExecProc(data, $"{schemeName}.[DataSource].[SyncDivisions]");
        }

        public void SyncDiseasesCode(List<DiseaseCodeDTO> data)
        {
            ExecProc(data, $"{schemeName}.[DataSource].[SyncDiseasesCode]");
        }

        public void SyncAdjustCase(List<AdjustCaseDTO> data)
        {
            ExecProc(data, $"{schemeName}.[DataSource].[SyncAdjustCase]");
        }

        private void ExecProc<T>(List<T> data, string spName)
        {
            SQLServerConnection.DapperQuery((conn) =>
            {
                try
                {
                    conn.Execute(spName,
                        param: new { SpecialMedicine = data.ConvertToDataTable() },
                        commandType: CommandType.StoredProcedure);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            });
        }

       
    }
}
