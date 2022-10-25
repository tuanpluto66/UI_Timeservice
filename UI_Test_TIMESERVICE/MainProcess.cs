using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI_Test_TIMESERVICE.DTO;

namespace UI_Test_TIMESERVICE
{
    internal class MainProcess
    {
        public void MainLogic()
        {
            LogHelper.InfoFormat("ACTION START");           // ACTION START

            DatabaseHelper databaseHelper = DatabaseHelper.getInstance(AppInfor.ConnectionString);
            MySqlConnection mySqlConnection = databaseHelper.GetMysqlConnection();
            log4net.Config.XmlConfigurator.Configure();
            List<Employee> employees;
            List<Calendar> calendars;
            List<DateTime> list_date_startd_toNow;
            List<DateTime> list_date_timesheet_status;
            List<DateTime> list_date_difference;
            List<DateTime> list_date_insert_timesheet;          
            List<DateTime> list_date_log_title = new List<DateTime>();

            System.DateTime enddate = AppInfor.Date.Date;
            System.DateTime startdate = System.DateTime.Parse(AppInfor.Startdate).Date;
            bool isSuccess = true;

            // 1. Get data employees from db_sanze
            employees = DatabaseHelper.Getemployee();

            // 2. Get data calendars  from db_sanze
            calendars = DatabaseHelper.GetCalender();

            // 8.Validate data employees, calendars
            if (!ValidateHelper.ValidateEmployees(employees) || !ValidateHelper.ValidateCalender(calendars)) return;


            // 3. Lấy ds ngày chưa được insert vào db_sanze => insert với status = 0
            // Lấy ngày bắt đầu startdate từ app.config
            list_date_startd_toNow = DateArray.GetDatesBetween(startdate, enddate.AddDays(-2));
            if (list_date_startd_toNow == null) return;

            list_date_timesheet_status = DatabaseHelper.Getdate_database();

            list_date_difference = list_date_startd_toNow.Get_list_date_difference(list_date_timesheet_status, a2 => a2, a1 => a1).ToList();

            // 4. Insert ds ngày không tồn tại trong timesheet_status từ ds ngày start_date into db_sanze
            //mySqlConnection.Open();
            var transaction = mySqlConnection.BeginTransaction();
            if (!DatabaseHelper.Inser_list_date(list_date_difference)) return;
            transaction.Commit();
            // 5. Lấy ds ngày cần thêm timesheet
            list_date_insert_timesheet = DatabaseHelper.Getdate_insert_timesheet().OrderBy(p=>p.Date).ToList();

            // 6. Get list file log

            // 6.1 Get list date have getting logs
            List<DateTime> dateTimes = GetListDateHaveToGetLog(list_date_insert_timesheet);

            // 6.1 Download file log from server
            if (!DownloadLog(dateTimes)) return;

            // 6.1 Read log from file
            List<Log> lstLog = GetLog(list_date_insert_timesheet, employees);
            if (lstLog.Count == 0)
            {
                LogHelper.Warn("DATA LOGS EMPTY !!!!!!!");
            }
            // 6.2 Get list Day only has title
            list_date_log_title = Get_lst_date_title_logs(list_date_insert_timesheet, employees);

         // 6.2 Insert log into log table 
            if (lstLog.Count > 0)
            {
                transaction = mySqlConnection.BeginTransaction();
                // delete Logs
                isSuccess = InsertLogToDatabase(lstLog);
                if (!isSuccess)
                {
                    return;
                }
                transaction.Commit();
            }

            // 7. Main process
            // 7.1 Read log from database
            
            List<Log> logs = DatabaseHelper.GetLogsByListDate(dateTimes);
            // 7.2 Create data time sheet
            List<Timesheet> lstTimeSheet = GetAllTimeSheet(logs, employees, dateTimes);
            if (lstTimeSheet.Count == 0) return;
            // 7.3 Insert data time sheet into database

            // 7.3.1 Delete old Time Sheet 
            transaction = mySqlConnection.BeginTransaction();
            if (!DeleteTimeSheetByDate(list_date_insert_timesheet))
            {
                LogHelper.Error("DELETE DATA TIME SHEET EXISTED BEFORE ERROR!");
                return;
            }

            // 7.3.2 Delete old Time Sheet 
            if (!InsertTimeSheetToDB(lstTimeSheet, list_date_insert_timesheet))
            {
                LogHelper.Error("INSERT DATA TIMESHEET ERROR!");
                return;
            }
            if (!Updatesatustimesheet_log_title(list_date_log_title)) 
            {
                LogHelper.Error("UPDATE STATUS TIMSHEET LOG_TITLE ERROR!");
                return;
            }
            transaction.Commit();
            databaseHelper.Dispose();

            LogHelper.InfoFormat("ACTION COMPLETE");            // ACTION COMPLETE
        }

        private bool DeleteTimeSheetByDate(List<DateTime> lstTimeSheet)
        {
            // 11. Delete timsheet from table timsheet (day_of_year, timesheet_status = 0)
            if (!DatabaseHelper.Delete_timsheet(lstTimeSheet)) return false;
            return true;
        }

        private bool InsertTimeSheetToDB(List<Timesheet> lstTimeSheet, List<DateTime> dateTimes)
        {
            List<Timesheet> timeSheetByDay;
            foreach (DateTime dt in dateTimes)
            {
                timeSheetByDay = lstTimeSheet.Where(p => p.Date.Equals(dt.ToString("yyyy-MM-dd"))).ToList();
                if (timeSheetByDay.Count > 0 && DatabaseHelper.InsertTimeSheet(timeSheetByDay))
                {
                    //  Update status = 1, timesheet_status
                    if (!DatabaseHelper.Update_status_timesheet(dt)) return false;
                }

            }
            return true;
        }
        private bool Updatesatustimesheet_log_title(List<DateTime> dateTimes) 
        {
            foreach (DateTime dt in dateTimes) { 
                if(!DatabaseHelper.Update_status_timesheet(dt))return false;
            }
            return true;
        }

        private List<Timesheet> GetAllTimeSheet(List<Log> logs, List<Employee> employees, List<DateTime> dateTimes)
        {
            List<DTO.Timesheet> timesheets = new List<Timesheet>();
            List<DTO.Timesheet> timesheetsbyday = new List<Timesheet>();
            List<Log> logByDay = new List<Log>();
            foreach (var dt in dateTimes)
            {
                logByDay = logs.Where(p => (p.Day_of_year == dt.ToString("yyyy-MM-dd")) || (p.Day_of_year == dt.AddDays(1).ToString("yyyy-MM-dd"))).ToList();
                if (logByDay == null || (logByDay != null && logByDay.Count == 0)
                    || (!logByDay.Exists(p => p.Day_of_year == dt.ToString("yyyy-MM-dd"))))
                {
                    continue;
                }
                // 10.Get list time sheet by day
                timesheetsbyday = TimeSheetHelper.GetTimeSheetByDay(logByDay, employees, dt);
                if (timesheetsbyday.Count > 0)
                {
                    timesheets.AddRange(timesheetsbyday);
                }

            }
            return timesheets;
        }

        private List<DateTime> GetListDateHaveToGetLog(List<DateTime> list_date_insert_timesheet)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            foreach (var dt in list_date_insert_timesheet)
            {
                if (!dateTimes.Exists(p => p.Date == dt))
                {
                    dateTimes.Add(dt);
                }
                if (!dateTimes.Exists(p => p.Date == dt.AddDays(1)))
                {
                    dateTimes.Add(dt.AddDays(1));

                }
            }
            return dateTimes;
        }


        private bool InsertLogToDatabase(List<Log> logs)
        {
            try
            {
                 DatabaseHelper.InsertLogstoDB(logs);
            }
            catch (Exception ex)
            {
                LogHelper.Error("INSERT LOG DATA INTO DATABASE ERROR!");
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                return false;
            }
            return true;
        }
        private bool DownloadLog(List<DateTime> dateTimes) 
        {
            int flag = Convert.ToInt32(AppInfor.Flag);
            try
            {
                if (flag == 0)
                {
                    // download file log from server
                    if (!DownloadCSV.DownLoadFromServer(dateTimes)) return false;
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                return false;               
            }
                return true;
        }
        private List<Log> GetLog(List<DateTime> list_date_insert_timesheet, List<Employee> employees)
        {
            List<Log> lstLogs = new List<Log>();
            try 
            { 
                // read Logs from files
                lstLogs = GetAllLog(list_date_insert_timesheet, employees);
            }
            catch (Exception ex)
            {
                LogHelper.Error("INSERT LOG DATA INTO DATABASE ERROR!");
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
            }
            return lstLogs;
        }

        private List<Log> GetAllLog(List<DateTime> list_date_insert_timesheet, List<Employee> employees)
        {
            int flag = Convert.ToInt32(AppInfor.Flag);
            List<Log> logs_today = null;
            List<Log> log_next_day = null;
            List<Log> allLogs = new List<Log>();              
            try
            {
                foreach (DateTime dt in list_date_insert_timesheet)
                {
                    if (!allLogs.Exists(p => p.Day_of_year == dt.ToString("yyyy-MM-dd")))
                    {
                        logs_today = CSVHelper.GetLogs(AppInfor.GetFileLogPath(dt), employees);
                        if (logs_today != null && logs_today.Count > 0)
                        {
                            allLogs.AddRange(logs_today);
                        }                      
                    }

                    if (!allLogs.Exists(p => p.Day_of_year == dt.AddDays(1).ToString("yyyy-MM-dd")))
                    {
                        log_next_day = CSVHelper.GetLogs(AppInfor.GetFileLogPath(dt.AddDays(1)), employees);
                        if (log_next_day != null && log_next_day.Count > 0)
                        {
                            allLogs.AddRange(log_next_day);
                        }                    
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("INSERT LOG DATA INTO DATABASE ERROR!");
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
            }

            return allLogs;
        }
        private List<DateTime> Get_lst_date_title_logs(List<DateTime> list_date_insert_timesheet, List<Employee> employees)
        {        
            List<Log> logs_today = null;
            List<Log> log_next_day = null;
            
            List<DateTime> lst_title = new List<DateTime>();
            try
            {
                foreach (DateTime dt in list_date_insert_timesheet)
                {
                        logs_today = CSVHelper.GetLogs(AppInfor.GetFileLogPath(dt), employees);
                    if (logs_today != null && logs_today.Count > 0)
                    {
                        continue;
                    }
                    else if (logs_today == null) 
                    {
                        continue;
                    }
                    else
                        {
                            if (!lst_title.Contains(dt))
                            {
                                lst_title.Add(dt);
                            }
                        }          
                        
                       log_next_day = CSVHelper.GetLogs(AppInfor.GetFileLogPath(dt.AddDays(1)), employees);
                        if (log_next_day != null && log_next_day.Count > 0)
                        {
                        continue;
                    }
                        else if (log_next_day == null)
                        {
                        continue;
                    }
                         else
                        {
                            lst_title.Add(dt.AddDays(1));
                         }
                    }
                }          
            catch (Exception ex)
            {              
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
            }
            return lst_title;
        }      
    }
}

