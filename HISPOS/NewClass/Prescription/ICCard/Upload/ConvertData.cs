using System.Text;

namespace His_Pos.NewClass.Prescription.ICCard.Upload
{
    public class ConvertData
    {
        //讀取INI轉安寧中文
        public string funPeacefulCode(string sPCode)
        {
            string sOrganDonate = "";

            switch (sPCode)
            {
                case "1":
                    sOrganDonate = "(1) 同意器官捐贈";
                    break;

                case "2":
                    sOrganDonate = "(2) 同意安寧緩和醫療";
                    break;

                case "3":
                    sOrganDonate = "(3) 同意不施行心肺復甦術";
                    break;

                case "4":
                    //sOrganDonate = "(4) 同意器官捐贈、同意安寧緩和醫療、同意不施行心肺復甦術";
                    sOrganDonate = "(4) 同意器官捐贈、同意安寧緩和醫療、同意不施行心肺復甦術、同意不施行維生醫療(舊)";
                    break;

                case "5":
                    sOrganDonate = "(5) 同意器官捐贈、同意安寧緩和醫療";
                    break;

                case "6":
                    sOrganDonate = "(6) 同意器官捐贈、同意不施行心肺復甦術";
                    break;

                case "7":
                    //sOrganDonate = "(7) 同意安寧緩和醫療、同意不施行心肺復甦術";
                    sOrganDonate = "(7) 同意安寧緩和醫療、同意不施行心肺復甦術、同意不施行維生醫療(舊)";
                    break;

                case "A":
                    sOrganDonate = "(A) 同意不施行維生醫療";
                    break;

                case "B":
                    sOrganDonate = "(B) 同意器官捐贈、同意不施行維生醫療";
                    break;

                case "C":
                    sOrganDonate = "(C) 同意安寧緩和醫療、同意不施行維生醫療";
                    break;

                case "D":
                    sOrganDonate = "(D) 同意不施行心肺復甦術、同意不施行維生醫療";
                    break;

                case "E":
                    sOrganDonate = "(E) 同意器官捐贈、同意安寧緩和醫療、同意不施行心肺復甦術、同意不施行維生醫療";
                    break;

                case "F":
                    sOrganDonate = "(F) 同意器官捐贈、同意安寧緩和醫療、同意不施行維生醫療";
                    break;

                case "G":
                    sOrganDonate = "(G) 同意器官捐贈、同意不施行心肺復甦術、同意不施行維生醫療";
                    break;

                case "H":
                    sOrganDonate = "(H) 同意安寧緩和醫療、同意不施行心肺復甦術、同意不施行維生醫療";
                    break;

                case "I":
                    sOrganDonate = "(I) 同意器官捐贈、同意安寧緩和醫療、同意不施行心肺復甦術";
                    break;

                case "J":
                    sOrganDonate = "(J) 同意安寧緩和醫療、同意不施行心肺復甦術";
                    break;

                default:
                    sOrganDonate = "未表示";
                    break;
            }

            return sOrganDonate;
        }

        //身分註記轉中文
        public string funNoteTheIdentityCode(string sNCode)
        {
            string sNote = "";

            switch (sNCode)
            {
                case "1":
                    sNote = "福民";
                    break;

                case "2":
                    sNote = "榮民";
                    break;

                case "3":
                    sNote = "一般";
                    break;

                default:
                    sNote = "";
                    break;
            }

            return sNote;
        }

        //byte轉字串
        public static string ByToString(byte[] bytes, int Start, int Num)
        {
            string data;
            data = "";
            data = System.Text.Encoding.GetEncoding(950).GetString(bytes, Start, Num);
            return data;
        }

        public string SubStr(string a_SrcStr, int a_StartIndex, int a_Cnt)
        {
            Encoding l_Encoding = Encoding.GetEncoding("UTF8", new EncoderExceptionFallback(), new DecoderReplacementFallback(""));
            byte[] l_byte = l_Encoding.GetBytes(a_SrcStr);
            if (a_Cnt <= 0)
                return "";
            //例若長度10
            //若a_StartIndex傳入9 -> ok, 10 ->不行
            if (a_StartIndex + 1 > l_byte.Length)
                return "";
            else
            {
                if (a_StartIndex + a_Cnt > l_byte.Length)
                    a_Cnt = l_byte.Length - a_StartIndex;
            }
            return l_Encoding.GetString(l_byte, a_StartIndex, a_Cnt);
        }

        public static byte Asc(string s)
        {
            return Encoding.ASCII.GetBytes(s.ToCharArray(), 0, 1)[0];
        }

        //字串轉btyes
        public static byte[] StringToBytes(string newString, int len)
        {
            int byteLength = newString.Length;
            byte[] bytes = new byte[len];
            //string hex;
            for (int i = 0; i < byteLength; i++)
            {
                bytes[i] = Asc(newString.Substring(i, 1));
            }
            return bytes;
        }
    }
}