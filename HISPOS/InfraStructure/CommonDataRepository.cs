﻿using Dapper;
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
            ExecProc( $"{schemeName}.[DataSource].[SyncSpecialMedicine]", 
                new[]{new { SpecialMedicine = data.ConvertToDataTable() } });
        }

        public void SyncSmokeMedicines(List<NHISmokeMedicineDTO> data)
        {
            ExecProc($"{schemeName}.[DataSource].[SyncSmokeMedicines]",
                new[] { new { SmokeMedicine = data.ConvertToDataTable() } });
        }

        public void SyncMedicines(List<NHIMedicineDTO> data)
        {
            ExecProc($"{schemeName}.[DataSource].[SyncMedicines]",
                new[] { new { Medicine = data.ConvertToDataTable() } });
        }

        public void SyncInstitutions(List<InstitutionDTO> data)
        {
            ExecProc($"{schemeName}.[DataSource].[SyncInstitutions]",
                new[] { new { Institutions = data.ConvertToDataTable() } });
        }

        public void SyncDivisions(List<DivisionDTO> data)
        {
            ExecProc($"{schemeName}.[DataSource].[SyncDivisions]", 
                new[] { new { Divisions = data.ConvertToDataTable() } });
        }

        public void SyncDiseasesCode(List<DiseaseCodeDTO> data)
        {
            ExecProc($"{schemeName}.[DataSource].[SyncDiseasesCode]",
                new[] { new { DiseasesCodes = data.ConvertToDataTable() } });
        }

        public void SyncAdjustCase(List<AdjustCaseDTO> data)
        {
            ExecProc($"{schemeName}.[DataSource].[SyncAdjustCase]",
                new[] { new { AdjustCases = data.ConvertToDataTable() } });
        }

        private void ExecProc( string spName,dynamic[] paramemter)
        {
            SQLServerConnection.DapperQuery((conn) =>
            {
                try
                {
                    conn.Execute(spName,
                         param: paramemter ,
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
