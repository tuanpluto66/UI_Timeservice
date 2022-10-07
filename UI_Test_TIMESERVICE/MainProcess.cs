using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //DateTime date = AppInfor.Date;
            DateTime datetest = DateTime.Parse(AppInfor.datetest);

            // 1. Get data employees from db_sanze
            employees = DBHelper.Getemployee();

            // 2. Get data calendars  from db_sanze
            calendars = DBHelper.getCalender();


            //3. Download log.csv from server
            //if (!DownloadCSV.Download(datetest)) return;

            // 4. Get data log  from CSV
            logs_today = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.GetfileInfor(datetest), employees);
            //logs_today = CSVHelper.GetLogs("D:\\Test_T9\\09\\log-20220901.csv", employees);

            // 5. Validate data employees,calendars,logs
            if (!ValidateHelper.ValidateEmployees(employees) || !ValidateHelper.ValidateCalender(calendars) || !ValidateHelper.ValidateLogs(logs_today)) return;

            //6. Update logs into db_sanze
            if (!DBHelper.InsertLogstoDB(logs_today)) return;

            //7. Get logs from database
            logs_today = DBHelper.Getlogs(datetest);
            logs_yesterday = DBHelper.Getlogs(datetest.AddDays(-1));

            // 8. Get list time sheet by day           
            timesheets_yesterday = TimeSheetHelper.GetTimeSheetByDay(logs_yesterday, employees, datetest.AddDays(-1));
            timesheets_today = TimeSheetHelper.GetTimeSheetByDay(logs_today, employees, datetest);

            // 9. Update database
            if (!DBHelper.UpdateTimeSheet_Yesterday(timesheets_yesterday) || !DBHelper.InsertTimeSheet_Today(timesheets_today)) return;
            //if (!DBHelper.InsertTimeSheet_Today(timesheets_today)) return;
            LogHelper.InfoFormat("ACTION COMPLETE");            // ACTION COMPLETE



        }
    }
}

