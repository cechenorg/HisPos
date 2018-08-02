﻿using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Pharmacy
{
    public static class PharmacyDb
    {
        internal static ObservableCollection<ManagePharmacy> GetManagePharmacy()
        {
            ObservableCollection<ManagePharmacy> collection = new ObservableCollection<ManagePharmacy>();

            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[PharmacyManageView].[GetManagePharmacy]");

            foreach (DataRow row in table.Rows)
            {
                string parent = row["MAN_PARENT"].ToString();

                if (parent.Equals(String.Empty))
                {
                    collection.Add(new ManagePharmacy(row));
                }
                else
                {
                    ManagePharmacy phar = collection.Where(m => m.Id.Equals(parent)).ToList()[0];

                    phar.PharmacyPrincipals.Add(new PharmacyPrincipal(row));
                }
            }

            return collection;
        }

        internal static ManagePharmacy AddNewManagePharmacy()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[PharmacyManageView].[AddNewManagePharmacy]");

            return new ManagePharmacy(table.Rows[0]);
        }

        internal static void UpdateManagePharmacy(ManagePharmacy pharmacy)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", pharmacy.Id));
            parameters.Add(new SqlParameter("NAME", pharmacy.Name));
            parameters.Add(new SqlParameter("NICKNAME", pharmacy.NickName));
            parameters.Add(new SqlParameter("ADDR", pharmacy.Address));
            parameters.Add(new SqlParameter("TEL", pharmacy.Telphone));
            parameters.Add(new SqlParameter("FAX", pharmacy.Fax));
            parameters.Add(new SqlParameter("EIN", pharmacy.UniformNumber));
            parameters.Add(new SqlParameter("EMAIL", pharmacy.Email));
            parameters.Add(new SqlParameter("CONTROL", DBNull.Value));
            parameters.Add(new SqlParameter("MEDICAL", pharmacy.MedicalID));
            parameters.Add(new SqlParameter("NOTE", pharmacy.Note));
            parameters.Add(new SqlParameter("PARENT", DBNull.Value));
            parameters.Add(new SqlParameter("RESDEP", DBNull.Value));
            parameters.Add(new SqlParameter("LINE", DBNull.Value));
            parameters.Add(new SqlParameter("PAYCONDITION", DBNull.Value));
            parameters.Add(new SqlParameter("PAYTYPE", DBNull.Value));
            parameters.Add(new SqlParameter("ISENABLE", 1));

            dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[UpdateManageManufactory]", parameters);

            foreach (var principal in pharmacy.PharmacyPrincipals)
            {
                parameters.Clear();
                if (principal.Id.Equals(""))
                    parameters.Add(new SqlParameter("ID", DBNull.Value));
                else
                    parameters.Add(new SqlParameter("ID", principal.Id));
                parameters.Add(new SqlParameter("NAME", principal.Name));
                parameters.Add(new SqlParameter("NICKNAME", principal.NickName));
                parameters.Add(new SqlParameter("ADDR", DBNull.Value));
                parameters.Add(new SqlParameter("TEL", principal.Telphone));
                parameters.Add(new SqlParameter("FAX", principal.Fax));
                parameters.Add(new SqlParameter("EIN", DBNull.Value));
                parameters.Add(new SqlParameter("EMAIL", principal.Email));
                parameters.Add(new SqlParameter("CONTROL", DBNull.Value));
                parameters.Add(new SqlParameter("MEDICAL", DBNull.Value));
                parameters.Add(new SqlParameter("NOTE", DBNull.Value));
                parameters.Add(new SqlParameter("PARENT", pharmacy.Id));
                parameters.Add(new SqlParameter("RESDEP", DBNull.Value));
                parameters.Add(new SqlParameter("LINE", principal.Line));
                parameters.Add(new SqlParameter("PAYCONDITION", DBNull.Value));
                parameters.Add(new SqlParameter("PAYTYPE", DBNull.Value));
                parameters.Add(new SqlParameter("ISENABLE", principal.IsEnable));

                dd.ExecuteProc("[HIS_POS_DB].[ManufactoryManageView].[UpdateManageManufactory]", parameters);
            }
        }
    }
}
