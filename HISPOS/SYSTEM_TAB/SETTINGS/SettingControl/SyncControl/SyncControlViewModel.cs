using Dapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.SyncControl
{
    public class SyncControlViewModel : ViewModelBase
    {
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                Set(() => IsBusy, ref isBusy, value);
            }
        }

        private string busyContent;
        public string BusyContent
        {
            get => busyContent;
            set
            {
                Set(() => BusyContent, ref busyContent, value);
            }
        }
        public RelayCommand BtnUpdateSingdeMedStockCommand { get; set; }//更新杏德藥品庫存
        public RelayCommand BtnUpdateSingdeOTCStockCommand { get; set; }//更新杏德OTC庫存
        public RelayCommand BtnSyncSingdeStockCommand { get; set; }//同步杏德商品
        public RelayCommand BtnSyncSingdeOTCStockCommand { get; set; }//同步杏德OTC商品
        public SyncControlViewModel()
        {
            BtnUpdateSingdeMedStockCommand = new RelayCommand(BtnUpdateSingdeMedStockAction);
            BtnUpdateSingdeOTCStockCommand = new RelayCommand(BtnUpdateSingdeOTCStockAction);
            BtnSyncSingdeStockCommand = new RelayCommand(BtnSyncSingdeStockAction);
            BtnSyncSingdeOTCStockCommand = new RelayCommand(BtnSyncSingdeOTCStockAction);
        }
        private void BtnUpdateSingdeMedStockAction()
        {
            MainWindow.SingdeConnection.OpenConnection();
            DataTable singdetable = ProductDB.GetDataToUpdateSingdeStock();
            MainWindow.SingdeConnection.CloseConnection();
            if (singdetable != null && singdetable.Rows.Count > 0)
            {
                DataTable source = CreateTableStruct(singdetable);
                try
                {
                    SQLServerConnection.DapperQuery((conn) =>
                    {
                        conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[MedicineFromSingde]", param: new
                        {
                            table = source
                        }, commandType: CommandType.StoredProcedure);
                    });
                    MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }
        private void BtnUpdateSingdeOTCStockAction()
        {
            MainWindow.SingdeConnection.OpenConnection();
            DataTable singdetable = ProductDB.GetDataToUpdateSingdeStock();
            MainWindow.SingdeConnection.CloseConnection();
            if (singdetable != null && singdetable.Rows.Count > 0)
            {
                DataTable source = CreateTableStruct(singdetable);
                try
                {
                    SQLServerConnection.DapperQuery((conn) =>
                    {
                        conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[syncProductFromSingde]", param: new
                        {
                            table = source
                        }, commandType: CommandType.StoredProcedure);
                    });
                    MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }
        private void BtnSyncSingdeStockAction()
        {
            MainWindow.SingdeConnection.OpenConnection();
            DataTable singdetable = ProductDB.GetDataToUpdateSingdeStock();
            MainWindow.SingdeConnection.CloseConnection();
            if (singdetable != null && singdetable.Rows.Count > 0)
            {
                DataTable source = CreateTableStruct(singdetable);
                try
                {
                    SQLServerConnection.DapperQuery((conn) =>
                    {

                        conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[UpdateSingdeStock]", param: new
                        {
                            table = source
                        }, commandType: CommandType.StoredProcedure);
                    });
                    MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }
        private void BtnSyncSingdeOTCStockAction()
        {
            MainWindow.SingdeConnection.OpenConnection();
            DataTable singdetable = ProductDB.GetDataToInsertSingdeOTC();
            MainWindow.SingdeConnection.CloseConnection();
            if (singdetable != null && singdetable.Rows.Count > 0)
            {
                DataTable source = CreateTableStruct(singdetable);
                try
                {
                    SQLServerConnection.DapperQuery((conn) =>
                    {
                        conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[UpdateOTCProductsFromSingde]", param: new
                        {
                            table = source
                        }, commandType: CommandType.StoredProcedure);
                    });
                    SQLServerConnection.DapperQuery((conn) =>
                    {
                        conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[OTCProductImport]", commandType: CommandType.StoredProcedure);
                    });
                    MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }

        private DataTable CreateTableStruct(DataTable source)
        {
            Dictionary<string, Type> dicCol = new Dictionary<string, Type>() {
                { "code", typeof(string) }, { "new_code", typeof(string) }, { "nat_code", typeof(string) }, { "name", typeof(string) },
                { "ch_name", typeof(string) }, { "type", typeof(string) }, { "high_price", typeof(string) }, { "side", typeof(string) },
                { "note", typeof(string) }, { "idn", typeof(string) }, { "stop_using", typeof(string) }, { "inv_qty", typeof(string) },
                { "min_order", typeof(int) }, { "bag_fact", typeof(int) }, { "ja_price", typeof(decimal) }, { "jn_price", typeof(decimal) },
                { "sup_stop", typeof(string) }, { "order_stop", typeof(string) }, { "inv_memo", typeof(string) }};

            DataTable table = new DataTable();
            foreach (KeyValuePair<string, Type> item in dicCol)
            {
                table.Columns.Add(item.Key, item.Value);
            }
            foreach (DataRow dr in source.Rows)
            {
                table.ImportRow(dr);
            }
            
            return table;
        }
    }
}
