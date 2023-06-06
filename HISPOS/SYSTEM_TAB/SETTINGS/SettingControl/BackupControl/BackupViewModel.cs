using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.BackupControl
{
    public class BackupViewModel : ViewModelBase
    {
        public BackupViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFileAction);
            ConfirmCommand = new RelayCommand(ConfirmAction);
        }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }

        private string displayFilePath = Properties.Settings.Default.BackupPath;
        public string DisplayFilePath
        {
            get { return displayFilePath; }
            set
            {
                Set(() => DisplayFilePath, ref displayFilePath, value);
            }
        }
        private void OpenFileAction()
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                DisplayFilePath = fdlg.SelectedPath;
                ConfirmChangeAction();
            }
        }
        private void ConfirmChangeAction()
        {
            string filePath = "C:\\Program Files\\HISPOS\\settings.singde";
            string leftLines = "";
            using (StreamReader fileReader = new StreamReader(filePath))
            {
                leftLines = fileReader.ReadLine();
            }
            Properties.Settings.Default.BackupPath = DisplayFilePath;
            using (TextWriter fileWriter = new StreamWriter(filePath, false))
            {
                fileWriter.WriteLine(leftLines);

                fileWriter.WriteLine("M " + Properties.Settings.Default.MedBagPrinter);
                fileWriter.WriteLine("Rc " + Properties.Settings.Default.ReceiptPrinter);
                fileWriter.WriteLine("Rp " + Properties.Settings.Default.ReportPrinter);
                fileWriter.WriteLine("Com " + Properties.Settings.Default.ReaderComPort);
                fileWriter.WriteLine("ICom " + Properties.Settings.Default.InvoiceComPort);
                fileWriter.WriteLine("INum " + Properties.Settings.Default.InvoiceNumber);
                fileWriter.WriteLine("IChk " + Properties.Settings.Default.InvoiceCheck);
                fileWriter.WriteLine("INumS " + Properties.Settings.Default.InvoiceNumberStart);
                fileWriter.WriteLine("INumC " + Properties.Settings.Default.InvoiceNumberCount);
                fileWriter.WriteLine("INumE " + Properties.Settings.Default.InvoiceNumberEng);
                fileWriter.WriteLine("PP " + Properties.Settings.Default.PrePrint);
                fileWriter.WriteLine("RPF " + Properties.Settings.Default.ReportFormat);
                fileWriter.WriteLine("BAK " + Properties.Settings.Default.BackupPath);
            }
        }
        private void ConfirmAction()
        {
            if (string.IsNullOrEmpty(DisplayFilePath))
            {
                MessageWindow.ShowMessage("請選擇備份路徑", MessageType.WARNING);
                return;
            }

            if (!Directory.Exists(displayFilePath))
            {
                Directory.CreateDirectory(displayFilePath);
            }
            string path = @"C:\Program Files\HISPOS\SQLBackup\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = Directory.GetFiles(@"SQLBackup");
            string file = string.Empty;
            if (files != null && files.Length > 0)
                file = files[0];

            string outputFilePath = string.Format(@"C:\Program Files\HISPOS\{0}", file);
            string output = string.Empty;
            try
            {
                string sqlContent = File.ReadAllText(files[0]);
                string modifiedSqlContent = sqlContent.Replace("/var/opt/mssql/data/backup", displayFilePath);
                modifiedSqlContent = modifiedSqlContent.Replace("LocalNewDB", Properties.Settings.Default.SystemSerialNumber);
                File.WriteAllText(outputFilePath, modifiedSqlContent);

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string sqlfile = path + "backup_db_full.sql";
                string cmdText = string.Format(@"sqlcmd.exe -U sa -P ""123456a@"" -S ""127.0.0.1, 1433"" -d {0} -i ""{1}""", Properties.Settings.Default.SystemSerialNumber, sqlfile);
                process.StandardInput.WriteLine(cmdText);
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
                output = process.StandardOutput.ReadToEnd();
                process.Close();
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("備份失敗", MessageType.WARNING);
            }
            finally
            {
                if (Directory.Exists(path))
                {
                    string msg = string.Empty;
                    string filePath = displayFilePath + string.Format(@"\Full_{0}.bak", DateTime.Today.ToString("yyyyMMdd"));
                    if (File.Exists(filePath))
                    {
                        msg = "備份完成";
                        MessageWindow.ShowMessage(msg, MessageType.SUCCESS);
                    }
                    else
                    {
                        string[] lines = output.Split(new string[] {"\r\n" }, StringSplitOptions.None);
                        msg = "備份失敗";
                        NewFunction.ExceptionLog(lines[6]);
                        MessageWindow.ShowMessage(msg, MessageType.WARNING);
                    }

                    Directory.Delete(path, true);
                }
            }
        }
    }
}
