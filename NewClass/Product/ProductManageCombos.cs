using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{

        public class ProductManageLocCombos : ObservableCollection<ProductManageLocCombo>
        {
            public ProductManageLocCombos(DataTable dataTable)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Add(new ProductManageLocCombo(row));
                }
            }
        internal static ProductManageLocCombos GetProductManageLocCombos()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable resultI = MainWindow.ServerConnection.ExecuteProc("[Get].[ProductLocationMasterComboBox]");
            MainWindow.ServerConnection.CloseConnection();


            return new ProductManageLocCombos(resultI);
        }
    }
    }
