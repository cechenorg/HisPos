﻿#pragma checksum "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2FC7368F0D9BEAE835C30CE77DCA21C944D87CAE97CC1EEED062BE62DC2130A4"
//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTaking;
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


namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.AuthenticationManage {
    
    
    /// <summary>
    /// AuthenticationManageView
    /// </summary>
    public partial class AuthenticationManageView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 40 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton LeaveToggle;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid LeaveGrid;
        
        #line default
        #line hidden
        
        
        #line 168 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel LeaveConfirmStack;
        
        #line default
        #line hidden
        
        
        #line 173 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox AuthLeaveAllSelectCheckBox;
        
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
            System.Uri resourceLocater = new System.Uri("/His_Pos;component/system_tab/h4_basic_manage/authenticationmanage/authentication" +
                    "manageview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
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
            this.LeaveToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 42 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
            this.LeaveToggle.Click += new System.Windows.RoutedEventHandler(this.AuthLeaveToggle);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LeaveGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 4:
            this.LeaveConfirmStack = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.AuthLeaveAllSelectCheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 176 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
            this.AuthLeaveAllSelectCheckBox.Click += new System.Windows.RoutedEventHandler(this.AuthLeaveAllSelectCheckBox_OnClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 204 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LeaveConfirm_OnClick);
            
            #line default
            #line hidden
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
            case 3:
            
            #line 90 "..\..\..\..\..\SYSTEM_TAB\H4_BASIC_MANAGE\AuthenticationManage\AuthenticationManageView.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Click += new System.Windows.RoutedEventHandler(this.AuthLeave_OnClick);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

