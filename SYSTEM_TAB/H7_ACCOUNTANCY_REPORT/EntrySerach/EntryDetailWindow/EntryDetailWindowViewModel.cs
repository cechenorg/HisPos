﻿using GalaSoft.MvvmLight;
using His_Pos.NewClass.StockValue.StockEntry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach.EntryDetailWindow
{
    public class EntryDetailWindowViewModel : ViewModelBase
    {
        #region Var
        private StockEntrys entryDetailCollection = new StockEntrys();
        public StockEntrys EntryDetailCollection
        {
            get { return entryDetailCollection; }
            set { Set(() => EntryDetailCollection, ref entryDetailCollection, value); }
        }
        private CollectionViewSource entryCollectionViewSource;
        private CollectionViewSource EntryCollectionViewSource
        {
            get => entryCollectionViewSource;
            set
            {
                Set(() => EntryCollectionViewSource, ref entryCollectionViewSource, value); 
            }
        }

        private ICollectionView entryCollectionView;
        public ICollectionView EntryCollectionView
        {
            get => entryCollectionView;
            private set
            {
                Set(() => EntryCollectionView, ref entryCollectionView, value);
            }
        }
        private bool isPurchaseCheck;
        public bool IsPurchaseCheck
        {
            get { return isPurchaseCheck; }
            set { Set(() => IsPurchaseCheck, ref isPurchaseCheck, value);
                EntryCollectionViewSource.Filter += Filter;
            }
        }
        private bool isReturnCheck;
        public bool IsReturnCheck
        {
            get { return isReturnCheck; }
            set { Set(() => IsReturnCheck, ref isReturnCheck, value);
                EntryCollectionViewSource.Filter += Filter;
            }
        }
        private bool isAdjustCheck;
        public bool IsAdjustCheck
        {
            get { return isAdjustCheck; }
            set { Set(() => IsAdjustCheck, ref isAdjustCheck, value);
                EntryCollectionViewSource.Filter += Filter;
            }
        }
        private bool isStockTakingCheck;
        public bool IsStockTakingCheck
        {
            get { return isStockTakingCheck; }
            set { Set(() => IsStockTakingCheck, ref isStockTakingCheck, value);
                EntryCollectionViewSource.Filter += Filter;
            }
        }
        #endregion
        public EntryDetailWindowViewModel(DateTime date) {
            EntryDetailCollection.GetDataByDate(date);
            EntryCollectionViewSource = new CollectionViewSource { Source = EntryDetailCollection };
            EntryCollectionView = EntryCollectionViewSource.View;
            EntryCollectionViewSource.Filter += Filter;
        }
        private void Filter(object sender, FilterEventArgs e) {
            if (e.Item is null) return; 
            e.Accepted = false;
            StockEntry stockEntry = ((StockEntry)e.Item);
            if(IsPurchaseCheck && stockEntry.Source == EntryDetailEnum.Purchase)
                e.Accepted = true;
            else if(IsReturnCheck && stockEntry.Source == EntryDetailEnum.Return)
                e.Accepted = true;
            else if (IsAdjustCheck && stockEntry.Source == EntryDetailEnum.Adjust)
                e.Accepted = true;
            else if (IsStockTakingCheck && stockEntry.Source == EntryDetailEnum.StockTaking)
                e.Accepted = true;
             
        }
    }
}
