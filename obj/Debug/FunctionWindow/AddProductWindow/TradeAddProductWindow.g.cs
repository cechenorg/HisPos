﻿#pragma checksum "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "BAD1E59344B6B796D5B25135F6FA4DCAD4A61918E93421FD92381543A844AE91"
//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using His_Pos.Behaviors;
using His_Pos.FunctionWindow.AddCustomerWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.GeneralCustomControl;
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


namespace His_Pos.FunctionWindow.AddProductWindow {
    
    
    /// <summary>
    /// TradeAddProductWindow
    /// </summary>
    public partial class TradeAddProductWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 127 "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ResultGrid;
        
        #line default
        #line hidden
        
        
        #line 186 "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnConfirm;
        
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
            System.Uri resourceLocater = new System.Uri("/His_Pos;component/functionwindow/addproductwindow/tradeaddproductwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml"
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
            this.ResultGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 127 "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml"
            this.ResultGrid.Loaded += new System.Windows.RoutedEventHandler(this.DataGrid_Loaded);
            
            #line default
            #line hidden
            
            #line 127 "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml"
            this.ResultGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.ResultGrid_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnConfirm = ((System.Windows.Controls.Button)(target));
            
            #line 186 "..\..\..\..\FunctionWindow\AddProductWindow\TradeAddProductWindow.xaml"
            this.btnConfirm.Click += new System.Windows.RoutedEventHandler(this.btnConfirm_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

