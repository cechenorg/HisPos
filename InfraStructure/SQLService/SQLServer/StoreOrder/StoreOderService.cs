using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using DomainModel.Class.StoreOrder;
using Newtonsoft.Json;

namespace InfraStructure.SQLService.SQLServer.StoreOrder
{
    public class StoreOrderService : SQLServerServiceBase
    {
        public StoreOrderService(string connectionString, string dataTable) : base(connectionString, dataTable)
        {
        }

        public List<dSingdeTotalOrder> Get_SingdeTotalOrdersNotDone()
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

                SELECT SUBSTRING(StoOrd_ReceiveID, 1, 7) AS Date,  P_COUNT As PurchaseCount,R_COUNT As ReturnCount,
                    SUM(IIF(StoOrd_Type = 'P', StoOrdDet_SubTotal, 0)) AS PurchasePrice, SUM(IIF(StoOrd_Type = 'R', StoOrdDet_SubTotal, 0)) AS ReturnPrice
                FROM[StoreOrder].[Master] AS M JOIN[StoreOrder].[Detail] AS D ON M.StoOrd_ID = D.StoOrdDet_MasterID

                JOIN #T AS T ON T.DATE = SUBSTRING(StoOrd_ReceiveID, 1, 7)
                WHERE StoOrd_IsEnable = 1 AND StoOrd_ManufactoryID = 0 AND SUBSTRING(StoOrd_ReceiveID, 1, 7) IN(SELECT DATE FROM #TOTAL_ORDER)
                GROUP BY SUBSTRING(StoOrd_ReceiveID, 1, 7), P_COUNT, R_COUNT";

                var result = conn.Query<dSingdeTotalOrder>(strSql);

                return result.ToList();
            }
        }

		//傳送常備藥品訂單
		public void Set_InsertStoreOrderCommonMedicine(int employeeID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string strSql = $@"DECLARE @PRO [HIS].[MedicineListTable];
	            INSERT INTO @PRO
	            SELECT Pro_ID FROM Product.Master 
	            where Pro_TypeID<>2;
	            
	            CREATE TABLE #MEDBAG_TEMP (Inv_ID INT, Inv_Inventory FLOAT, Inv_OnTheWay FLOAT, Inv_MedBagOnTheWay FLOAT, MegBagAmount FLOAT, OnTheFrame FLOAT)
	            INSERT INTO #MEDBAG_TEMP 
	            EXEC [Get].[AllInventoryByProIDs] @PRO, 0
	            
	            Select Distinct * 
	            INTO #PURCHASERETURN_TEMP
	            From
	            (
	             select Pro_ID,
	            		Pro_ChineseName PRO_NAME,
	            		OnTheFrame INVENTORY,
	            		I.Inv_OnTheWay As ONTHEWAY,
	            		Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay,0) As NEED_VALUE,
	            		Case When Cast(Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay,0) as int) % Pro_MinOrder > 0 Then ((Cast(Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay,0) as int) / Pro_MinOrder) + 1) * Pro_MinOrder
	            		Else Cast(Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay, 0) as int) End  As PURCHASE_VALUE,
	            		0 As RETURN_VALUE
	            FROM
	            	Product.Master AS P JOIN [Product].[ProductInventory] PI ON P.Pro_ID = PI.ProInv_ProductID AND ProInv_MainFlag = 1 AND ProInv_WareHouseID = 0 AND Pro_IsCommon = 1
	            	JOIN Product.Inventory AS I ON PI.ProInv_InventoryID = I.Inv_ID
	            	JOIN #MEDBAG_TEMP AS M ON M.Inv_ID = I.Inv_ID
	            WHERE (OnTheFrame + I.Inv_OnTheWay) < Inv_SafeAmount
	            )T
	            
	            DECLARE @ITEM_COUNT INT;
	            DECLARE @STOORD_COUNT NVARCHAR(3);
	            DECLARE @DATE NVARCHAR(10) = CONVERT(NVARCHAR,GETDATE(),112);
	            DECLARE @NEWSTOORD_ID NVARCHAR(13);
	            
	            SET @ITEM_COUNT = (SELECT COUNT(*) FROM #PURCHASERETURN_TEMP WHERE PURCHASE_VALUE > 0)
	            
	            IF @ITEM_COUNT > 0
	            BEGIN
	            	SET @STOORD_COUNT = (SELECT TOP(1) FORMAT(SUBSTRING(StoOrd_ID, 11, 2) + 1, '-00') FROM [StoreOrder].[Master] WHERE CONVERT(DATE, GETDATE()) = CONVERT(DATE, StoOrd_CreateTime) ORDER BY SUBSTRING(StoOrd_ID, 11, 2) DESC);
	            	SET @NEWSTOORD_ID =  'P' + @DATE + ISNULL(@STOORD_COUNT, '-00');
	            
	            	INSERT INTO [StoreOrder].[Master](StoOrd_ID,StoOrd_ReceiveID, StoOrd_OrderEmployeeID, StoOrd_CreateTime, StoOrd_ManufactoryID, StoOrd_Status, StoOrd_Type, StoOrd_WarehouseID,[StoOrd_IsOTCType])
	            	VALUES(@NEWSTOORD_ID,@NEWSTOORD_ID, {employeeID}, GETDATE(), 0, 'U', 'P', 0,'藥品');
	            
	            	INSERT INTO [StoreOrder].[Detail] (StoOrdDet_MasterID, StoOrdDet_ProductID, StoOrdDet_ID, StoOrdDet_OrderAmount, StoOrdDet_UnitName, StoOrdDet_UnitAmount, StoOrdDet_Price, StoOrdDet_SubTotal, [StoOrdDet_FreeAmount], [StoOrdDet_RealAmount])
	            	SELECT @NEWSTOORD_ID, PRO_ID, ROW_NUMBER() OVER (ORDER BY PRO_ID), PURCHASE_VALUE, '基本單位', 1, ISNULL(CASE WHEN PURCHASE_VALUE > [bag_fact] THEN [jn_price] ELSE [ja_price] END, 0), ISNULL(CASE WHEN PURCHASE_VALUE > [bag_fact] THEN [jn_price] ELSE [ja_price] END, 0) * PURCHASE_VALUE, 0, 0
	            	FROM #PURCHASERETURN_TEMP AS T LEFT JOIN [HIS_POS_Server].[DataSource].[ProductFromSingde] AS S ON S.code = T.PRO_ID
	            	WHERE PURCHASE_VALUE > 0
	            
	            	SELECT StoOrd_ID, StoOrd_ReceiveID, E.Emp_Name AS OrderEmp_Name, EE.Emp_Name AS RecEmp_Name, M.Man_ID, M.Man_Name, M.Man_NickName, M.Man_Telephone,
	            		   StoOrd_Status, StoOrd_Type, W.War_ID, W.War_Name, StoOrd_Note, s.StoOrd_CustomerName, s.StoOrd_PlanArrivalDate,
	            		   '' AS CUS_DATA, [StoOrd_TargetCustomerName],isnull([StoOrd_IsOTCType],'藥品')[StoOrd_IsOTCType],
	            		   ISNULL(D.ProductCount, 0) AS ProductCount, ISNULL(D.Total, 0) AS Total, CASE WHEN StoOrd_Status = 'U' THEN 1 WHEN StoOrd_Status = 'W' THEN 2 ELSE 3 END AS OrderKey, S.StoOrd_ReceiveTime, StoOrd_CreateTime,isnull([StoOrd_IsOTCType],'no'),StoOrd_IsPayCash
	            	FROM [StoreOrder].[Master] AS S JOIN [Manufactory].[Master] AS M ON S.StoOrd_ManufactoryID = M.Man_ID
	            		 JOIN [Employee].[Master] AS E ON S.StoOrd_OrderEmployeeID = E.Emp_ID
	            		 JOIN [Product].[Warehouse] AS W ON S.StoOrd_WarehouseID = W.War_ID
	            		 LEFT JOIN [Employee].[Master] AS EE ON S.StoOrd_ReceiveEmployeeID = EE.Emp_ID
	            		 LEFT JOIN (SELECT [StoOrdDet_MasterID], SUM([StoOrdDet_SubTotal]) AS Total, COUNT(*) AS ProductCount FROM [StoreOrder].[Detail] GROUP BY [StoOrdDet_MasterID]) AS D ON D.StoOrdDet_MasterID = S.StoOrd_ID
	            		 LEFT JOIN [HIS].[PrescriptionMaster] AS P ON S.StoOrd_PrescriptionID = P.PreMas_ID
	            		 LEFT JOIN [HIS_POS_Server].[DataSource].[Institution] AS I ON I.Ins_ID = P.PreMas_InstitutionID
	            		 LEFT JOIN [HIS_POS_Server].[DataSource].[Division] AS DI ON DI.Div_ID = P.PreMas_DivisionID
	            		 LEFT JOIN [Customer].[Master] AS C ON P.PreMas_CustomerID = C.Cus_ID
	            	WHERE StoOrd_ID = @NEWSTOORD_ID
	            	ORDER BY OrderKey, StoOrd_ReceiveID;
	            	RETURN
	            END
	            
	            SELECT '' AS StoOrdID";

                conn.Execute(strSql);
				  
            }
        }

		//傳送常備OTC訂單
		public void Set_InsertStoreOrderOTCMedicine(int employeeId)
		{

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				string strSql = $@"DECLARE @PRO [HIS].[MedicineListTable];
				INSERT INTO @PRO
				SELECT Pro_ID FROM Product.Master 
				where Pro_TypeID=2;

				CREATE TABLE #MEDBAG_TEMP (Inv_ID INT, Inv_Inventory FLOAT, Inv_OnTheWay FLOAT, Inv_MedBagOnTheWay FLOAT, MegBagAmount FLOAT, OnTheFrame FLOAT)
				INSERT INTO #MEDBAG_TEMP 
				EXEC [Get].[AllInventoryByProIDs] @PRO, 0

				 select Pro_ID,
						Pro_ChineseName PRO_NAME,
						OnTheFrame INVENTORY,
						I.Inv_OnTheWay As ONTHEWAY,
						Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay,0) As NEED_VALUE,
						Case When Cast(Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay,0) as int) % Pro_MinOrder > 0 Then ((Cast(Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay,0) as int) / Pro_MinOrder) + 1) * Pro_MinOrder
						Else Cast(Isnull(Inv_BasicAmount - OnTheFrame - I.Inv_OnTheWay, 0) as int) End  As PURCHASE_VALUE,
						0 As RETURN_VALUE
				INTO #PURCHASERETURN_TEMP
				FROM
					Product.Master AS P JOIN [Product].[ProductInventory] PI ON P.Pro_ID = PI.ProInv_ProductID AND ProInv_MainFlag = 1 AND ProInv_WareHouseID = 0 AND Pro_IsCommon = 1
					JOIN Product.Inventory AS I ON PI.ProInv_InventoryID = I.Inv_ID
					JOIN #MEDBAG_TEMP AS M ON M.Inv_ID = I.Inv_ID
				WHERE (OnTheFrame + I.Inv_OnTheWay) < Inv_SafeAmount

				DECLARE @ITEM_COUNT INT;
				DECLARE @STOORD_COUNT NVARCHAR(3);
				DECLARE @DATE NVARCHAR(10) = CONVERT(NVARCHAR,GETDATE(),112);
				DECLARE @NEWSTOORD_ID NVARCHAR(13);
				
				SET @ITEM_COUNT = (SELECT COUNT(*) FROM #PURCHASERETURN_TEMP WHERE PURCHASE_VALUE > 0)
				
				IF @ITEM_COUNT > 0
				BEGIN
					SET @STOORD_COUNT = (SELECT TOP(1) FORMAT(SUBSTRING(StoOrd_ID, 11, 2) + 1, '-00') FROM [StoreOrder].[Master] WHERE CONVERT(DATE, GETDATE()) = CONVERT(DATE, StoOrd_CreateTime) ORDER BY SUBSTRING(StoOrd_ID, 11, 2) DESC);
					SET @NEWSTOORD_ID =  'P' + @DATE + ISNULL(@STOORD_COUNT, '-00');
				
					INSERT INTO [StoreOrder].[Master](StoOrd_ID,StoOrd_ReceiveID, StoOrd_OrderEmployeeID, StoOrd_CreateTime, StoOrd_ManufactoryID, StoOrd_Status, StoOrd_Type, StoOrd_WarehouseID,[StoOrd_IsOTCType])
					VALUES(@NEWSTOORD_ID,@NEWSTOORD_ID, {employeeId}, GETDATE(), 0, 'U', 'P', 0,'OTC');
				
					INSERT INTO [StoreOrder].[Detail] (StoOrdDet_MasterID, StoOrdDet_ProductID, StoOrdDet_ID, StoOrdDet_OrderAmount, StoOrdDet_UnitName, StoOrdDet_UnitAmount, StoOrdDet_Price, StoOrdDet_SubTotal, [StoOrdDet_FreeAmount], [StoOrdDet_RealAmount])
					SELECT @NEWSTOORD_ID, PRO_ID, ROW_NUMBER() OVER (ORDER BY PRO_ID), PURCHASE_VALUE, '基本單位', 1, ISNULL(CASE WHEN PURCHASE_VALUE > [bag_fact] THEN [jn_price] ELSE [ja_price] END, 0), ISNULL(CASE WHEN PURCHASE_VALUE > [bag_fact] THEN [jn_price] ELSE [ja_price] END, 0) * PURCHASE_VALUE, 0, 0
					FROM #PURCHASERETURN_TEMP AS T LEFT JOIN [HIS_POS_Server].[DataSource].[ProductFromSingde] AS S ON S.code = T.PRO_ID
					WHERE PURCHASE_VALUE > 0

					SELECT StoOrd_ID, StoOrd_ReceiveID, E.Emp_Name AS OrderEmp_Name, EE.Emp_Name AS RecEmp_Name, M.Man_ID, M.Man_Name, M.Man_NickName, M.Man_Telephone,
						   StoOrd_Status, StoOrd_Type, W.War_ID, W.War_Name, StoOrd_Note, s.StoOrd_CustomerName, s.StoOrd_PlanArrivalDate,
						   '' AS CUS_DATA, [StoOrd_TargetCustomerName],isnull([StoOrd_IsOTCType],'OTC')[StoOrd_IsOTCType],
						   ISNULL(D.ProductCount, 0) AS ProductCount, ISNULL(D.Total, 0) AS Total, CASE WHEN StoOrd_Status = 'U' THEN 1 WHEN StoOrd_Status = 'W' THEN 2 ELSE 3 END AS OrderKey, S.StoOrd_ReceiveTime, StoOrd_CreateTime,[StoOrd_IsPayCash]
					FROM [StoreOrder].[Master] AS S JOIN [Manufactory].[Master] AS M ON S.StoOrd_ManufactoryID = M.Man_ID
						 JOIN [Employee].[Master] AS E ON S.StoOrd_OrderEmployeeID = E.Emp_ID
						 JOIN [Product].[Warehouse] AS W ON S.StoOrd_WarehouseID = W.War_ID
						 LEFT JOIN [Employee].[Master] AS EE ON S.StoOrd_ReceiveEmployeeID = EE.Emp_ID
						 LEFT JOIN (SELECT [StoOrdDet_MasterID], SUM([StoOrdDet_SubTotal]) AS Total, COUNT(*) AS ProductCount FROM [StoreOrder].[Detail] GROUP BY [StoOrdDet_MasterID]) AS D ON D.StoOrdDet_MasterID = S.StoOrd_ID
						 LEFT JOIN [HIS].[PrescriptionMaster] AS P ON S.StoOrd_PrescriptionID = P.PreMas_ID
						 LEFT JOIN [HIS_POS_Server].[DataSource].[Institution] AS I ON I.Ins_ID = P.PreMas_InstitutionID
						 LEFT JOIN [HIS_POS_Server].[DataSource].[Division] AS DI ON DI.Div_ID = P.PreMas_DivisionID
						 LEFT JOIN [Customer].[Master] AS C ON P.PreMas_CustomerID = C.Cus_ID
					WHERE StoOrd_ID = @NEWSTOORD_ID
					ORDER BY OrderKey, StoOrd_ReceiveID;
					RETURN
				END

				SELECT '' AS StoOrdID";

				 conn.Execute(strSql); 
			}
		}

		//慢箋退貨功能
		public string Set_InsertStoreOrderReturnReserve(int employeeID)
		{

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				string strSql = $@"DECLARE @PRO [HIS].[MedicineListTable];
					INSERT INTO @PRO
					SELECT ProInv_ProductID 
					FROM 
					Product.ProductInventory proinv 
					join Product.Inventory on ProInv_InventoryID = Inv_ID and ProInv_WareHouseID = 0
					where Inv_Inventory > 0 and ProInv_MainFlag = 1;
				
					CREATE TABLE #MEDBAG_TEMP (Inv_ID INT, Inv_Inventory FLOAT, Inv_OnTheWay FLOAT, Inv_MedBagOnTheWay FLOAT, MegBagAmount FLOAT, OnTheFrame FLOAT)
					INSERT INTO #MEDBAG_TEMP 
					EXEC [Get].[AllInventoryByProIDs] @PRO, 0;
				
						select
							ProInv_InventoryID,
							Sum(isnull(ResDet_TotalAmount,0)) TotalAmount
						into #resUse
						from 
							His.ReserveMaster res,
							His.ReserveDetail resdet,
							Product.ProductInventory proinv 
						where 
							ResDet_MedicineID = ProInv_ProductID and
							ProInv_WareHouseID = 0 and
							res.ResMas_ID = resdet.ResDet_ReserveID and 
							res.ResMas_MedPrepareStatus = 'N' and
							((SYSDATETIME() between ResMas_AdjustDate and DateAdd(Day,20,ResMas_AdjustDate) or SYSDATETIME() <= ResMas_AdjustDate) and
							(DATEADD(Day,90,SYSDATETIME()) between ResMas_AdjustDate and DateAdd(Day,20,ResMas_AdjustDate) or DATEADD(Day,90,SYSDATETIME()) >= ResMas_AdjustDate)) 
						group by ProInv_InventoryID;
				
					Select
					distinct 
						PRO_ID PRO_ID,
						Pro_ChineseName PRO_NAME,
						Inv_Inventory INVENTORY,
						Inv_OnTheWay ONTHEWAY,
						0 PURCHASE_VALUE,
						meg.OnTheFrame - Isnull(resuse.TotalAmount,0) RETURN_VALUE
					Into #temp
					from 
						#MEDBAG_TEMP meg 
						join Product.ProductInventory proinv on meg.Inv_ID = ProInv_InventoryID and ProInv_MainFlag = 1
						join 
						  (SELECT 
								CASE WHEN Med_SystemID IS NULL THEN Pro_ID ELSE Med_NHIID END AS F_KEY, P.*
							FROM 
								[Product].[Master] AS P 
								Left Join [HIS_POS_Server].[DataSource].[Medicine_NHI_Singde] AS MNS ON P.Pro_ID = MNS.Med_SystemID  
						  ) pro on ProInv_ProductID = Pro_ID
						left join #resUse resuse on meg.Inv_ID = resuse.ProInv_InventoryID
						left Join HIS_POS_Server.DataSource.MedicineFromNHI on Med_ID = pro.F_KEY
					where Pro_IsCommon = 0 and Med_Control is null and Med_IsFrozen = 0 and meg.OnTheFrame - Isnull(resuse.TotalAmount,0) > 0
					order by Pro_ChineseName
				
				    
				DECLARE @ITEM_COUNT INT;
				DECLARE @STOORD_COUNT NVARCHAR(3);
				DECLARE @DATE NVARCHAR(10) = CONVERT(NVARCHAR,GETDATE(),112);
				DECLARE @NEWSTOORD_ID NVARCHAR(13);
				
				SET @ITEM_COUNT = (SELECT COUNT(*) FROM #temp WHERE RETURN_VALUE > 0)
				
				IF @ITEM_COUNT > 0
				BEGIN
					SET @STOORD_COUNT = (SELECT TOP(1) FORMAT(SUBSTRING(StoOrd_ID, 11, 2) + 1, '-00') FROM [StoreOrder].[Master] WHERE CONVERT(DATE, GETDATE()) = CONVERT(DATE, StoOrd_CreateTime) AND SUBSTRING(StoOrd_ID, 1, 1)in ('P','R') ORDER BY SUBSTRING(StoOrd_ID, 11, 2) DESC);
					SET @NEWSTOORD_ID =  'R' + @DATE + ISNULL(@STOORD_COUNT, '-00');
				
					INSERT INTO [StoreOrder].[Master](StoOrd_ID, StoOrd_OrderEmployeeID, StoOrd_CreateTime, StoOrd_ManufactoryID, StoOrd_Status, StoOrd_Type, StoOrd_WarehouseID)
					VALUES(@NEWSTOORD_ID, {employeeID}, GETDATE(), 0, 'U', 'R', 0);
				
					INSERT INTO [StoreOrder].[Detail] (StoOrdDet_MasterID, StoOrdDet_ProductID, StoOrdDet_ID, StoOrdDet_OrderAmount, StoOrdDet_UnitName, StoOrdDet_UnitAmount, StoOrdDet_Price, StoOrdDet_SubTotal, [StoOrdDet_FreeAmount], [StoOrdDet_RealAmount])
					SELECT @NEWSTOORD_ID, PRO_ID, ROW_NUMBER() OVER (ORDER BY PRO_ID), RETURN_VALUE, '基本單位', 1, ISNULL(CASE WHEN RETURN_VALUE > [bag_fact] THEN [jn_price] ELSE [ja_price] END, 0), ISNULL(CASE WHEN RETURN_VALUE > [bag_fact] THEN [jn_price] ELSE [ja_price] END, 0) * RETURN_VALUE, 0, 0
					FROM #temp AS T LEFT JOIN [HIS_POS_Server].[DataSource].[ProductFromSingde] AS S ON S.code = T.PRO_ID
					WHERE RETURN_VALUE > 0
				END
				Select @NEWSTOORD_ID StoOrdID";

				var result = conn.QueryFirst(strSql);

				return result;
			}
		}

		//寫入訂單資料
		public void Set_SaveStoreOrder()
		{
            using (var tranScope = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string strSql = @" ";

                    var result = conn.Query<dSingdeTotalOrder>(strSql);

                }


                tranScope.Complete();

			}
        }

		//不足訂購量轉新訂單 
		public void Set_StoreOrderAddLowerThenOrderAmount()
        {
			//@ORDER_ID NVARCHAR(20),
			//@EMPLOYEE INT,
			//@MAN_ID INT,
			//@WARE_ID INT,
			//@PRODUCTS[StoreOrder].[DetailTable] READONLY

			using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string strSql = @"DECLARE @STOORD_COUNT NVARCHAR(3) = (SELECT TOP(1) FORMAT(SUBSTRING(StoOrd_ID, 11, 2) + 1, '-00') FROM [StoreOrder].[Master] WHERE CONVERT(DATE, GETDATE()) = CONVERT(DATE, StoOrd_CreateTime) AND SUBSTRING(StoOrd_ID, 1, 1)in ('P','R') ORDER BY SUBSTRING(StoOrd_ID, 11, 2) DESC);
					DECLARE @DATE NVARCHAR(10) = CONVERT(NVARCHAR,GETDATE(),112);
					
					DECLARE @NEWSTOORD_ID NVARCHAR(13) = 'P' + @DATE + ISNULL(@STOORD_COUNT, '-00');
					
					INSERT INTO [StoreOrder].[Master](StoOrd_ID, StoOrd_ReceiveID, StoOrd_OrderEmployeeID, StoOrd_CreateTime, StoOrd_ManufactoryID, StoOrd_Status, StoOrd_Type, StoOrd_WarehouseID, StoOrd_Note)
					VALUES(@NEWSTOORD_ID, @NEWSTOORD_ID, @EMPLOYEE, GETDATE(), @MAN_ID, 'P', 'P', @WARE_ID, @ORDER_ID + ' 不足量轉新收貨單 ' + @NEWSTOORD_ID);
					
					INSERT INTO [StoreOrder].[Detail] ([StoOrdDet_MasterID], [StoOrdDet_ProductID], [StoOrdDet_ID], [StoOrdDet_OrderAmount], [StoOrdDet_UnitName], [StoOrdDet_UnitAmount], [StoOrdDet_RealAmount], [StoOrdDet_Price], [StoOrdDet_SubTotal], [StoOrdDet_ValidDate], [StoOrdDet_BatchNumber], [StoOrdDet_Note], [StoOrdDet_FreeAmount], [StoOrdDet_Invoice])
					SELECT @NEWSTOORD_ID, [StoOrdDet_ProductID], [StoOrdDet_ID], [StoOrdDet_OrderAmount], [StoOrdDet_UnitName], [StoOrdDet_UnitAmount], [StoOrdDet_RealAmount], [StoOrdDet_Price], [StoOrdDet_SubTotal], [StoOrdDet_ValidDate], [StoOrdDet_BatchNumber], [StoOrdDet_Note], [StoOrdDet_FreeAmount], [StoOrdDet_Invoice]
					FROM @PRODUCTS

	                SELECT @NEWSTOORD_ID AS NEW_ID";

                var result = conn.Query<dSingdeTotalOrder>(strSql);

            }
        }

		

	}
}
