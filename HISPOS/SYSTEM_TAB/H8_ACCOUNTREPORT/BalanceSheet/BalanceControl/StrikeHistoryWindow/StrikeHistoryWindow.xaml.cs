using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    /// <summary>
    /// StrikeHistoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StrikeHistoryWindow : Window
    {
        public StrikeHistoryWindow()
        {
            InitializeComponent();
            DataContext = new StrikeHistoryViewModel();
            GetDataList();
        }

        public DataTable _StrikeName = new DataTable();
        public DataTable _StrikeType = new DataTable();

        public class AccountsName
        {
            public string StrikeName { get; set; }
        }
        public class AccountsType
        {
            public string StrikeType { get; set; }
        }
        public List<AccountsName> _StrikeNameAccounts = new List<AccountsName>();
        public List<AccountsType> _StrikeTypeAccounts = new List<AccountsType>();
        private void GetDataList()
        {
            _StrikeName = ((StrikeHistoryViewModel)DataContext).TypeTable;
            foreach(DataRow dr in _StrikeName.Rows)
            {
                AccountsName name = new AccountsName();
                name.StrikeName = " " + Convert.ToString(dr[0]);//預留空格，防止關鍵字搜尋帶入第一筆
                _StrikeNameAccounts.Add(name);
            }
            _StrikeType = ((StrikeHistoryViewModel)DataContext).SujectTable;
            foreach (DataRow dr in _StrikeType.Rows)
            {
                AccountsType name = new AccountsType();
                name.StrikeType = " " + Convert.ToString(dr[0]);//預留空格，防止關鍵字搜尋帶入第一筆
                _StrikeTypeAccounts.Add(name);
            }
        }

        private void Cob_KeyUp(object sender, KeyEventArgs e)
        {
            cobStrikeName.ItemsSource = _StrikeNameAccounts;
            cobStrikeType.ItemsSource = _StrikeTypeAccounts;
            ComboBox cmb = (ComboBox)sender;
            ICollectionView itemsView = CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            bool isFilter = itemsView.CanFilter;
            if (isFilter)
            {
                if (cmb.Name == "cobStrikeName")
                {
                    itemsView.Filter = (o) =>
                    {
                        AccountsName accounts = (AccountsName)o;
                        if (string.IsNullOrEmpty(cmb.Text.TrimStart().TrimStart()))
                        {
                            return true;
                        }
                        else
                        {
                            if (Convert.ToString(accounts.StrikeName).Contains(cmb.Text.TrimStart().TrimStart()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    };
                }
                else
                {
                    itemsView.Filter = (o) =>
                    {
                        AccountsType accounts = (AccountsType)o;
                        if (string.IsNullOrEmpty(cmb.Text.TrimStart().TrimStart()))
                        {
                            return true;
                        }
                        else
                        {
                            if (Convert.ToString(accounts.StrikeType).Contains(cmb.Text.TrimStart().TrimStart()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    };
                }
            }
            cmb.IsDropDownOpen = true;
            itemsView.Refresh();
        }
    }
}