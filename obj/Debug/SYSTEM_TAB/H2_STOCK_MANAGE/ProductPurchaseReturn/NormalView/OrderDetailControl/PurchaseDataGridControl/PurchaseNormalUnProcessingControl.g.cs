﻿#pragma checksum "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3BDC3C90DCE820F965B58A965525BE805A159F1888641D4AE90199CA0D9AE0F2"
//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using His_Pos.GeneralCustomControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using His_Pos.Service;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.PurchaseDataGridControl {
    
    
    /// <summary>
    /// PurchaseNormalUnProcessingControl
    /// </summary>
    public partial class PurchaseNormalUnProcessingControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 97 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ProductDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/His_Pos;component/system_tab/h2_stock_manage/productpurchasereturn/normalview/or" +
                    "derdetailcontrol/purchasedatagridcontrol/purchasenormalunprocessingcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ProductDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 2:
            
            #line 172 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.ProductIDTextbox_OnKeyDown);
            
            #line default
            #line hidden
            break;
            case 3:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Control.MouseDoubleClickEvent;
            
            #line 199 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.ShowDetail);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 4:
            
            #line 219 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.UIElement_OnGotFocus);
            
            #line default
            #line hidden
            
            #line 222 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnKeyDown);
            
            #line default
            #line hidden
            
            #line 223 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.UIElement_OnPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            case 5:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.MouseEnterEvent;
            
            #line 271 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseEventHandler(this.GetProductToolTip);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 6:
            
            #line 297 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.UIElement_OnGotFocus);
            
            #line default
            #line hidden
            
            #line 300 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnKeyDown);
            
            #line default
            #line hidden
            
            #line 301 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.UIElement_OnPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 319 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.UIElement_OnGotFocus);
            
            #line default
            #line hidden
            
            #line 322 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnKeyDown);
            
            #line default
            #line hidden
            
            #line 323 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.UIElement_OnPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 341 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.UIElement_OnGotFocus);
            
            #line default
            #line hidden
            
            #line 344 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnKeyDown);
            
            #line default
            #line hidden
            
            #line 345 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.UIElement_OnPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 370 "..\..\..\..\..\..\..\..\SYSTEM_TAB\H2_STOCK_MANAGE\ProductPurchaseReturn\NormalView\OrderDetailControl\PurchaseDataGridControl\PurchaseNormalUnProcessingControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.UIElement_OnKeyDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

