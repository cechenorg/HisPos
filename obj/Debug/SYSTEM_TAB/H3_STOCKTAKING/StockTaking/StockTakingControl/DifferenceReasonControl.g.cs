﻿#pragma checksum "..\..\..\..\..\..\SYSTEM_TAB\H3_STOCKTAKING\StockTaking\StockTakingControl\DifferenceReasonControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E0B33F2C883A03A2F9943F1D5658C2C3D969D60501C9EF7C0AFECFB9DF505892"
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
using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking.StockTakingControl;
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


namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking.StockTakingControl {
    
    
    /// <summary>
    /// DifferenceReasonControl
    /// </summary>
    public partial class DifferenceReasonControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 16 "..\..\..\..\..\..\SYSTEM_TAB\H3_STOCKTAKING\StockTaking\StockTakingControl\DifferenceReasonControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ReasonDataGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/His_Pos;component/system_tab/h3_stocktaking/stocktaking/stocktakingcontrol/diffe" +
                    "rencereasoncontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\SYSTEM_TAB\H3_STOCKTAKING\StockTaking\StockTakingControl\DifferenceReasonControl.xaml"
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
            this.ReasonDataGrid = ((System.Windows.Controls.DataGrid)(target));
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
            switch (connectionId)
            {
            case 2:
            
            #line 155 "..\..\..\..\..\..\SYSTEM_TAB\H3_STOCKTAKING\StockTaking\StockTakingControl\DifferenceReasonControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.InputTextbox_OnGotFocus);
            
            #line default
            #line hidden
            
            #line 158 "..\..\..\..\..\..\SYSTEM_TAB\H3_STOCKTAKING\StockTaking\StockTakingControl\DifferenceReasonControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.DoubleTextBox_OnKeyDown);
            
            #line default
            #line hidden
            
            #line 159 "..\..\..\..\..\..\SYSTEM_TAB\H3_STOCKTAKING\StockTaking\StockTakingControl\DifferenceReasonControl.xaml"
            ((System.Windows.Controls.TextBox)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.InputTextbox_OnPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

