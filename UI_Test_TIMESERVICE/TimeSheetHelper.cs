using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI_Test_TIMESERVICE.DTO;

namespace UI_Test_TIMESERVICE
{
    public class TimeSheetHelper
    {
        static int count_log;
        //static int block = 1; 
        static int block = Convert.ToInt32(AppInfor.Blockforminute);  //call block from app.config, takes two values 1 and 30
        public static List<Log> GetLog(List<Log> listLog, int id, string date, string next_date)
        {
            DateTime this_day = Convert.ToDateTime(date);
            DateTime next_day = Convert.ToDateTime(next_date);
            TimeSpan day_point = TimeSpan.Parse("05:00:00");
            var log = listLog.Where(l => (l.Employee_id == id && l.Status == true &&
                                            l.Door_type == true &&
                                            (
                                                (Convert.ToDateTime(l.Day_of_year) == this_day &&
                                                    (TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) > 0 ||
                                                    TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) == 0)) ||
                                                (Convert.ToDateTime(l.Day_of_year) == next_day && TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) < 0)
                                            )
                                         )).OrderBy(l => (l.Day_of_year, l.Time_of_day)).ToList();
            //string query = "SELECT day_of_year,time_of_day,state,id FROM log WHERE employee_id = @Employee_id AND status = 1 AND door_type = 1 AND ((day_of_year = @Day_of_year AND (time_of_day > '05:00:00' OR time_of_day = '05:00:00')) OR (day_of_year = @Next_day_of_year AND time_of_day < '05:00:00')) ORDER BY day_of_year,time_of_day ASC";
            return log;
        }
        public static Log GetFirstLog(List<Log> listLog, int id, string date)
        {
            DateTime this_day = Convert.ToDateTime(date);
            TimeSpan day_point = TimeSpan.Parse("05:00:00");
            var log = listLog.Where(l => (l.Employee_id == id && l.Status == true &&
                                            l.Door_type == true &&
                                            (
                                                (Convert.ToDateTime(l.Day_of_year) == this_day &&
                                                    (TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) > 0 ||
                                                        TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) == 0)
                                                )
                                            )
                                         )).OrderBy(l => (l.Day_of_year, l.Time_of_day)).FirstOrDefault();
            //string query = "SELECT id,time_of_day FROM log WHERE employee_id = @Employee_id AND status = 1 AND state = 1 AND door_type = 1 AND (day_of_year = @Day_of_year AND ((time_of_day > '05:00:00' OR time_of_day = '05:00:00') AND (time_of_day < '23:59:59' OR time_of_day = '23:59:59'))) ORDER BY day_of_year,time_of_day ASC LIMIT 1";
            return log;
        }
        public static Log GetFirstInLog(List<Log> listLog, int id, string date)
        {
            DateTime this_day = Convert.ToDateTime(date);
            TimeSpan day_point = TimeSpan.Parse("05:00:00");
            var log = listLog.Where(l => (l.Employee_id == id && l.Status == true &&
                                            l.Door_type == true && l.State == true &&
                                            (
                                                (Convert.ToDateTime(l.Day_of_year) == this_day &&
                                                    (TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) > 0 ||
                                                        TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) == 0)
                                                )
                                            )
                                         )).OrderBy(l => (l.Day_of_year, l.Time_of_day)).FirstOrDefault();
            //string query = "SELECT id,time_of_day FROM log WHERE employee_id = @Employee_id AND status = 1 AND state = 1 AND door_type = 1 AND (day_of_year = @Day_of_year AND ((time_of_day > '05:00:00' OR time_of_day = '05:00:00') AND (time_of_day < '23:59:59' OR time_of_day = '23:59:59'))) ORDER BY day_of_year,time_of_day ASC LIMIT 1";
            return log;
        }
        public static State_of_work GetStateOfWork(TimeSpan time_in_real, TimeSpan start_time_in_rule, TimeSpan end_time_in_rule)
        {
            return TimeSpan.Compare(time_in_real, start_time_in_rule) < 0 ? ((TimeSpan.Compare(time_in_real, new TimeSpan(5, 0, 0)) < 0) ? State_of_work.after : State_of_work.before) :
                (TimeSpan.Compare(time_in_real, end_time_in_rule) > 0 ? State_of_work.after :
                (TimeSpan.Compare(time_in_real, new TimeSpan(12, 0, 0)) < 0 ? State_of_work.before_lunch_break :
                (TimeSpan.Compare(time_in_real, new TimeSpan(13, 0, 0)) > 0 ? State_of_work.after_lunch_break : State_of_work.lunch_break)));
        }
        public static Log GetLastOutLog(List<Log> listLog, int id, string start_time, string date, string next_date)
        {
            DateTime this_day = Convert.ToDateTime(date);
            DateTime next_day = Convert.ToDateTime(next_date);
            TimeSpan day_point = TimeSpan.Parse("05:00:00");
            //xóa mốc 3h
            //TimeSpan day_end = TimeSpan.Parse("03:00:00");
            var log = listLog.Where(l => (l.Employee_id == id && l.Status == true && l.Door_type == true && l.State == false &&
                                            (
                                                ((Convert.ToDateTime(l.Day_of_year) == this_day &&
                                                    TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) > 0)
                                                      ||
                                                 (Convert.ToDateTime(l.Day_of_year) == next_day &&
                                                      (TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) < 0 ||
                                                        TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) == 0)
                                                    //chuyển mốc 3h về 5h
                                                    //(TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_end) < 0 ||
                                                    //  TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_end) == 0)
                                                    )
                                                )
                                            )
                                         )).OrderByDescending(l => (l.Day_of_year, l.Time_of_day)).FirstOrDefault();
            //string query = "SELECT time_of_day FROM log WHERE employee_id = @Employee_id AND status = 1 AND state = 0 AND door_type = 1 AND ((day_of_year = @Day_of_year AND time_of_day > @Start_time) OR (day_of_year = @Next_day_of_year AND (time_of_day < '03:00:00' OR time_of_day = '03:00:00'))) ORDER BY day_of_year,time_of_day DESC LIMIT 1";
            return log;
        }
        public static Log GetLastLog(List<Log> listLog, int id, string date, string next_date)
        {
            DateTime this_day = Convert.ToDateTime(date);
            DateTime next_day = Convert.ToDateTime(next_date);
            TimeSpan day_point = TimeSpan.Parse("05:00:00");
            var log = listLog.Where(l => (l.Employee_id == id && l.Status == true &&
                                            l.Door_type == true &&
                                            (
                                                (Convert.ToDateTime(l.Day_of_year) == this_day &&
                                                    (TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) > 0 ||
                                                    TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) == 0)) ||
                                                (Convert.ToDateTime(l.Day_of_year) == next_day && TimeSpan.Compare(TimeSpan.Parse(l.Time_of_day), day_point) < 0)
                                            )
                                         )).OrderByDescending(l => (l.Day_of_year, l.Time_of_day)).FirstOrDefault();
            //string query = "SELECT id FROM log WHERE employee_id = @Employee_id AND status = 1 AND door_type = 1 AND ((day_of_year = @Day_of_year AND (time_of_day > '05:00:00' OR time_of_day = '05:00:00')) OR (day_of_year = @Next_day_of_year AND time_of_day < '03:00:00')) ORDER BY day_of_year,time_of_day DESC LIMIT 1";
            return log;
        }
        public static void GetGoOnBussinessTime(TimeSpan current_time, SaveRecord prev_record, TimeSpan start_break, TimeSpan end_break, TimeSpan start_day, ref Timesheet ts)
        {
            double dif_second = current_time.TotalSeconds - prev_record.Time.TotalSeconds;
            if (dif_second < 0)
            {
                dif_second += 86400;
            }
            TimeSpan dif_time = TimeSpan.FromSeconds(dif_second);
            if ((dif_second) >= (double)1800)
            {
                if (TimeSpan.Compare(prev_record.Time, start_break) <= 0 && TimeSpan.Compare(prev_record.Time, start_day) >= 0)
                {
                    if (TimeSpan.Compare(current_time, start_break) <= 0 && TimeSpan.Compare(current_time, start_day) >= 0)
                    {
                        ts.Go_on_business_time += dif_time;
                    }
                    else if ((TimeSpan.Compare(current_time, start_break) > 0 && TimeSpan.Compare(current_time, end_break) < 0))
                    {
                        if (start_break.Subtract(prev_record.Time).TotalSeconds >= (double)1800)
                        {
                            ts.Go_on_business_time += start_break.Subtract(prev_record.Time);
                        }
                    }
                    else
                    {
                        if (dif_time.Subtract(ts.Break_time_in_rule).TotalSeconds >= (double)1800)
                        {
                            ts.Go_on_business_time += dif_time.Subtract(ts.Break_time_in_rule);
                        }
                    }
                }
                else if (TimeSpan.Compare(prev_record.Time, start_break) > 0 && TimeSpan.Compare(prev_record.Time, end_break) < 0)
                {
                    if ((TimeSpan.Compare(current_time, start_break) > 0 && TimeSpan.Compare(current_time, end_break) < 0))
                    {
                        ts.Go_on_business_time += new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        if (current_time.Subtract(end_break).TotalSeconds >= (double)1800)
                        {
                            ts.Go_on_business_time += current_time.Subtract(end_break);
                        }
                    }
                }
                else
                {
                    ts.Go_on_business_time += dif_time;
                }

                if (TimeSpan.Compare(prev_record.Time, ts.End_time_in_rule) < 0 && TimeSpan.Compare(prev_record.Time, start_day) >= 0)
                {
                    if (TimeSpan.Compare(prev_record.Time, ts.Start_time_in_rule) < 0)
                    {
                        if (TimeSpan.Compare(current_time, start_day) >= 0)
                        {
                            if (TimeSpan.Compare(current_time, ts.Start_time_in_rule) < 0)
                            {
                                ts.Go_on_business_time_in_work_time += TimeSpan.Parse("0");
                            }
                            else if (TimeSpan.Compare(current_time, start_break) <= 0)
                            {
                                ts.Go_on_business_time_in_work_time += current_time.Subtract(ts.Start_time_in_rule);
                            }
                            else if (TimeSpan.Compare(current_time, end_break) < 0)
                            {
                                if (start_break.Subtract(prev_record.Time).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += start_break.Subtract(ts.Start_time_in_rule);
                                }
                            }
                            else if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += current_time.Subtract(ts.Start_time_in_rule).Subtract(ts.Break_time_in_rule);
                                }
                            }
                            else
                            {
                                if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(ts.Start_time_in_rule).Subtract(ts.Break_time_in_rule);
                                }
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (TimeSpan.Compare(ts.End_time_in_rule, start_day) < 0)
                                {
                                    if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0)).TotalSeconds >= (double)1800)
                                    {
                                        ts.Go_on_business_time_in_work_time += current_time.Subtract(ts.Start_time_in_rule).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0));
                                    }
                                }
                            }
                            else
                            {
                                if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0)).TotalSeconds >= (double)1800)
                                {
                                    if (ts.End_time_in_rule > ts.Start_time_in_rule)
                                    {
                                        ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(ts.Start_time_in_rule).Subtract(ts.Break_time_in_rule);
                                    }
                                    else
                                    {
                                        ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(ts.Start_time_in_rule).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0));
                                    }
                                }
                            }
                        }
                    }
                    else if (TimeSpan.Compare(prev_record.Time, start_break) <= 0)
                    {
                        if (TimeSpan.Compare(current_time, start_day) >= 0)
                        {
                            if (TimeSpan.Compare(current_time, start_break) <= 0)
                            {
                                ts.Go_on_business_time_in_work_time += dif_time;
                            }
                            else if (TimeSpan.Compare(current_time, end_break) < 0)
                            {
                                if (start_break.Subtract(prev_record.Time).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += start_break.Subtract(prev_record.Time);
                                }
                            }
                            else if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += dif_time.Subtract(ts.Break_time_in_rule);
                                }
                            }
                            else
                            {
                                if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule);
                                }
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (TimeSpan.Compare(ts.End_time_in_rule, start_day) < 0)
                                {
                                    if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0)).TotalSeconds >= (double)1800)
                                    {
                                        ts.Go_on_business_time_in_work_time += dif_time.Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0));
                                    }
                                }
                            }
                            else
                            {
                                if (current_time.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0)).TotalSeconds >= (double)1800)
                                {

                                    if (ts.End_time_in_rule > prev_record.Time)
                                    {
                                        ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule);
                                    }
                                    else
                                    {
                                        ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(prev_record.Time).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0));
                                    }
                                }
                            }
                        }
                    }
                    else if (TimeSpan.Compare(prev_record.Time, end_break) < 0)
                    {
                        if (TimeSpan.Compare(current_time, start_day) >= 0)
                        {
                            if (TimeSpan.Compare(current_time, end_break) < 0)
                            {
                                ts.Go_on_business_time_in_work_time += TimeSpan.Parse("0");
                            }
                            else if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (current_time.Subtract(end_break).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += current_time.Subtract(end_break);
                                }
                            }
                            else
                            {
                                if (current_time.Subtract(end_break).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(end_break);
                                }
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (TimeSpan.Compare(ts.End_time_in_rule, start_day) < 0)
                                {
                                    if (current_time.Subtract(end_break).Add(new TimeSpan(24, 0, 0)).TotalSeconds >= (double)1800)
                                    {
                                        ts.Go_on_business_time_in_work_time += current_time.Subtract(end_break).Add(new TimeSpan(24, 0, 0));
                                    }
                                }
                            }
                            else
                            {
                                if (current_time.Subtract(end_break).Subtract(ts.Break_time_in_rule).Add(new TimeSpan(24, 0, 0)).TotalSeconds >= (double)1800)
                                {
                                    ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(end_break).Add(new TimeSpan(24, 0, 0));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (TimeSpan.Compare(current_time, start_day) >= 0)
                        {
                            if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                ts.Go_on_business_time_in_work_time += dif_time;
                            }
                            else
                            {
                                ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(prev_record.Time);
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(current_time, ts.End_time_in_rule) <= 0)
                            {
                                if (TimeSpan.Compare(ts.End_time_in_rule, start_day) < 0)
                                {
                                    ts.Go_on_business_time_in_work_time += dif_time.Add(new TimeSpan(24, 0, 0));
                                }
                            }
                            else
                            {
                                ts.Go_on_business_time_in_work_time += ts.End_time_in_rule.Subtract(prev_record.Time).Add(new TimeSpan(24, 0, 0));
                            }
                        }
                    }
                }
            }
        }
        public static TimeSpan GetTimeByBlock(TimeSpan time)
        {
            return new TimeSpan(0, (((int)((int)time.TotalMinutes / block)) * block), 0);
        }
        public static int GetTypeOfDay(DateTime date)
        {

            string query = "SELECT type FROM calendar WHERE day_of_year = @Day_of_year";
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            sqlParameters.Add(new MySqlParameter("@Day_of_year", date.ToString("yyyy-MM-dd")));
            var scalarResult = DBHelper.ExecuteScalar(query, sqlParameters);
            if (scalarResult == null)
            {
                return -1;
            }
            int type_of_day = Convert.ToInt32(scalarResult);
            return type_of_day;
        }
        public static List<Timesheet> GetTimeSheetByDay(List<Log> listLogOfDay, List<Employee> employees, DateTime date)
        {
            var employee_list = employees;
            List<Timesheet> timesheets = new List<Timesheet>();
            try
            {
                TimeSpan start_break = new TimeSpan(12, 0, 0);
                TimeSpan end_break = new TimeSpan(13, 0, 0);
                
                SaveRecord flag_record;
                SaveRecord prev_record;
                int index;
                bool check_over_day = false;
                TimeSpan last_time = TimeSpan.Parse("0");
                for (index = 0; index < employee_list.Count; index++)
                {
                    DateTime next_date = date.AddDays(1);
                    var listLog = GetLog(listLogOfDay, employee_list[index].Id, date.ToString("yyyy-MM-dd"), next_date.ToString("yyyy-MM-dd"));
                    Timesheet ts = new Timesheet();
                    ts.Ic_card = employee_list[index].Ic_card;
                    ts.Employee_code = employee_list[index].Employee_code;
                    ts.Name = employee_list[index].Name;
                    ts.Employee_id = employee_list[index].Id;
                    ts.Start_time_in_rule = TimeSpan.Parse(employee_list[index].Start_work_time);
                    ts.End_time_in_rule = TimeSpan.Parse(employee_list[index].End_work_time);
                    ts.Break_time_in_rule = TimeSpan.Parse("1:00:00");
                    ts.Go_on_business_time = TimeSpan.Parse("0");
                    ts.Go_on_business_time_in_work_time = TimeSpan.Parse("0");
                    ts.Date = date.ToString("yyyy-MM-dd");
                    ts.Time_before_work = TimeSpan.Parse("0");
                    ts.Time_in_work = TimeSpan.Parse("0");
                    ts.Time_after_work = TimeSpan.Parse("0");
                    ts.Work = false;
                    bool ready_to_handle = false;
                    int ic_error = 0;
                    flag_record = new SaveRecord();
                    prev_record = new SaveRecord();
                    int type_of_day = GetTypeOfDay(date);
                    if (type_of_day == -1)
                    {
                        ts.Time_before_work_by_block = new TimeSpan(0, 0, 0);
                        ts.Time_after_work_by_block = new TimeSpan(0, 0, 0);
                        ts.Ic_error = 0;
                        timesheets.Add(ts);
                        continue;
                    }
                    foreach (var l in listLog)
                    {
                        ts.Department = l.Department;
                    }
                    //ts.Department = employee_list[index].Department;
                    switch (type_of_day)
                    {
                        case 0:
                            ts.Type_of_day = "平日";
                            break;

                        case 1:
                            ts.Type_of_day = "休日";
                            break;

                        case 2:
                            ts.Type_of_day = "休日";
                            break;

                        case 3:
                            ts.Type_of_day = "休日";
                            break;
                    }
                    if (listLog.Count > 0)
                    {
                        var start_record = GetFirstInLog(listLog, employee_list[index].Id, date.ToString("yyyy-MM-dd"));
                        TimeSpan start_day = new TimeSpan(5, 0, 0);
                        bool start_at_5_o = false;
                        Log first_log = GetFirstLog(listLog, employee_list[index].Id, date.ToString("yyyy-MM-dd"));
                        if (first_log != null)
                        {
                            if (employee_list[index].Last_day == date.AddDays(-1))
                            {
                                if (employee_list[index].Last_day_status)
                                {
                                    if (first_log.State)
                                    {
                                        if ((TimeSpan.Parse(first_log.Time_of_day).TotalSeconds - employee_list[index].Last_day_time.TotalSeconds) > (double)60 || ((TimeSpan.Parse(first_log.Time_of_day).TotalSeconds - employee_list[index].Last_day_time.TotalSeconds) <= (double)0) && (TimeSpan.Parse(first_log.Time_of_day).TotalSeconds + (double)86400 - employee_list[index].Last_day_time.TotalSeconds) > (double)60)
                                        {
                                            ic_error++;
                                        }
                                    }
                                    else
                                    {
                                        start_at_5_o = true;
                                    }
                                }
                                else
                                {
                                    if (!first_log.State)
                                    {
                                        if ((TimeSpan.Parse(first_log.Time_of_day).TotalSeconds - employee_list[index].Last_day_time.TotalSeconds) > (double)60 || ((TimeSpan.Parse(first_log.Time_of_day).TotalSeconds - employee_list[index].Last_day_time.TotalSeconds) <= (double)0) && (TimeSpan.Parse(first_log.Time_of_day).TotalSeconds + (double)86400 - employee_list[index].Last_day_time.TotalSeconds) > (double)60)
                                        {
                                            ic_error++;
                                        }
                                    }
                                }
                            }
                        }

                        if (employee_list[index].Last_day_status && employee_list[index].Last_day == date.AddDays(-1))
                        {
                            if (first_log != null)
                            {
                                if (!first_log.State)
                                {
                                    start_at_5_o = true;
                                }
                            }
                        }
                        if (start_record != null || start_at_5_o)
                        {
                            int start_record_id = 0;
                            string start_time = "";
                            Log fake_log = new Log();
                            if (start_at_5_o)
                            {
                                fake_log.ID = count_log;
                                count_log++;
                                fake_log.Time_of_day = (new TimeSpan(5, 0, 0)).ToString();
                                listLog.Insert(0, fake_log);

                                start_record_id = fake_log.ID;
                                start_time = fake_log.Time_of_day;
                            }
                            else
                            {
                                start_record_id = Convert.ToInt32(start_record.ID);
                                start_time = start_record.Time_of_day;
                            }
                            ts.Start_time_in_real = TimeSpan.Parse(start_time);
                            flag_record.Time = ts.Start_time_in_real;
                            flag_record.State = true;
                            flag_record.State_of_work = GetStateOfWork(ts.Start_time_in_real, ts.Start_time_in_rule, ts.End_time_in_rule);
                            flag_record.Time = ts.Start_time_in_real;
                            prev_record.Time = ts.Start_time_in_real;
                            prev_record.State = true;
                            prev_record.State_of_work = GetStateOfWork(ts.Start_time_in_real, ts.Start_time_in_rule, ts.End_time_in_rule);
                            var end_record = GetLastOutLog(listLog, employee_list[index].Id, start_time, date.ToString("yyyy-MM-dd"), next_date.ToString("yyyy-MM-dd"));
                            if (end_record != null)
                            {
                                string end_time = end_record.Time_of_day.ToString();
                                ts.End_time_in_real = TimeSpan.Parse(end_time);
                            }
                            var lastRecord = GetLastLog(listLog, employee_list[index].Id, date.ToString("yyyy-MM-dd"), next_date.ToString("yyyy-MM-dd"));
                            if (lastRecord == null)
                            {
                                ts.Time_before_work_by_block = new TimeSpan(0, 0, 0);
                                ts.Time_after_work_by_block = new TimeSpan(0, 0, 0);
                                ts.Ic_error = 0;
                            }
                            else
                            {
                                last_time = TimeSpan.Parse(lastRecord.Time_of_day.ToString());
                                int lastRecordId = lastRecord.ID;
                                int i;
                                for (i = 0; i < listLog.Count; i++)
                                {
                                    if (Convert.ToInt32(listLog[i].ID) == start_record_id)
                                    {
                                        ready_to_handle = true;
                                    }
                                    else
                                    {
                                        if (ready_to_handle)
                                        {
                                            TimeSpan current_time = TimeSpan.Parse(listLog[i].Time_of_day.ToString());
                                            bool current_state = Convert.ToBoolean(listLog[i].State);
                                            var current_state_of_work = GetStateOfWork(current_time, ts.Start_time_in_rule, ts.End_time_in_rule);
                                            /// xét trạng thái
                                            /// ///cùng trạng thái
                                            if (current_state == prev_record.State)
                                            {
                                                if (!current_state)
                                                {
                                                    GetGoOnBussinessTime(current_time, prev_record, start_break, end_break, start_day, ref ts);
                                                }
                                                if ((current_time.TotalSeconds - prev_record.Time.TotalSeconds) > (double)60 || ((current_time.TotalSeconds - prev_record.Time.TotalSeconds) <= (double)0) && (current_time.TotalSeconds + (double)86400 - prev_record.Time.TotalSeconds) > (double)60)
                                                {
                                                    ic_error++;
                                                }
                                                prev_record.Time = current_time;
                                                prev_record.State = current_state;
                                                prev_record.State_of_work = GetStateOfWork(current_time, ts.Start_time_in_rule, ts.End_time_in_rule);
                                            }
                                            /// ///khác trạng thái
                                            else
                                            {
                                                if (!current_state)
                                                {
                                                    switch (prev_record.State_of_work)
                                                    {
                                                        case State_of_work.before:
                                                            switch (current_state_of_work)
                                                            {
                                                                case State_of_work.before:
                                                                    ts.Time_before_work += (current_time - prev_record.Time);
                                                                    break;

                                                                case State_of_work.before_lunch_break:
                                                                    ts.Time_before_work += (ts.Start_time_in_rule - prev_record.Time);
                                                                    ts.Time_in_work += (current_time - ts.Start_time_in_rule);
                                                                    break;

                                                                case State_of_work.lunch_break:
                                                                    ts.Time_before_work += (ts.Start_time_in_rule - prev_record.Time);
                                                                    ts.Time_in_work += (new TimeSpan(12, 0, 0) - ts.Start_time_in_rule);
                                                                    break;

                                                                case State_of_work.after_lunch_break:
                                                                    ts.Time_before_work += (ts.Start_time_in_rule - prev_record.Time);
                                                                    ts.Time_in_work += (new TimeSpan(12, 0, 0) - ts.Start_time_in_rule);
                                                                    ts.Time_in_work += (current_time - new TimeSpan(13, 0, 0));
                                                                    break;

                                                                case State_of_work.after:
                                                                    ts.Time_before_work += (ts.Start_time_in_rule - prev_record.Time);
                                                                    ts.Time_in_work += (new TimeSpan(12, 0, 0) - ts.Start_time_in_rule);
                                                                    ts.Time_in_work += (ts.End_time_in_rule - new TimeSpan(13, 0, 0));
                                                                    if (TimeSpan.Compare(current_time, ts.End_time_in_rule) >= 0)
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule);
                                                                    }
                                                                    else
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule + new TimeSpan(24, 0, 0));
                                                                    }
                                                                    break;
                                                            }
                                                            break;

                                                        case State_of_work.before_lunch_break:
                                                            switch (current_state_of_work)
                                                            {
                                                                case State_of_work.before_lunch_break:
                                                                    ts.Time_in_work += (current_time - prev_record.Time);
                                                                    break;

                                                                case State_of_work.lunch_break:
                                                                    ts.Time_in_work += (new TimeSpan(12, 0, 0) - prev_record.Time);
                                                                    break;

                                                                case State_of_work.after_lunch_break:
                                                                    ts.Time_in_work += (new TimeSpan(12, 0, 0) - prev_record.Time);
                                                                    ts.Time_in_work += (current_time - new TimeSpan(13, 0, 0));
                                                                    break;

                                                                case State_of_work.after:
                                                                    ts.Time_in_work += (new TimeSpan(12, 0, 0) - prev_record.Time);
                                                                    ts.Time_in_work += (ts.End_time_in_rule - new TimeSpan(13, 0, 0));
                                                                    if (TimeSpan.Compare(current_time, ts.End_time_in_rule) >= 0)
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule);
                                                                    }
                                                                    else
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule + new TimeSpan(24, 0, 0));
                                                                    }
                                                                    break;
                                                            }
                                                            break;

                                                        case State_of_work.lunch_break:
                                                            switch (current_state_of_work)
                                                            {
                                                                case State_of_work.lunch_break:
                                                                    break;

                                                                case State_of_work.after_lunch_break:
                                                                    ts.Time_in_work += (current_time - new TimeSpan(13, 0, 0));
                                                                    break;

                                                                case State_of_work.after:
                                                                    ts.Time_in_work += (ts.End_time_in_rule - new TimeSpan(13, 0, 0));
                                                                    if (TimeSpan.Compare(current_time, ts.End_time_in_rule) >= 0)
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule);
                                                                    }
                                                                    else
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule + new TimeSpan(24, 0, 0));
                                                                    }
                                                                    break;
                                                            }
                                                            break;

                                                        case State_of_work.after_lunch_break:
                                                            switch (current_state_of_work)
                                                            {
                                                                case State_of_work.after_lunch_break:
                                                                    ts.Time_in_work += (current_time - prev_record.Time);
                                                                    break;

                                                                case State_of_work.after:
                                                                    ts.Time_in_work += (ts.End_time_in_rule - prev_record.Time);
                                                                    if (TimeSpan.Compare(current_time, ts.End_time_in_rule) >= 0)
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule);
                                                                    }
                                                                    else
                                                                    {
                                                                        ts.Time_after_work += (current_time - ts.End_time_in_rule + new TimeSpan(24, 0, 0));
                                                                    }
                                                                    break;
                                                            }
                                                            break;

                                                        case State_of_work.after:
                                                            if (TimeSpan.Compare(current_time, prev_record.Time) >= 0)
                                                            {
                                                                ts.Time_after_work += (current_time - prev_record.Time);
                                                            }
                                                            else
                                                            {
                                                                ts.Time_after_work += (current_time - prev_record.Time + new TimeSpan(24, 0, 0));
                                                            }
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    GetGoOnBussinessTime(current_time, prev_record, start_break, end_break, start_day, ref ts);
                                                }
                                                flag_record.Time = current_time;
                                                flag_record.State = current_state;
                                                flag_record.State_of_work = current_state_of_work;
                                                prev_record.Time = current_time;
                                                prev_record.State = current_state;
                                                prev_record.State_of_work = current_state_of_work;
                                            }
                                        }
                                    }
                                }



                                check_over_day = false;
                                if (lastRecord.State)
                                {
                                    ts.End_time_in_real = new TimeSpan(5, 0, 0);
                                    check_over_day = true;
                                    var last_log_time_state = GetStateOfWork(TimeSpan.Parse(lastRecord.Time_of_day.ToString()), ts.Start_time_in_rule, ts.End_time_in_rule);
                                    switch (last_log_time_state)
                                    {
                                        case State_of_work.before:
                                            ts.Time_before_work += (ts.Start_time_in_rule - last_time);
                                            ts.Time_in_work += (new TimeSpan(12, 0, 0) - ts.Start_time_in_rule);
                                            ts.Time_in_work += (ts.End_time_in_rule - new TimeSpan(13, 0, 0));
                                            ts.Time_after_work += (new TimeSpan(24, 0, 0) - ts.End_time_in_rule);
                                            ts.Time_after_work += (start_day - new TimeSpan(0, 0, 0));
                                            break;

                                        case State_of_work.before_lunch_break:
                                            ts.Time_in_work += (new TimeSpan(12, 0, 0) - last_time);
                                            ts.Time_in_work += (ts.End_time_in_rule - new TimeSpan(13, 0, 0));
                                            ts.Time_after_work += (new TimeSpan(24, 0, 0) - ts.End_time_in_rule);
                                            ts.Time_after_work += (start_day - new TimeSpan(0, 0, 0));
                                            break;

                                        case State_of_work.lunch_break:
                                            ts.Time_in_work += (ts.End_time_in_rule - new TimeSpan(13, 0, 0));
                                            ts.Time_after_work += (new TimeSpan(24, 0, 0) - ts.End_time_in_rule);
                                            ts.Time_after_work += (start_day - new TimeSpan(0, 0, 0));
                                            break;

                                        case State_of_work.after_lunch_break:
                                            ts.Time_in_work += (ts.End_time_in_rule - last_time);
                                            ts.Time_after_work += (new TimeSpan(24, 0, 0) - ts.End_time_in_rule);
                                            ts.Time_after_work += (start_day - new TimeSpan(0, 0, 0));
                                            break;

                                        case State_of_work.after:
                                            if (TimeSpan.Compare(last_time, ts.End_time_in_rule) < 0)
                                            {
                                                ts.Time_after_work += (start_day - last_time);
                                            }
                                            else
                                            {
                                                ts.Time_after_work += (new TimeSpan(24, 0, 0) - last_time);
                                                ts.Time_after_work += (start_day - new TimeSpan(0, 0, 0));
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    ts.End_time_in_real = TimeSpan.Parse(lastRecord.Time_of_day.ToString());
                                }

                                ts.Time_before_work_by_block = GetTimeByBlock(ts.Time_before_work);
                                ts.Time_after_work_by_block = GetTimeByBlock(ts.Time_after_work);
                                ts.Ic_error = ic_error;
                                ts.Work = true;
                            }


                        }
                        else
                        {
                            var lastRecord = GetLastLog(listLog, employee_list[index].Id, date.ToString("yyyy-MM-dd"), next_date.ToString("yyyy-MM-dd"));
                            if (lastRecord != null)
                            {
                                if (lastRecord.State)
                                {
                                    check_over_day = true;
                                }
                                last_time = TimeSpan.Parse(lastRecord.Time_of_day);
                                for (int j = listLog.Count - 1; j >= 0; j--)
                                {
                                    if (listLog[j].ID == lastRecord.ID)
                                    {
                                        continue;
                                    }
                                    if (listLog[j].State == lastRecord.State)
                                    {
                                        last_time = TimeSpan.Parse(listLog[j].Time_of_day);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            employee_list[index].Last_day_status = check_over_day;
                            employee_list[index].Last_day = date;
                            employee_list[index].Last_day_time = last_time;

                            ts.Time_before_work_by_block = new TimeSpan(0, 0, 0);
                            ts.Time_after_work_by_block = new TimeSpan(0, 0, 0);
                            ts.Ic_error = 0;
                            continue;
                        }
                    }
                    else
                    {
                        ts.Time_before_work_by_block = new TimeSpan(0, 0, 0);
                        ts.Time_after_work_by_block = new TimeSpan(0, 0, 0);
                        ts.Ic_error = 0;
                    }
                    if (ts.Work == true)
                    {
                        timesheets.Add(ts);
                    }
                    employee_list[index].Last_day_status = check_over_day;
                    employee_list[index].Last_day = date;
                    employee_list[index].Last_day_time = last_time;
                }

            }

            catch (InvalidOperationException ioe)
            {
                LogHelper.Error(ioe.StackTrace);
                throw ioe;
            }
            return timesheets;
        }

        

    }
}
