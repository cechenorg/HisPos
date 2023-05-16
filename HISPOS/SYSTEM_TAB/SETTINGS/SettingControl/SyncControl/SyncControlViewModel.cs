using Dapper;
using DTO.WebService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.Database;
using His_Pos.FunctionWindow;
using His_Pos.InfraStructure;
using His_Pos.NewClass.Product;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.SyncControl
{
    public class SyncControlViewModel : ViewModelBase
    {
        public static readonly Dictionary<string, string> btnItemValue = new Dictionary<string, string>()
        {
            { "SynControlMedData", "管藥資料" },
            { "SynSingdeStockData", "杏德門市商品資料(價格及庫存量)"},
            { "SynSingdeMedData", "杏德藥品"}
        };
        private string busyContent;
        public string BusyContent
        {
            get => busyContent;
            set { Set(() => BusyContent, ref busyContent, value); }
        }
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }
        public class ApiBtnItem
        {
            public string ItemID { get; set; }
            public string ItemName { get; set; }
            public DateTime UpdateTime { get; set; }
            public DateTime SourceUpdateTime { get; set; }
        }
        private ObservableCollection<ApiBtnItem> currentApiItem;
        public ObservableCollection<ApiBtnItem> CurrentApiItem
        {
            get => currentApiItem;
            set { Set(() => CurrentApiItem, ref currentApiItem, value); }
        }
        public ApiBtnItem selectedApiItem;
        public ApiBtnItem SelectedApiItem
        {
            get => selectedApiItem;
            set { Set(() => SelectedApiItem, ref selectedApiItem, value); }
        }
        public SyncControlViewModel()
        {
            GetData();
        }
        private async void GetData()
        {
            HISPOSWebApiService service = new HISPOSWebApiService();
            CommonDataRepository _commonDataRepository = new CommonDataRepository();
            List<UpdateTimeDTO> _updSouceList = await service.GetDataSourceUpdTime();
            List<UpdateTimeDTO> _updList = _commonDataRepository.GetCurrentUpdateTime();
            CurrentApiItem = new ObservableCollection<ApiBtnItem>();
            foreach (KeyValuePair<string, string> pair in btnItemValue)
            {
                ApiBtnItem item = new ApiBtnItem();
                item.ItemID = pair.Key;
                item.ItemName = pair.Value;
                IEnumerable<UpdateTimeDTO> updateTime = _updList.Where(w => w.UpdTime_TableName.Equals(pair.Key));
                if (updateTime != null && updateTime.Count() > 0)
                    item.UpdateTime = updateTime.FirstOrDefault().UpdTime_LastUpdateTime;

                IEnumerable<UpdateTimeDTO> updateSourceTime = _updSouceList.Where(w => w.UpdTime_TableName.Equals(pair.Key));
                if (updateSourceTime != null && updateTime.Count() > 0)
                    item.SourceUpdateTime = updateSourceTime.FirstOrDefault().UpdTime_LastUpdateTime;

                CurrentApiItem.Add(item);
            }
        }
        public void BtnSyncAction(string name)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {
                BusyContent = "同步資料中";
                switch (name)
                {
                    case "SynControlMedData":
                        if (CheckDate())
                            BtnUpdateSingdeMedStockAction(name);
                        break;

                    case "SynSingdeStockData":
                        if (CheckDate())
                            BtnUpdateSingdeOTCStockAction(name);
                        break;

                    case "SynSingdeMedData":
                        if (CheckDate())
                            BtnSyncSingdeStockAction(name);
                        break;

                    default:
                        break;
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                GetData();
                IsBusy = false;
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }
        private bool CheckDate()
        {
            if (SelectedApiItem != null)
            {
                if (DateTime.Compare(SelectedApiItem.SourceUpdateTime, SelectedApiItem.UpdateTime) > 0)
                {
                    return true;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageWindow.ShowMessage("已更新", MessageType.SUCCESS);
                    });
                    return false;
                }
            }
            return false;
        }
        private void BtnUpdateSingdeMedStockAction(string name)
        {
            DataTable singdetable = ProductDB.GetDataToUpdateSingdeStock();
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                        SetUpdateRecord(name);
                    });
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }
        private void BtnUpdateSingdeOTCStockAction(string name)
        {
            DataTable singdetable = ProductDB.GetDataToUpdateSingdeStock();
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
                    SQLServerConnection.DapperQuery((conn) =>
                    {
                        conn.Query<int>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[UpdateOTCProductsFromSingde]", param: new
                        {
                            table = source
                        }, commandType: CommandType.StoredProcedure);
                    });
                    SQLServerConnection.DapperQuery((conn) =>
                    {
                        conn.Query<string>($"{Properties.Settings.Default.SystemSerialNumber}.[Set].[OTCProductImport]", commandType: CommandType.StoredProcedure);
                    });
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                        SetUpdateRecord(name);
                    });
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }
        private void BtnSyncSingdeStockAction(string name)
        {
            DataTable singdetable = ProductDB.GetDataToUpdateSingdeStock();
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageWindow.ShowMessage("同步成功", MessageType.SUCCESS);
                        SetUpdateRecord(name);
                    });
                }
                catch (Exception e)
                {
                    MessageWindow.ShowMessage("同步失敗", MessageType.ERROR);
                }
            }
        }
        private void SetUpdateRecord(string name)
        {
            List<UpdateTimeDTO> updateList = new List<UpdateTimeDTO>
                        {
                            new UpdateTimeDTO
                            {
                                UpdTime_LastUpdateTime = DateTime.Today,
                                UpdTime_TableName = name
                            }
                        };
            CommonDataRepository _commonDataRepository = new CommonDataRepository();
            _commonDataRepository.SyncUpdateTime(updateList);
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
