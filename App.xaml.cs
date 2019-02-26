using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using His_Pos.FunctionWindow;

namespace His_Pos
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CheckAdministrator();
            StartupUri = new Uri("FunctionWindow/LoginWindow.xaml", UriKind.RelativeOrAbsolute);
        }
        private void CheckAdministrator()
        { 
            var wp = new WindowsPrincipal(WindowsIdentity.GetCurrent()); 
             
            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
                
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message,Class.MessageType.ERROR);
                }
                Environment.Exit(0);
            }
        }
    }
}
