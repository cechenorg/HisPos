﻿#pragma checksum "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5670436093D0F428545F97EA0B6C0E5A3ECFC101596E163E3220C23A7515DA0F"
//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow {
    
    
    /// <summary>
    /// MedBagSelectionWindow
    /// </summary>
    public partial class MedBagSelectionWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainGrid;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Back;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MultiMode;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SingleMode;
        
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
            System.Uri resourceLocater = new System.Uri("/His_Pos;component/system_tab/h1_declare/prescriptiondeclare/functionwindow/medba" +
                    "gselectionwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
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
            this.MainGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.Back = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
            this.Back.Click += new System.Windows.RoutedEventHandler(this.Back_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MultiMode = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
            this.MultiMode.Click += new System.Windows.RoutedEventHandler(this.MultiMode_Click);
            
            #line default
            #line hidden
            
            #line 55 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
            this.MultiMode.KeyDown += new System.Windows.Input.KeyEventHandler(this.MultiMode_KeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.SingleMode = ((System.Windows.Controls.Button)(target));
            
            #line 78 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
            this.SingleMode.Click += new System.Windows.RoutedEventHandler(this.SingleMode_Click);
            
            #line default
            #line hidden
            
            #line 79 "..\..\..\..\..\..\SYSTEM_TAB\H1_DECLARE\PrescriptionDeclare\FunctionWindow\MedBagSelectionWindow.xaml"
            this.SingleMode.KeyDown += new System.Windows.Input.KeyEventHandler(this.SingleMode_KeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

