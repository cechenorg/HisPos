﻿using System.Collections.Generic;
using ChromeTabs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using His_Pos.Class;
using System.Windows;
using System.Windows.Controls;

namespace His_Pos.ViewModel
{
    public class ProductDetailViewModel : ViewModelBase
    {
        public RelayCommand<TabReorder> ReorderTabsCommand { get; set; }
        public RelayCommand<object> AddTabCommand { get; set; }
        public RelayCommand<TabBase> CloseTabCommand { get; set; }
        public ObservableCollection<TabBase> ItemCollection { get; set; }
        //This is the current selected tab, if you change it, the tab is selected in the tab control.
        private TabBase _selectedTab;
        public TabBase SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab != value)
                {
                    Set(() => SelectedTab, ref _selectedTab, value);
                }
            }
        }

        private bool _canAddTabs;
        public bool CanAddTabs
        {
            get { return _canAddTabs; }
            set
            {
                if (_canAddTabs != value)
                {
                    Set(() => CanAddTabs, ref _canAddTabs, value);
                    AddTabCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ProductDetailViewModel()
        {
            this.ItemCollection = new ObservableCollection<TabBase>();
            this.ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
            this.ReorderTabsCommand = new RelayCommand<TabReorder>(ReorderTabsCommandAction);
            this.AddTabCommand = new RelayCommand<object>(AddTabCommandAction);
            //this.AddTabCommand = new RelayCommand<object>(AddProductDetailTabAction);
            this.CloseTabCommand = new RelayCommand<TabBase>(CloseTabCommandAction);
            CanAddTabs = true;
        }

        protected virtual void ReorderTabsCommandAction(TabReorder reorder)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            int from = reorder.FromIndex;
            int to = reorder.ToIndex;
            var tabCollection = view.Cast<TabBase>().ToList();//Get the ordered collection of our tab control
            tabCollection[from].TabNumber = tabCollection[to].TabNumber; //Set the new index of our dragged tab

            if (to > from)
            {
                for (int i = from + 1; i <= to; i++)
                {
                    tabCollection[i].TabNumber--; //When we increment the tab index, we need to decrement all other tabs.
                }
            }
            else if (from > to)//when we decrement the tab index
            {
                for (int i = to; i < from; i++)
                {
                    tabCollection[i].TabNumber++;//When we decrement the tab index, we need to increment all other tabs.
                }
            }

            view.Refresh();//Refresh the view to force the sort description to do its work.
        }

        //We need to set the TabNumber property on the viewmodels when the item source changes to keep it in sync.
        void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TabBase tab in e.NewItems)
                {
                    if (this.ItemCollection.Count > 1)
                    {
                        //If the new tab don't have an existing number, we increment one to add it to the end.
                        if (tab.TabNumber == 0)
                            tab.TabNumber = this.ItemCollection.OrderBy(x => x.TabNumber).LastOrDefault().TabNumber + 1;
                    }
                }
            }
            else
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
                view.Refresh();
                var tabCollection = view.Cast<TabBase>().ToList();
                foreach (var item in tabCollection)
                    item.TabNumber = tabCollection.IndexOf(item);
            }
        }
        private void CloseTabCommandAction(TabBase vm)
        {
            switch (vm.TabName)
            {
                case "OTCCC":
                    break;
                case nameof(FeatureItem.處方登錄):
                    break;
                case nameof(FeatureItem.處方查詢):
                    break;
                case nameof(FeatureItem.庫存查詢):
                    break;
                case nameof(FeatureItem.處理單管理):
                    //if (ProductPurchase.ProductPurchaseView.Instance.backgroundWorker.IsBusy)
                    //{
                    //    MessageWindow message = new MessageWindow("正在儲存", MessageType.ERROR);
                    //    return;
                    //}
                    break;
                case nameof(FeatureItem.處理單紀錄):
                    break;
                case nameof(FeatureItem.新增盤點):
                    break;
                case nameof(FeatureItem.庫存盤點紀錄):
                    break;
                default:
                    return;
            }

            this.ItemCollection.Remove(vm);
        }
        public void AddProductDetailTabAction(object featureItem)
        {
            if (IsTabOpened(featureItem.ToString())) return;
            TabBase newTab;
            newTab = new InventoryManagement.OtcDetailView() { TabName = "OTCCC", Icon = MainWindow.HisFeatures[0].Icon };
            this.ItemCollection.Add(newTab.getTab());
            this.SelectedTab = this.ItemCollection[ItemCollection.Count - 1];
        }
        //Adds a random tab
        public void AddTabCommandAction(object featureItem)
        {
            if (IsTabOpened(featureItem.ToString())) return;

            TabBase newTab;

            switch (featureItem.ToString())
            {
                case nameof(FeatureItem.處方登錄):
                    newTab = new PrescriptionDec.PrescriptionDec() { TabName = MainWindow.HisFeatures[0].Functions[0], Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                case nameof(FeatureItem.處方查詢):
                    newTab = new PrescriptionInquire.PrescriptionInquire() { TabName = MainWindow.HisFeatures[0].Functions[1], Icon = MainWindow.HisFeatures[0].Icon };
                    break;
                case nameof(FeatureItem.庫存查詢):
                    newTab = new InventoryManagement.InventoryManagement() { TabName = MainWindow.HisFeatures[1].Functions[0], Icon = MainWindow.HisFeatures[1].Icon };
                    break;
                case nameof(FeatureItem.處理單管理):
                    newTab = new ProductPurchase.ProductPurchase() { TabName = MainWindow.HisFeatures[1].Functions[1], Icon = MainWindow.HisFeatures[1].Icon };
                    break;
                case nameof(FeatureItem.處理單紀錄):
                    newTab = new ProductPurchaseRecord.ProductPurchaseRecord() { TabName = MainWindow.HisFeatures[1].Functions[2], Icon = MainWindow.HisFeatures[1].Icon };
                    break;
                case nameof(FeatureItem.新增盤點):
                    newTab = new StockTaking.StockTaking() { TabName = MainWindow.HisFeatures[2].Functions[0], Icon = MainWindow.HisFeatures[2].Icon };
                    break;
                case nameof(FeatureItem.庫存盤點紀錄):
                    newTab = new StockTakingRecord.StockTakingRecord() { TabName = MainWindow.HisFeatures[2].Functions[1], Icon = MainWindow.HisFeatures[2].Icon };
                    break;
                default:
                    return;
            }
            this.ItemCollection.Add(newTab.getTab());
            this.SelectedTab = this.ItemCollection[ItemCollection.Count - 1];
        }

        public bool IsTabOpened(string tabName)
        {
            
            foreach (TabBase tab in ItemCollection)
            {
                if (tab.TabName == tabName)
                {
                    SelectedTab = tab;

                    switch (tabName)
                    {
                        case nameof(FeatureItem.處方登錄):
                            break;
                        case nameof(FeatureItem.處方查詢):
                            break;
                        case nameof(FeatureItem.庫存查詢):
                            if (InventoryManagement.InventoryManagementView.Instance is null) break;

                            if (InventoryManagement.InventoryManagementView.DataChanged)
                            {
                                InventoryManagement.InventoryManagementView.Instance.MergingData();
                                InventoryManagement.InventoryManagementView.Instance.SearchData();
                            }
                            break;
                        case nameof(FeatureItem.處理單管理):
                            break;
                        case nameof(FeatureItem.處理單紀錄):
                            if (ProductPurchaseRecord.ProductPurchaseRecordView.Instance is null) break;

                            if (ProductPurchaseRecord.ProductPurchaseRecordView.DataChanged)
                                ProductPurchaseRecord.ProductPurchaseRecordView.Instance.UpdateUi();

                            ProductPurchaseRecord.ProductPurchaseRecordView.Instance.PassValueSearchData();
                            break;
                        case nameof(FeatureItem.新增盤點):
                            break;
                        case nameof(FeatureItem.庫存盤點紀錄):
                            break;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}

