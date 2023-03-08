using Dapper;
using His_Pos.Database;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.InfraStructure
{
    public class AccountService
    {
        public IEnumerable<AccountsCommonItem> GetAccountCommonItems(string acctName)
        {

            IEnumerable<AccountsCommonItem> result = default;

            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<AccountsCommonItem>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[AccountsCommonItems]",
                    param: new
                    {
                        Item = acctName
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }
    }
}
