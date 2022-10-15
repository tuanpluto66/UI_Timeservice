using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeService;
using UI_Test_TIMESERVICE.DTO;

namespace UI_Test_TIMESERVICE
{
    //employees = CSVHelper.GetEmployees("D:\\Test_T9\\employee.csv");
    //calendars = CSVHelper.GetCalendars("D:\\Test_T9\\holiday2022.csv");
    //if (!DBHelper.UpdateEmployee(employees) || !DBHelper.UpdateCalendar(calendars)) return;
    internal class MainProcess
    {
        public void MainLogic()
        {
            log4net.Config.XmlConfigurator.Configure();

            LogHelper.InfoFormat("ACTION START");            // ACTION START


            List<Employee> employees;
            List<Calendar> calendars;
            List<Log> logs_today;
            List<Log> logs_before_yesterday;
            List<DateTime> date;
            List<DateTime> date_db;
            //List<Log> logs_yesterday;

            //List<Timesheet> timesheets_today;
            List<Timesheet> timesheets_before_yesterday;// output use to update db

            System.DateTime enddate = AppInfor.Date;
            System.DateTime startdate = System.DateTime.Parse(AppInfor.Startdate);

            // 1. Get data employees from db_sanze
            employees = DBHelper.Getemployee();

            // 2. Get data calendars  from db_sanze
            calendars = DBHelper.getCalender();

            // Lấy ds ngày cần thực hiện
            // Lấy ngày bắt đầu startdate từ app.config
            //DateTime[] dates = DateArray.GetDatesBetween(startdate, enddate.AddDays(-2)).ToArray();
            date = DateArray.GetDatesBetween(startdate, enddate.AddDays(-2));

            date_db = DBHelper.Getdate();
            //DateTime[] datedb = DBHelper.Getdate().ToArray();
            
            List<DateTime> result  = date.FindNewItems(date_db, a2 => a2, a1 => a1).ToList();

            if (!DBHelper.Inser_list_date(result)) return;

            //3.Download log.csv from server
            int flag = Convert.ToInt32(AppInfor.Flag);
            if (flag == 0)
            {
                if (DownloadCSV.Download(startdate))
                {
                    logs_today = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.Folder_name + @"\" + AppInfor.Get_year_folder_log(startdate.AddDays(-1)) + @"\" + AppInfor.Getmonth_folder_log(startdate.AddDays(-1)) + @"\" + AppInfor.GetfileInfor(startdate.AddDays(-1)), employees);
                }
                else
                {
                    return;
                }
            }
            else if (flag == 1)
            {
                logs_today = CSVHelper.GetLogs(AppInfor.Local_path_logs + AppInfor.Get_year_folder_log(startdate.AddDays(-1)) + @"\" + AppInfor.Getmonth_folder_log(startdate.AddDays(-1)) + @"\" + AppInfor.GetfileInfor(startdate.AddDays(-1)), employees);
            }
            else
            {
                return;
            }

            //5.Validate data employees, calendars, logs
            if (!ValidateHelper.ValidateEmployees(employees) || !ValidateHelper.ValidateCalender(calendars) || !ValidateHelper.ValidateLogs(logs_today)) return;

            //6.Update logs into db_sanze
            if (!DBHelper.InsertLogstoDB(logs_today)) return;

            //7.Get logs from database
            //logs_today = DBHelper.Getlogs(date);
            //logs_yesterday = DBHelper.Getlogs(date.AddDays(-1));
            logs_before_yesterday = DBHelper.Getlogs(startdate.AddDays(-2));

            //8.Get list time sheet by day
            timesheets_before_yesterday = TimeSheetHelper.GetTimeSheetByDay(logs_before_yesterday, employees, startdate.AddDays(-2));
            //timesheets_today = TimeSheetHelper.GetTimeSheetByDay(logs_today, employees, date);

            //9.Update database
            //if (!DBHelper.UpdateTimeSheet_Yesterday(timesheets_yesterday) || !DBHelper.InsertTimeSheet_Today(timesheets_today)) return;
            if (!DBHelper.InsertTimeSheet_before_yesterday(timesheets_before_yesterday)) return;

            if (!DBHelper.Update_check_ts(startdate)) return;

            //int numberday = DateTime.DaysInMonth(AppInfor.Getyear(datetest), AppInfor.Getmonth(datetest));

            //for(int i = 1; i < numberday + 1; i++)
            //{
            //    if (i < 10)
            //    {
            //        logs_today = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.Getfiletest(datetest) + "0" + i + ".csv", employees);
            //    }
            //    else {
            //        // 4. Get data log  from CSV
            //        logs_today = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.Getfiletest(datetest) + i + ".csv", employees);
            //    }

            //    // 5. Validate data employees,calendars,logs
            //    if (!ValidateHelper.ValidateEmployees(employees) || !ValidateHelper.ValidateCalender(calendars) || !ValidateHelper.ValidateLogs(logs_today)) return;

            //    //6. Update logs into db_sanze
            //    if (!DBHelper.InsertLogstoDB(logs_today)) return;

            //    //7. Get logs from database
            //    logs_today = DBHelper.Getlogs(datetest.AddDays(i-1));
            //    logs_yesterday = DBHelper.Getlogs(datetest.AddDays(i-2));

            //    // 8. Get list time sheet by day           
            //    timesheets_yesterday = TimeSheetHelper.GetTimeSheetByDay(logs_yesterday, employees, datetest.AddDays(i-2));
            //    timesheets_today = TimeSheetHelper.GetTimeSheetByDay(logs_today, employees, datetest.AddDays(i-1));

            //    // 9. Update database
            //    if (!DBHelper.UpdateTimeSheet_Yesterday(timesheets_yesterday) || !DBHelper.InsertTimeSheet_Today(timesheets_today)) return;
            //    //if (!DBHelper.InsertTimeSheet_Today(timesheets_today)) return;
            //}


            LogHelper.InfoFormat("ACTION COMPLETE");            // ACTION COMPLETE



        }
    }
}

