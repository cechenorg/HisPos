using His_Pos.Service;
using System;
using System.IO;
using System.Xml;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFile.CooperativeFtpFile
{
    public class CooperativeFtpFileDb
    {
        public XmlDocument GetData(DateTime dateTime)
        {
            string filepath = "MDA" + dateTime.AddYears(-1911).ToString("yyyMM"); //"MDA10710";
            string filename = filepath + ".ZIP";
            string path = @"C:\Program Files\HISPOS\CooperativeFtpFile\" + filepath;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FtpMethod ftpMethod = new FtpMethod(@"ftp://ftp_sd:ftp.sd123@www.ihis.com.tw/nhdata/" + VM.CooperativeInstitutionID + "/" + filename, "ftp_sd", "ftp.sd123");
            ftpMethod.Getfile(path + @"\" + filename);
            //NewFunction.Uncompress(path + @"\" + filename, path);
            XmlDocument xml = new XmlDocument();
            xml.Load(path + @"\TOTFA.xml");
            return xml;
        }
    }
}