﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicineSetWindow;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow;

using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DomainModel.Enum;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;


namespace His_Pos.FunctionWindow.AddProductWindow
{
    public class AddProductViewModel : ViewModelBase
    {
        #region ----- Define Command -----

        public RelayCommand GetRelatedDataCommand { get; set; }
        public RelayCommand StartEditingCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        public RelayCommand<string> FocusUpDownCommand { get; set; }
        public RelayCommand ProductSelected { get; set; }

        #endregion ----- Define Command -----

        #region ----- Define Variables -----

        private CollectionViewSource proStructCollectionViewSource;
        private ICollectionView proStructCollectionView;
        private ProductStruct selectedProductStruct;
        private string searchString;
        private bool isEditing = true;
        private AddProductEnum addProEnum;
        private string wareID;
        private OrderStatusEnum orderStatus = (OrderStatusEnum)1;
        private string orderTypeIsOTC="全部";

        private CollectionViewSource ProStructCollectionViewSource
        {
            get => proStructCollectionViewSource;
            set { Set(() => ProStructCollectionViewSource, ref proStructCollectionViewSource, value); }
        }

        public ICollectionView ProStructCollectionView
        {
            get => proStructCollectionView;
            private set { Set(() => ProStructCollectionView, ref proStructCollectionView, value); }
        }

        public bool IsEditing
        {
            get => isEditing;
            private set { Set(() => IsEditing, ref isEditing, value); }
        }

        public bool IsProductSelected { get; set; } = false;
        public bool HideDisableProduct { get; set; }
        public bool ShowOnlyThisManufactory { get; set; }

        public string OrderTypeIsOTC
        {
            get => orderTypeIsOTC;
            set { Set(() => OrderTypeIsOTC, ref orderTypeIsOTC, value); }
        }
        public string SearchString
        {
            get => searchString;
            set { Set(() => SearchString, ref searchString, value); }
        }
        public ProductStruct SelectedProductStruct
        {
            get => selectedProductStruct;
            set { Set(() => SelectedProductStruct, ref selectedProductStruct, value); }
        }

        public ProductStructs ProductStructCollection { get; set; }

        #endregion ----- Define Variables -----

        public AddProductViewModel(AddProductEnum addProductEnum, string wareHouseID, OrderStatusEnum OrderStatus)
        {
            addProEnum = addProductEnum;
            RegisterCommand();
            RegisterFilter();

            SearchString = "";
            wareID = wareHouseID;
            orderStatus = OrderStatus;
        }

        public AddProductViewModel(AddProductEnum addProductEnum, string wareHouseID)
        {
            addProEnum = addProductEnum;
            RegisterCommand();
            RegisterFilter();

            SearchString = "";
            wareID = wareHouseID;
        }

        public AddProductViewModel(string searchString, AddProductEnum addProductEnum, string wareHouseID, OrderStatusEnum OrderStatus)
        {
            addProEnum = addProductEnum;
            RegisterCommand();
            RegisterFilter();

            SearchString = searchString;
            wareID = wareHouseID;
            orderStatus = OrderStatus;
            GetRelatedDataAction(addProductEnum);
        }

        public AddProductViewModel(string searchString, AddProductEnum addProductEnum, string wareHouseID)
        {
            addProEnum = addProductEnum;
            RegisterCommand();
            RegisterFilter();

            SearchString = searchString;
            wareID = wareHouseID;
            GetRelatedDataAction(addProductEnum);
        }

        public AddProductViewModel(string searchString, AddProductEnum addProductEnum, string wareHouseID, OrderStatusEnum OrderStatus, string orderTypeIsOTC)
        {
            addProEnum = addProductEnum;
            RegisterCommand();
            RegisterFilter();

            SearchString = searchString;
            wareID = wareHouseID;

            OrderTypeIsOTC = orderTypeIsOTC;
            GetRelatedDataAction(addProductEnum);
        }

        ~AddProductViewModel()
        {
            SearchString = string.Empty;
            Messenger.Default.Unregister(this);
        }

        #region ----- Define Actions -----
        private void GetRelatedDataAction()
        {
            GetRelatedDataAction(AddProductEnum.PrescriptionDeclare);
        }
        private void GetRelatedDataAction(AddProductEnum addProductEnum)
        {
            if (IsEditing)
            {
                if (SearchString.Length >0)
                {
                    int noOTC = addProductEnum == AddProductEnum.PrescriptionDeclare? 1 : 0;
                    IsEditing = false;
                    HideDisableProduct = false;
                    MainWindow.ServerConnection.OpenConnection();
                    ProductStructCollection = ProductStructs.GetProductStructsBySearchString(SearchString, wareID, noOTC);
                    MainWindow.ServerConnection.CloseConnection();
                    ProStructCollectionViewSource = new CollectionViewSource { Source = ProductStructCollection };
                    ProStructCollectionView = ProStructCollectionViewSource.View;
                    AddFilter();
                    ProStructCollectionViewSource.Filter += ProductIsOTCFilter;
                    switch (ProStructCollectionView.Cast<object>().Count())
                    {
                        case 0:
                            MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
                            break;

                        /*case 1:
                            SelectedProductStruct = (ProductStruct)ProStructCollectionView.Cast<object>().First();
                            if (SelectedProductStruct.OTCFromSingde == true && addProEnum == AddProductEnum.ProductPurchase /*&& orderStatus == OrderStatusEnum.SINGDE_UNPROCESSIN)
                            {
                                if (orderStatus == OrderStatusEnum.SINGDE_UNPROCESSING || orderStatus == OrderStatusEnum.SINGDE_PROCESSING) {
                                    MessageWindow.ShowMessage("非杏德品無法訂貨", MessageType.WARNING);
                                    return; 
                                }
                            }
                            ProductSelectedAction();
                            break;*/

                        default:
                            ProStructCollectionViewSource.View.MoveCurrentToFirst();
                            SelectedProductStruct = (ProductStruct)ProStructCollectionViewSource.View.CurrentItem;
                            if (ProductStructCollection.Count==1) 
                            {
                                ProductSelectedAction();
                            }
                            
                            break;
                    }
                }
                else
                    MessageWindow.ShowMessage("查詢ID需至少5碼", MessageType.WARNING);
            }
            else
            {
                ProductSelectedAction();
            }
        }

        private void ProductPurchaseFilterAction()
        {
        }

        private void ProductReturnFilter(object sender, FilterEventArgs e)
        {
            if (((ProductStruct)e.Item).Inventory > 0)
                e.Accepted = true;
            else
                e.Accepted = false;
        }

        private void ProductTradeFilter(object sender, FilterEventArgs e)
        {
            if (((ProductStruct)e.Item).Type == ProductTypeEnum.OTCMedicine)
                e.Accepted = true;
            else
                e.Accepted = false;
        }

        private void FocusUpDownAction(string direction)
        {
            if (!IsEditing && ProductStructCollection.Count > 0)
            {
                int maxIndex = ProductStructCollection.Count - 1;

                switch (direction)
                {
                    case "UP":
                        if (ProStructCollectionView.CurrentPosition > 0)
                            ProStructCollectionView.MoveCurrentToPrevious();
                        break;

                    case "DOWN":
                        if (ProStructCollectionView.CurrentPosition < maxIndex)
                            ProStructCollectionView.MoveCurrentToNext();
                        break;
                    case "ENTER":
                        ProductSelectedAction();
                        break;
                }
                if (ProStructCollectionView.CurrentItem != null) 
                {
                    SelectedProductStruct = (ProductStruct)ProStructCollectionView.CurrentItem;
                }
            }
        }

        private void ProductSelectedAction()
        {
            if (string.IsNullOrEmpty(SelectedProductStruct.ID)) return;
            Messenger.Default.Send(SelectedProductStruct, "SelectedProduct");
            switch (addProEnum)
            {
                case AddProductEnum.ProductReturn:
                    if (SelectedProductStruct.Inventory == 0)
                    {
                        MessageWindow.ShowMessage(string.Format("{0} \n 庫存為零，不能退貨!", SelectedProductStruct.ID), MessageType.ERROR);
                        return;
                    }
                    Messenger.Default.Send(new NotificationMessage<ProductStruct>(this, SelectedProductStruct, nameof(ProductPurchaseReturnViewModel)));
                    break;

                case AddProductEnum.ProductPurchase:
                    if (SelectedProductStruct.OTCFromSingde == true /*&& orderStatus == OrderStatusEnum.SINGDE_UNPROCESSING*/)
                    {
                        if (orderStatus == OrderStatusEnum.SINGDE_UNPROCESSING || orderStatus == OrderStatusEnum.SINGDE_PROCESSING)
                        {
                            MessageWindow.ShowMessage("非杏德品無法訂貨", MessageType.WARNING);
                            return;
                        }
                    }
                    Messenger.Default.Send(new NotificationMessage<ProductStruct>(this, SelectedProductStruct, nameof(ProductPurchaseReturnViewModel)));
                    break;

                case AddProductEnum.PrescriptionDeclare:
                    Messenger.Default.Send(new NotificationMessage<ProductStruct>(this, SelectedProductStruct, nameof(PrescriptionDeclareViewModel)));
                    break;

                case AddProductEnum.PrescriptionEdit:
                    Messenger.Default.Send(new NotificationMessage<ProductStruct>(this, SelectedProductStruct, nameof(PrescriptionEditViewModel)));
                    break;

                case AddProductEnum.MedicineSetWindow:
                    Messenger.Default.Send(new NotificationMessage<ProductStruct>(this, SelectedProductStruct, nameof(MedicineSetViewModel)));
                    break;

                case AddProductEnum.ProductGroupSetting:
                    Messenger.Default.Send(new NotificationMessage<ProductStruct>(this, SelectedProductStruct, nameof(ProductGroupSettingWindowViewModel)));
                    break;
            }
            Messenger.Default.Send(new NotificationMessage("CloseAddProductView"));
        }

        private void StartEditingAction()
        {
            IsEditing = true;
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommand()
        {
            GetRelatedDataCommand = new RelayCommand(GetRelatedDataAction);
            ProductSelected = new RelayCommand(ProductSelectedAction);
            FocusUpDownCommand = new RelayCommand<string>(FocusUpDownAction);
            StartEditingCommand = new RelayCommand(StartEditingAction);
        }

        private void RegisterFilter()
        {
            switch (addProEnum)
            {
                case AddProductEnum.ProductPurchase:
                    FilterCommand = new RelayCommand(ProductPurchaseFilterAction);
                    break;
            }
        }
        private void ProductIsOTCFilter(object sender, FilterEventArgs e)
        {
            if (OrderTypeIsOTC == "OTC")
            {
                if (((ProductStruct)e.Item).Type == ProductTypeEnum.NHIMedicine)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
            else if (OrderTypeIsOTC == "藥品") {
                if (((ProductStruct)e.Item).Type == ProductTypeEnum.OTCMedicine || ((ProductStruct)e.Item).Type == ProductTypeEnum.Deposit)
                    e.Accepted = false;
                else
                    e.Accepted = true;
            }
            else
                e.Accepted = true;
        }

        private void AddFilter()
        {
            switch (addProEnum)
            {
                case AddProductEnum.ProductReturn:
                    ProStructCollectionViewSource.Filter += ProductReturnFilter;
                    break;

                case AddProductEnum.Trade:
                    ProStructCollectionViewSource.Filter += ProductTradeFilter;
                    break;
            }
        }

        #endregion ----- Define Functions -----
    }
}