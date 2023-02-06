using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Service
{
    internal class HISPOSWebApiService
    {

        public bool SyncData()
        {
            WebClient webClient = new WebClient();

            webClient.DownloadStringTaskAsync("");


            return false;
        }

    }
}
