using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UI_Test_TIMESERVICE.DTO;

namespace UI_Test_TIMESERVICE
{
    static class DownloadCSV
    {
        public static bool Download(DateTime date)
        {
            try
            {
                string da = date.ToString("yyyy-MM-dd");
                var value = da.Split('-');
                string year = value[0];        
                string month = value[1];
                string day = value[2];
                AppInfor appInfor = new AppInfor();
                string url = appInfor.url_server + month + @"\" + "log-" + year + month + day + ".csv";
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(url, "log-" + year + month + day + ".csv");
                }
            }
            catch (Exception ex) {
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                throw ex;
            }
            
                return true;
        }
    }
}
