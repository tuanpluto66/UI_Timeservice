using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace UI_Test_TIMESERVICE
{
    public class AppInfor
    {
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["Getconnectionstring"];
            }
        }
        public string url_server
        {
            get
            {
                return ConfigurationManager.AppSettings["url_server"];
            }
        }
        public static string pathEmployeeCSV
        {
            get
            {
                return ConfigurationManager.AppSettings["pathEmployeeCSV"];
            }
        }
        public static string pathCalendarCSV
        {
            get
            {
                return ConfigurationManager.AppSettings["pathCalendarCSV"];
            }
        }
       

        public static string TimeToRunService
        {
            get
            {
                return ConfigurationManager.AppSettings["TimeToRun"];
            }
        }
        public static string Blockforminute
        {
            get
            {
                return ConfigurationManager.AppSettings["block"];
            }
        }
        public static DateTime Date
        {
            get
            {
                return DateTime.Now;
            }
        }
        public static string datetest
        {
            get
            {
                return ConfigurationManager.AppSettings["datetest"];
            }
        }
        public static string GetfileInfor( DateTime date)
        {
            string da = date.ToString("yyyy-MM-dd");
            var value = da.Split('-');
            string year = value[0];
            string month = value[1];
            string day = value[2];
            return "log-" + year + month + day + ".csv";
        }

    }
}
