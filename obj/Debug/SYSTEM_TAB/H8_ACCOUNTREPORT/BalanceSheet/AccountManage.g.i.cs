﻿#pragma checksum "..\..\..\..\..\SYSTEM_TAB\H8_ACCOUNTREPORT\BalanceSheet\AccountManage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "12D8682010E75C50B4E3D22B59328B3DD144C9F1D433ADBD94385583A2E2D069"
//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using ChromeTabs;
using ChromeTabs.Converters;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.OTCControl;
using His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet;
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


namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet {
    
    
    /// <summary>
    /// AccountManage
    /// </summary>
    public partial class AccountManage : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\..\..\SYSTEM_TAB\H8_ACCOUNTREPORT\BalanceSheet\AccountManage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCheckout;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\..\SYSTEM_TAB\H8_ACCOUNTREPORT\BalanceSheet\AccountManage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid AccountList;
        
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
            System.Uri resourceLocater = new System.Uri("/His_Pos;component/system_tab/h8_accountreport/balancesheet/accountmanage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\SYSTEM_TAB\H8_ACCOUNTREPORT\BalanceSheet\AccountManage.xaml"
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
            this.btnCheckout = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\..\..\SYSTEM_TAB\H8_ACCOUNTREPORT\BalanceSheet\AccountManage.xaml"
            this.btnCheckout.Click += new System.Windows.RoutedEventHandler(this.btnCheckout_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.AccountList = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

