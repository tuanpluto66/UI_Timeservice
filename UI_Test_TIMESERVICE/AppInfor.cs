﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI_Test_TIMESERVICE;

namespace UI_Test_TIMESERVICE
{
    public static class AppInfor
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["getconnectionstring"];
            }
        }
        public static string Url_server
        {
            get
            {
                return ConfigurationManager.AppSettings["url_server"];
            }
        }
        public static string Server_name
        {
            get
            {
                return ConfigurationManager.AppSettings["server_name"];
            }
        }
        public static string Domain_computername
        {
            get
            {
                return ConfigurationManager.AppSettings["domain_computername"];
            }
        }
        public static string User
        {
            get
            {
                return ConfigurationManager.AppSettings["user"];
            }
        }
        public static string Password
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
        public static string Startdate
        {
            get
            {
                return ConfigurationManager.AppSettings["startdate"];
            }
        }
        public static string Flag
        {
            get
            {
                return ConfigurationManager.AppSettings["flag"];
            }
        }
        public static string Local_path_logs
        {
            get
            {
                return ConfigurationManager.AppSettings["pathlogs"];
            }
        }
        public static string GetfileInfor(DateTime date)
        {
            string da = date.ToString("yyyyMMdd");
            return "log-" + da + ".csv";
        }
        public static string GetFileLogPath(DateTime date)
        {
            if (Convert.ToInt32(Flag) == 0)
            {
                return AppDomain.CurrentDomain.BaseDirectory + AppInfor.Folder_name + @"\" + AppInfor.Get_year_folder_log(date) + @"\" + AppInfor.Getmonth_folder_log(date) + @"\" + AppInfor.GetfileInfor(date);
            }
            else
            {
                return AppInfor.Local_path_logs + AppInfor.Get_year_folder_log(date) + @"\" + AppInfor.Getmonth_folder_log(date) + @"\" + AppInfor.GetfileInfor(date);
            }
        }
        public static string Getmonth_folder_log(DateTime date)
        {
            string da = date.ToString("MM");
            return da;
        }
        public static string Get_year_folder_log(DateTime date)
        {
            string da = date.ToString("yyyy");
            return da;
        }
    }

}
