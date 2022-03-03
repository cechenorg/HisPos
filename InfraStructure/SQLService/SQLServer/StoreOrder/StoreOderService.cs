using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace InfraStructure.SQLService.SQLServer.StoreOrder
{
    public class StoreOrderService : SQLServerServiceBase
    {
        public StoreOrderService(string connectionString):base(connectionString)
        { 
        }

        public DataTable Get_SingdeTotalOrdersNotDone()
        {
             
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string strSql = @"DECLARE @THREEDAYBEFORE NVARCHAR(8) = (SELECT CONVERT(NVARCHAR(8), GETDATE() - 3, 112));

                SET @THREEDAYBEFORE = (SELECT CAST(SUBSTRING(@THREEDAYBEFORE, 1, 4) - 1911 AS NVARCHAR(4)) + SUBSTRING(@THREEDAYBEFORE, 5, 4))

                SELECT DISTINCT SUBSTRING(StoOrd_ReceiveID, 1, 7) AS DATE
                INTO #TOTAL_ORDER
                FROM[StoreOrder].[Master]
                WHERE StoOrd_IsEnable = 1 AND StoOrd_ManufactoryID = 0 AND(StoOrd_Status = 'P' OR(StoOrd_Status = 'D' AND SUBSTRING(StoOrd_ReceiveID, 1, 7) >= @THREEDAYBEFORE))

                SELECT SUBSTRING(StoOrd_ReceiveID, 1, 7) AS DATE, SUM(IIF(StoOrd_Type = 'P', 1, 0)) P_COUNT, SUM(IIF(StoOrd_Type = 'R', 1, 0)) AS R_COUNT
                INTO #T
                FROM[StoreOrder].[Master] AS M
                WHERE StoOrd_IsEnable = 1 AND StoOrd_ManufactoryID = 0 AND SUBSTRING(StoOrd_ReceiveID, 1, 7) IN(SELECT DATE FROM #TOTAL_ORDER)
                GROUP BY SUBSTRING(StoOrd_ReceiveID, 1, 7)

                SELECT SUBSTRING(StoOrd_ReceiveID, 1, 7) AS DATE, P_COUNT, R_COUNT,
                    SUM(IIF(StoOrd_Type = 'P', StoOrdDet_SubTotal, 0)) AS P_TOTAL, SUM(IIF(StoOrd_Type = 'R', StoOrdDet_SubTotal, 0)) AS R_TOTAL
                FROM[StoreOrder].[Master] AS M JOIN[StoreOrder].[Detail] AS D ON M.StoOrd_ID = D.StoOrdDet_MasterID

                JOIN #T AS T ON T.DATE = SUBSTRING(StoOrd_ReceiveID, 1, 7)
                WHERE StoOrd_IsEnable = 1 AND StoOrd_ManufactoryID = 0 AND SUBSTRING(StoOrd_ReceiveID, 1, 7) IN(SELECT DATE FROM #TOTAL_ORDER)
                GROUP BY SUBSTRING(StoOrd_ReceiveID, 1, 7), P_COUNT, R_COUNT";

                var result = conn.Query(strSql);
                var json = JsonConvert.SerializeObject(result);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                return null;
            }
        }
    }
}
