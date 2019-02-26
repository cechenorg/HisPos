using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace His_Pos.Service {
    public  class FtpMethod {
        private string FtpUrl { get; set; }
        private string Account { get; set; }
        private string Password { get; set; }
        public  FtpMethod(string ftpUrl,string account,string password) {
            FtpUrl = ftpUrl;
            Account = account;
            Password = password;
        }
        public void Getfile(string downloadpath) {
            try {
                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(Account, Password);
                    byte[] fileData = request.DownloadData(FtpUrl);

                    using (FileStream file = File.Create(downloadpath))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
            }
            catch (Exception ex) {
                NewFunction.ExceptionLog(ex.Message);
            }
        }
    }
}
