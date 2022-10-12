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
    internal class MainProcess
    {
        public void MainLogic()
        {
            log4net.Config.XmlConfigurator.Configure();

            LogHelper.InfoFormat("ACTION START");            // ACTION START


            List<Employee> employees;
            List<Calendar> calendars;
            List<Log> logs_today;
            List<Log> logs_yesterday;

           

            List<Timesheet> timesheets_today;
            List<Timesheet> timesheets_yesterday;// output use to update db

            
            //employees = CSVHelper.GetEmployees("D:\\Test_T9\\employee.csv");
            //calendars = CSVHelper.GetCalendars("D:\\Test_T9\\holiday2022.csv");
            //if (!DBHelper.UpdateEmployee(employees) || !DBHelper.UpdateCalendar(calendars)) return;

            DateTime date = AppInfor.Date;
            //DateTime datetest = DateTime.Parse(AppInfor.datetest);

            // 1. Get data employees from db_sanze
            employees = DBHelper.Getemployee();

            // 2. Get data calendars  from db_sanze
            calendars = DBHelper.getCalender();

            //3. Download log.csv from server
            bool flag = bool.Parse(AppInfor.Flag);
            if (!flag)
            {
                DownloadCSV.Download(date);
                logs_today = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.Folder_name + @"\" + AppInfor.Getmonth_folder_log(date) + @"\" + AppInfor.GetfileInfor(date), employees);
            }
            else
            {
                logs_today = CSVHelper.GetLogs(AppInfor.Local_path_logs + AppInfor.GetfileInfor(date),employees);
            }

            //5.Validate data employees, calendars, logs
            if (!ValidateHelper.ValidateEmployees(employees) || !ValidateHelper.ValidateCalender(calendars) || !ValidateHelper.ValidateLogs(logs_today)) return;

            //6.Update logs into db_sanze
            if (!DBHelper.InsertLogstoDB(logs_today)) return;

            //7.Get logs from database
            logs_today = DBHelper.Getlogs(date);
            logs_yesterday = DBHelper.Getlogs(date.AddDays(-1));

            //8.Get list time sheet by day
            timesheets_yesterday = TimeSheetHelper.GetTimeSheetByDay(logs_yesterday, employees, date.AddDays(-1));
            timesheets_today = TimeSheetHelper.GetTimeSheetByDay(logs_today, employees, date);

            //9.Update database
    if (!DBHelper.UpdateTimeSheet_Yesterday(timesheets_yesterday) || !DBHelper.InsertTimeSheet_Today(timesheets_today)) return;

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

