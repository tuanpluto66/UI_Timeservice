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
            List<Log> log_next_day;
            List<Log> logs_result;
            List<DateTime> list_date_startd_toNow;
            List<DateTime> list_date_timesheet_status;
            List<DateTime> list_date_difference;
            List<DateTime> list_date_insert_timesheet;

            List<Timesheet> timesheets; // output use to update db
                                        //logs_today = CSVHelper.GetLogs("D:\\log-20221001.csv",employees);

            System.DateTime enddate = AppInfor.Date.Date;
            System.DateTime startdate = System.DateTime.Parse(AppInfor.Startdate).Date;

            // 1. Get data employees from db_sanze
            employees = DBHelper.Getemployee();

            // 2. Get data calendars  from db_sanze
            calendars = DBHelper.getCalender();

            // 8.Validate data employees, calendars
            if (!ValidateHelper.ValidateEmployees(employees) || !ValidateHelper.ValidateCalender(calendars)) return;

           
            // 3. Lấy ds ngày chưa được insert vào db_sanze => insert với status = 0
            // Lấy ngày bắt đầu startdate từ app.config
            //DateTime[] dates = DateArray.GetDatesBetween(startdate, enddate.AddDays(-2)).ToArray();
            list_date_startd_toNow = DateArray.GetDatesBetween(startdate, enddate.AddDays(-2));
            if (list_date_startd_toNow == null) return;

            list_date_timesheet_status = DBHelper.Getdate_database();
            //DateTime[] datedb = DBHelper.Getdate().ToArray();

            list_date_difference = list_date_startd_toNow.Get_list_date_difference(list_date_timesheet_status, a2 => a2, a1 => a1).ToList();
           

            // 4. Insert ds ngày không tồn tại trong timesheet_status từ ds ngày start_date into db_sanze
            if (!DBHelper.Inser_list_date(list_date_difference)) return;

            // 5. Lấy ds ngày cần thêm timesheet
            list_date_insert_timesheet = DBHelper.Getdate_insert_timesheet();

            // 6. For ds ngày cần thêm timesheet
            foreach (var dt in list_date_insert_timesheet) {
                // 7.Download log.csv from server
                int flag = Convert.ToInt32(AppInfor.Flag);
                if (flag == 0)
                {
                    if (DownloadCSV.Download(dt))
                    {
                        logs_today = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.Folder_name + @"\" + AppInfor.Get_year_folder_log(dt) + @"\" + AppInfor.Getmonth_folder_log(dt) + @"\" + AppInfor.GetfileInfor(dt), employees);
                        log_next_day = CSVHelper.GetLogs(AppDomain.CurrentDomain.BaseDirectory + AppInfor.Folder_name + @"\" + AppInfor.Get_year_folder_log(dt.AddDays(1)) + @"\" + AppInfor.Getmonth_folder_log(dt.AddDays(1)) + @"\" + AppInfor.GetfileInfor(dt.AddDays(1)), employees);
                    }
                    else
                    {
                        return;
                    }
                }
                else if (flag == 1)
                {
                    logs_today = CSVHelper.GetLogs(AppInfor.Local_path_logs + AppInfor.Get_year_folder_log(dt) + @"\" + AppInfor.Getmonth_folder_log(dt) + @"\" + AppInfor.GetfileInfor(dt), employees);
                    log_next_day = CSVHelper.GetLogs(AppInfor.Local_path_logs + AppInfor.Get_year_folder_log(dt.AddDays(1)) + @"\" + AppInfor.Getmonth_folder_log(dt.AddDays(1)) + @"\" + AppInfor.GetfileInfor(dt.AddDays(1)), employees);
                }
                else
                {
                    return;
                }

                if (logs_today == null || log_next_day == null) 
                { 
                    continue;
                }
                

                // 9.Insert logs into db_sanze
                if (!DBHelper.InsertLogstoDB(logs_today)||!DBHelper.InsertLogstoDB(log_next_day)) return;

                //7.Get logs from database
                //logs_today = DBHelper.Getlogs(date);
                //logs_yesterday = DBHelper.Getlogs(date.AddDays(-1));
                logs_result = DBHelper.Getlogs(dt);

                //8.Get list time sheet by day
                timesheets = TimeSheetHelper.GetTimeSheetByDay(logs_result, employees, dt);
                //timesheets_today = TimeSheetHelper.GetTimeSheetByDay(logs_today, employees, date);

                // 9. Delete timsheet from table timsheet (day_of_year, timesheet_status = 0)
                if (!DBHelper.Delete_timsheet(dt)) return;

                //10.Update database
                //if (!DBHelper.UpdateTimeSheet_Yesterday(timesheets_yesterday) || !DBHelper.InsertTimeSheet_Today(timesheets_today)) return;
                if (!DBHelper.InsertTimeSheet(timesheets)) return;

                // 11. Update status = 1, timesheet_status
                if (!DBHelper.Update_status_timesheet(dt)) return;
            }

            LogHelper.InfoFormat("ACTION COMPLETE");            // ACTION COMPLETE

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






        }
    }
}

