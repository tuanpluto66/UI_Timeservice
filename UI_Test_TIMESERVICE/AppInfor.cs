using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeService
{
    public class AppInfor
    {
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["getconnectionstring"];
            }
        }
        public string Url_server
        {
            get
            {
                return ConfigurationManager.AppSettings["url_server"];
            }
        }
        public string Server_name
        {
            get
            {
                return ConfigurationManager.AppSettings["server_name"];
            }
        }
        public string Domain_computername
        {
            get
            {
                return ConfigurationManager.AppSettings["domain_computername"];
            }
        }
        public string User
        {
            get
            {
                return ConfigurationManager.AppSettings["user"];
            }
        }
        public string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["password"];
            }
        }
        public static string TimeToRunService
        {
            get
            {
                return ConfigurationManager.AppSettings["timetorun"];
            }
        }
        public static string Blockforminute
        {
            get
            {
                return ConfigurationManager.AppSettings["block"];
            }
        }
        public static string Folder_name
        {
            get
            {
                return ConfigurationManager.AppSettings["folder_name"];
            }
        }
        public static DateTime Date
        {
            get
            {
                return DateTime.Now;
            }
        }

        public static string Flag
        {
            get
            {
                return ConfigurationManager.AppSettings["flag"];
            }
        }
        public static string Local_path_logs {
            get {
                return ConfigurationManager.AppSettings["pathlogs"];
            }
        }
        public static string GetfileInfor(DateTime date)
        {
            string da = date.ToString("yyyyMMdd");          
            return "log-" + da + ".csv";
        }
        public static string Getmonth_folder_log(DateTime date)
        {
            string da = date.ToString("MM");
            return da;
        }

    }

}
