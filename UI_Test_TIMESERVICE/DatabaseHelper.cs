using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using UI_Test_TIMESERVICE.DTO;
using UI_Test_TIMESERVICE;
using static log4net.Appender.RollingFileAppender;

namespace TimeService
{
    public class DatabaseHelper
    {
        public static MySqlConnection mySqlConnection;
        string _connect;
        private static DatabaseHelper _instance;
        public DatabaseHelper(string connectionstring)
        {
            this._connect = connectionstring;
            mySqlConnection = new MySqlConnection(connectionstring);
            Connect();
        }
        public static DatabaseHelper getInstance(string connectionstring)
        {
            if (_instance == null)
            {
                _instance = new DatabaseHelper(connectionstring);
                mySqlConnection = new MySqlConnection(connectionstring);
                //_instance.Connect();
            }
            return _instance;
        }
        public MySqlConnection GetMysqlConnection()
        {
            return mySqlConnection;
        }
        private void Connect()
        {
            if(mySqlConnection != null && mySqlConnection.State != ConnectionState.Open)
            {
                mySqlConnection.Open();
            }
        }

        public void Dispose()
        {
            mySqlConnection.Close();
            mySqlConnection.Dispose();
        }
        public static void ExecuteNonQuery(string sql, List<MySqlParameter> lstparam)
        {
            try
            {
                using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        cmd.Parameters.AddRange(lstparam.ToArray());
                        cmd.ExecuteNonQuery();
                    }
                    //c.Close();
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                LogHelper.Error("Error query" + sql);
                LogHelper.Error(ex.StackTrace);
                throw ex;
            }
        }
        //public static void ExecuteNonQuery(string sql, List<MySqlParameter> lstparam)
        //{
        //    try
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
        //        {
        //            cmd.Parameters.AddRange(lstparam.ToArray());
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        LogHelper.Error(ex.Message);
        //        LogHelper.Error("Error query" + sql);
        //        LogHelper.Error(ex.StackTrace);
        //        throw ex;
        //    }
        //}
        //public static object ExecuteScalar(string sql, List<MySqlParameter> lstparam)
        //{
        //    try
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection))
        //        {
        //            cmd.Parameters.AddRange(lstparam.ToArray());
        //            return cmd.ExecuteScalar();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("Error query" + sql);
        //        LogHelper.Error(ex.Message);
        //        LogHelper.Error(ex.StackTrace);
        //        throw ex;
        //    }
        //}
        public static object ExecuteScalar(string sql, List<MySqlParameter> lstparam)
        {

            using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
            {
                try
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        cmd.Parameters.AddRange(lstparam.ToArray());
                        return cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error query" + sql);
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    throw ex;
                }
            }
        }
        public static DataTable ExecuteReader(string sql, List<MySqlParameter> lstparam = null)
        {
            try
            {
                using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        if (lstparam != null && lstparam.Count > 0)
                        {
                            cmd.Parameters.AddRange(lstparam.ToArray());
                        }
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        var dataTable = new DataTable();
                        try
                        {
                            dataTable.Load(rdr);
                        }
                        catch (ConstraintException cr)
                        {
                            LogHelper.Error(cr.StackTrace);
                        }
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Error query" + sql);
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// UpdateEmployee
        /// </summary>
        /// <returns></returns>
        public static bool UpdateEmployee(List<UI_Test_TIMESERVICE.DTO.Employee> employees)
        {
            string query = "";
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            foreach (var ep in employees)
            {
                query = "SELECT COUNT(employee_Id) FROM employee WHERE ic_card = @Ic_card";
                sqlParameters.Add(new MySqlParameter("@Ic_card", ep.Ic_card));
                int check_dupblicate = Convert.ToInt32(ExecuteScalar(query, sqlParameters));

                if (check_dupblicate > 0)
                {
                    query = "UPDATE employee SET employee_Code = @Employee_code WHERE ic_card = @Ic_card";
                }
                else
                {
                    query = "INSERT INTO employee (employee_Code,employee_Name,ic_card,start_work_time,end_work_time) values(@Employee_code, @Name, @Ic_card, @Start_work_time, @End_work_time)";
                }
                sqlParameters.Add(new MySqlParameter("@Employee_code", ep.Employee_code));
                sqlParameters.Add(new MySqlParameter("@Name", ep.Name));
                sqlParameters.Add(new MySqlParameter("@Start_work_time", ep.Start_work_time));
                sqlParameters.Add(new MySqlParameter("@End_work_time", ep.End_work_time));
                try
                {
                    ExecuteNonQuery(query, sqlParameters);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    throw ex;
                }
                sqlParameters.Clear();
            }
            return true;
        }
        /// <summary>
        /// UpdateEmployee
        /// </summary>
        /// <returns></returns>
        //public static bool UpdateCalendar(List<DTO.Calendar> calendars)
        //{
        //    List<MySqlParameter> sqlParameters;
        //    string query = "";
        //    foreach (var cld in calendars)
        //    {
        //        sqlParameters = new List<MySqlParameter>();

        //        query = "SELECT COUNT(id) FROM holiday WHERE day_of_year = @Day_of_year";
        //        sqlParameters.Add(new MySqlParameter("@Day_of_year", cld.Day_of_year));
        //        sqlParameters.Add(new MySqlParameter("@Type", cld.Type_of_day));

        //        int check_dupblicate = Convert.ToInt32(ExecuteScalar(query, sqlParameters));
        //        if (check_dupblicate > 0)
        //        {
        //            query = "UPDATE holiday SET type = @Type WHERE day_of_year = @Day_of_year";
        //        }
        //        else
        //        {
        //            query = "INSERT INTO holiday (day_of_year,type) values(@Day_of_year, @Type)";
        //        }
        //        try
        //        {
        //            ExecuteNonQuery(query, sqlParameters);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Error(ex.Message); LogHelper.Error(ex.StackTrace);
        //            throw ex;

        //        }
        //    }

        //    return true;
        //}
        /// <summary>
        /// Update Time Sheet
        /// </summary>
        /// <returns></returns>
        //public static bool InsertLogstoDB(List<Log> logs)
        //{
        //    bool isSuccess = true;
        //    List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
        //    string query = "";
        //    string currentDayWriteLog = null, beforeDayWriteLog = null;
        //    foreach (var lg in logs)
        //    {
        //        currentDayWriteLog = lg.Day_of_year;
        //        query = "SELECT COUNT(id) FROM log_time WHERE ic_card = @ic_card AND time_of_day = @time_of_day AND day_of_year = @day_of_year AND state = @state AND door_type = @door_type";
        //        sqlParameters.Add(new MySqlParameter("@time_of_day", lg.Time_of_day));
        //        sqlParameters.Add(new MySqlParameter("@ic_card", lg.Ic_card));
        //        sqlParameters.Add(new MySqlParameter("@day_of_year", lg.Day_of_year));
        //        sqlParameters.Add(new MySqlParameter("@state", lg.State));
        //        sqlParameters.Add(new MySqlParameter("@door_type", lg.Door_type));
        //        int check_dupblicate = Convert.ToInt32(ExecuteScalar(query, sqlParameters));
        //        if (check_dupblicate > 0)
        //        {
        //            query = "UPDATE log_time SET employee_Code = @employee_Code WHERE time_of_day = @time_of_day AND ic_card = @ic_card AND day_of_year = @day_of_year";
        //        }
        //        else
        //        {
        //            query = "INSERT INTO log_time (day_of_year," +
        //                "time_of_day," +
        //                "employee_Id," +
        //                "ic_card," +
        //                "employee_Code," +
        //                "employee_Name," +
        //                "department," +
        //                "door_name," +
        //                "door_type," +
        //                "state," +
        //                "status)" +
        //                "values(@day_of_year,@time_of_day,@employee_Id,@ic_card,@employee_Code,@employee_Name,@department,@door_name,@door_type,@state,@status)";
        //        }
        //        sqlParameters.Add(new MySqlParameter("@employee_Id", lg.Employee_id));
        //        sqlParameters.Add(new MySqlParameter("@employee_Code", lg.Employee_code));
        //        sqlParameters.Add(new MySqlParameter("@employee_Name", lg.Name));
        //        sqlParameters.Add(new MySqlParameter("@department", lg.Department));
        //        sqlParameters.Add(new MySqlParameter("@door_name", lg.Door_name));
        //        sqlParameters.Add(new MySqlParameter("@status", lg.Status));

        //        try
        //        {
        //            if (currentDayWriteLog != null && currentDayWriteLog != beforeDayWriteLog)
        //            {
        //                LogHelper.Info(string.Format("INSERT LOG DAY : {0}",currentDayWriteLog));
        //            }
        //            ExecuteNonQuery(query, sqlParameters);
        //            sqlParameters.Clear();
        //            beforeDayWriteLog = lg.Day_of_year;
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Error(ex.Message);
        //            LogHelper.Error(ex.StackTrace);
        //            isSuccess = false;
        //            sqlParameters.Clear();
        //            //break;
        //            return isSuccess;
        //        }

        //    }
        //    return isSuccess;
        //}

        public static bool InsertLogstoDB(List<Log> logs)
        {
            bool isSuccess = true;
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            string query = "";
            string currentDayWriteLog = null, beforeDayWriteLog = null;
            try
            {
                foreach (var lg in logs)
                {
                    currentDayWriteLog = lg.Day_of_year;
                    query = "SELECT COUNT(id) FROM log_time WHERE ic_card = @ic_card AND time_of_day = @time_of_day AND day_of_year = @day_of_year AND state = @state AND door_type = @door_type";
                    sqlParameters.Add(new MySqlParameter("@time_of_day", lg.Time_of_day));
                    sqlParameters.Add(new MySqlParameter("@ic_card", lg.Ic_card));
                    sqlParameters.Add(new MySqlParameter("@day_of_year", lg.Day_of_year));
                    sqlParameters.Add(new MySqlParameter("@state", lg.State));
                    sqlParameters.Add(new MySqlParameter("@door_type", lg.Door_type));
                    int check_dupblicate = Convert.ToInt32(ExecuteScalar(query, sqlParameters));
                    if (check_dupblicate > 0)
                    {
                        query = "UPDATE log_time SET employee_Code = @employee_Code WHERE time_of_day = @time_of_day AND ic_card = @ic_card AND day_of_year = @day_of_year";
                    }
                    else
                    {
                        query = "INSERT INTO log_time (day_of_year," +
                            "time_of_day," +
                            "employee_Id," +
                            "ic_card," +
                            "employee_Code," +
                            "employee_Name," +
                            "department," +
                            "door_name," +
                            "door_type," +
                            "state," +
                            "status)" +
                            "values(@day_of_year,@time_of_day,@employee_Id,@ic_card,@employee_Code,@employee_Name,@department,@door_name,@door_type,@state,@status)";
                    }
                    sqlParameters.Add(new MySqlParameter("@employee_Id", lg.Employee_id));
                    sqlParameters.Add(new MySqlParameter("@employee_Code", lg.Employee_code));
                    sqlParameters.Add(new MySqlParameter("@employee_Name", lg.Name));
                    sqlParameters.Add(new MySqlParameter("@department", lg.Department));
                    sqlParameters.Add(new MySqlParameter("@door_name", lg.Door_name));
                    sqlParameters.Add(new MySqlParameter("@status", lg.Status));

                    try
                    {
                        if (currentDayWriteLog != null && currentDayWriteLog != beforeDayWriteLog)
                        {
                            LogHelper.Info(string.Format("INSERT LOG DAY : {0}", currentDayWriteLog));
                        }
                        ExecuteNonQuery(query, sqlParameters);
                        sqlParameters.Clear();
                        beforeDayWriteLog = lg.Day_of_year;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex.Message);
                        LogHelper.Error(ex.StackTrace);
                        isSuccess = false;
                        sqlParameters.Clear();
                        return isSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                isSuccess = false;
                sqlParameters.Clear();
                return isSuccess;
            }
            return isSuccess;
        }
        public static bool InsertTimeSheet(List<UI_Test_TIMESERVICE.DTO.Timesheet> timesheets)
        {
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            string query = "";
            try
            {
                foreach (var ts in timesheets)
                {
                    query = "SELECT COUNT(id) FROM timesheetbyday WHERE ic_card = @ic_card AND day_of_year = @day_of_year";
                    sqlParameters.Add(new MySqlParameter("@ic_card", ts.Ic_card));
                    sqlParameters.Add(new MySqlParameter("@day_of_year", ts.Date));
                    int check_dupblicate = Convert.ToInt32(ExecuteScalar(query, sqlParameters));

                    if (check_dupblicate > 0)
                    {
                        query = "UPDATE timesheetbyday SET employee_Code = @Employee_code WHERE ic_card = @ic_card AND day_of_year = @day_of_year";
                    }
                    else
                    {
                        query = "INSERT INTO timesheetbyday (day_of_year," +
                            "department," +
                            "type_of_day," +
                            "ic_card," +
                            "employee_Code," +
                            "employee_Name," +
                            "start_time_in_rule," +
                            "break_time_in_rule," +
                            "end_time_in_rule," +
                            "total_time_rule," +
                            "start_time_in_real," +
                            "end_time_in_real," +
                            "total_time_in_real," +
                            "total_time_in_rule_2," +
                            "time_before_work," +
                            "time_in_work," +
                            "time_after_work," +
                            "time_before_work_by_block," +
                            "time_after_work_by_block," +
                            "occupancy_rate_on_time," +
                            "ic_error) " +
                            "values(@day_of_year, @department, @type_of_day,@ic_card,@employee_Code,@employee_Name,@start_time_in_rule," +
                            "@break_time_in_rule,@end_time_in_rule,@total_time_rule,@start_time_in_real,@end_time_in_real," +
                            "@total_time_in_real,@total_time_in_rule_2,@time_before_work,@time_in_work,@time_after_work," +
                            "@time_before_work_by_block,@time_after_work_by_block,@occupancy_rate_on_time,@ic_error)";
                    }

                    sqlParameters.Add(new MySqlParameter("@department", ts.Department));
                    sqlParameters.Add(new MySqlParameter("@type_of_day", ts.Type_of_day));
                    sqlParameters.Add(new MySqlParameter("@employee_Code", ts.Employee_code));
                    sqlParameters.Add(new MySqlParameter("@employee_Name", ts.Name));
                    sqlParameters.Add(new MySqlParameter("@start_time_in_rule", ts.Start_time_in_rule));
                    sqlParameters.Add(new MySqlParameter("@break_time_in_rule", ts.Break_time_in_rule));
                    sqlParameters.Add(new MySqlParameter("@end_time_in_rule", ts.End_time_in_rule));
                    sqlParameters.Add(new MySqlParameter("@total_time_rule", ts.Total_time_in_rule));
                    sqlParameters.Add(new MySqlParameter("@start_time_in_real", ts.Start_time_in_real));
                    sqlParameters.Add(new MySqlParameter("@end_time_in_real", ts.End_time_in_real));
                    sqlParameters.Add(new MySqlParameter("@total_time_in_real", ts.Total_time_in_real));
                    sqlParameters.Add(new MySqlParameter("@total_time_in_rule_2", ts.Total_time_in_rule_2));
                    sqlParameters.Add(new MySqlParameter("@time_before_work", ts.Time_before_work));
                    sqlParameters.Add(new MySqlParameter("@time_in_work", ts.Time_in_work));
                    sqlParameters.Add(new MySqlParameter("@time_after_work", ts.Time_after_work));
                    sqlParameters.Add(new MySqlParameter("@time_before_work_by_block", ts.Time_before_work_by_block));
                    sqlParameters.Add(new MySqlParameter("@time_after_work_by_block", ts.Time_after_work_by_block));
                    sqlParameters.Add(new MySqlParameter("@occupancy_rate_on_time", ts.Percentage_real_with_rule));
                    sqlParameters.Add(new MySqlParameter("@ic_error", ts.Ic_error));
                    try
                    {
                        ExecuteNonQuery(query, sqlParameters);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex.Message); LogHelper.Error(ex.StackTrace);
                        return false;
                    }
                    sqlParameters.Clear();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message); LogHelper.Error(ex.StackTrace);
                return false;
            }
            return true;
        }

        //public static bool UpdateTimeSheet_Yesterday(List<DTO.Timesheet> timesheets)
        //{
        //    List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
        //    string query = "";
        //    foreach (var ts in timesheets)
        //    {
        //        query = "SELECT COUNT(id) FROM timesheetbyday WHERE ic_card = @ic_card AND day_of_year = @day_of_year";
        //        sqlParameters.Add(new MySqlParameter("@ic_card", ts.Ic_card));
        //        sqlParameters.Add(new MySqlParameter("@day_of_year", ts.Date));
        //        try
        //        {
        //            int check_dupblicate = Convert.ToInt32(ExecuteScalar(query, sqlParameters));
        //            if (check_dupblicate > 0)
        //            {
        //                query = "UPDATE timesheetbyday SET employee_Code = @Employee_code,department = @department, type_of_day = @type_of_day, employee_Name = @employee_Name," +
        //                    "end_time_in_real = @end_time_in_real, total_time_in_real = @total_time_in_real," +
        //                    "total_time_in_rule_2 = @total_time_in_rule_2, time_before_work = @time_before_work, time_in_work = @time_in_work," +
        //                    "time_after_work = @time_after_work, time_before_work_by_block = @time_before_work_by_block, time_after_work_by_block = @time_after_work_by_block," +
        //                    "occupancy_rate_on_time = @occupancy_rate_on_time, ic_error = @ic_error WHERE ic_card = @ic_card AND day_of_year = @day_of_year";
        //            }
        //            sqlParameters.Add(new MySqlParameter("@department", ts.Department));
        //            sqlParameters.Add(new MySqlParameter("@type_of_day", ts.Type_of_day));
        //            sqlParameters.Add(new MySqlParameter("@employee_Code", ts.Employee_code));
        //            sqlParameters.Add(new MySqlParameter("@employee_Name", ts.Name));
        //            sqlParameters.Add(new MySqlParameter("@start_time_in_rule", ts.Start_time_in_rule));
        //            sqlParameters.Add(new MySqlParameter("@break_time_in_rule", ts.Break_time_in_rule));
        //            sqlParameters.Add(new MySqlParameter("@end_time_in_rule", ts.End_time_in_rule));
        //            sqlParameters.Add(new MySqlParameter("@total_time_rule", ts.Total_time_in_rule));
        //            sqlParameters.Add(new MySqlParameter("@start_time_in_real", ts.Start_time_in_real));
        //            sqlParameters.Add(new MySqlParameter("@end_time_in_real", ts.End_time_in_real));
        //            sqlParameters.Add(new MySqlParameter("@total_time_in_real", ts.Total_time_in_real));
        //            sqlParameters.Add(new MySqlParameter("@total_time_in_rule_2", ts.Total_time_in_rule_2));
        //            sqlParameters.Add(new MySqlParameter("@time_before_work", ts.Time_before_work));
        //            sqlParameters.Add(new MySqlParameter("@time_in_work", ts.Time_in_work));
        //            sqlParameters.Add(new MySqlParameter("@time_after_work", ts.Time_after_work));
        //            sqlParameters.Add(new MySqlParameter("@time_before_work_by_block", ts.Time_before_work_by_block));
        //            sqlParameters.Add(new MySqlParameter("@time_after_work_by_block", ts.Time_after_work_by_block));
        //            sqlParameters.Add(new MySqlParameter("@occupancy_rate_on_time", ts.Percentage_real_with_rule));
        //            sqlParameters.Add(new MySqlParameter("@ic_error", ts.Ic_error));
        //            try
        //            {
        //                ExecuteNonQuery(query, sqlParameters);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogHelper.Error(ex.Message); LogHelper.Error(ex.StackTrace);
        //                throw ex;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Error(ex.Message); LogHelper.Error(ex.StackTrace);
        //            throw ex;
        //        }
        //        sqlParameters.Clear();
        //    }
        //    return true;
        //}

        public static List<Employee> Getemployee()
        {
            List<Employee> employees = new List<Employee>();
            string sql = "SELECT * FROM employee";
            using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
            {
                try
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Employee ep = new Employee();
                            ep.Id = (int)reader["employee_Id"];
                            ep.Employee_code = (string)reader["employee_Code"];
                            ep.Name = (string)reader["employee_Name"];
                            ep.Ic_card = (string)reader["ic_card"];
                            ep.Start_work_time = (string)reader["start_work_time"];
                            ep.End_work_time = (string)reader["end_work_time"];
                            employees.Add(ep);
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error query" + sql);
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    //throw ex;
                    return null;
                }
            }
            return employees;
        }
        public static List<Calendar> GetCalender()
        {
            List<Calendar> calendars = new List<Calendar>();
            string sql = "SELECT * FROM holiday";
            using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
            {
                try
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Calendar cl = new Calendar();
                            cl.Id = (int)reader["id"];
                            cl.Day_of_year = (string)reader["day_of_year"];
                            cl.Type_of_day = (int)reader["type"];
                            cl.Status = (bool)reader["status"];
                            calendars.Add(cl);
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error query" + sql);
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    //throw ex; 
                    return null;
                }
            }
            return calendars;
        }
        public static List<Log> GetLogsByListDate(List<DateTime> dateTimes)
        {
            string arrayDate = "";
            foreach (DateTime dt in dateTimes)
            {
                arrayDate += "'" + dt.ToString("yyyy-MM-dd") + "'" + ",";
            }
            string arDate = arrayDate.Substring(0, arrayDate.Length - 1);
            List<Log> logs = new List<Log>();
            string sql = "SELECT * FROM log_time WHERE day_of_year in (" + arDate + ")";
            using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
            {
                try
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Log lg = new Log();
                            lg.ID = (int)reader["id"];
                            lg.Day_of_year = (String)reader["day_of_year"];
                            lg.Time_of_day = (String)reader["time_of_day"];
                            lg.Employee_id = (int)reader["employee_Id"];
                            lg.Ic_card = (string)reader["ic_card"];
                            lg.Employee_code = (string)reader["employee_Code"];
                            lg.Name = (string)reader["employee_Name"];
                            lg.Department = (string)reader["department"];
                            lg.Door_name = (string)reader["door_name"];
                            lg.Door_type = Convert.ToBoolean((reader)["door_type"]);
                            lg.State = Convert.ToBoolean((reader)["state"]);
                            lg.Status = Convert.ToBoolean((reader)["status"]);
                            logs.Add(lg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error query" + sql);
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    //throw ex;
                    return null;
                }
            }
            return logs;
        }
        //public static List<Log> Getlogs(DateTime date)
        //{
        //    string day = date.ToString("yyyy-MM-dd");
        //    string nextday = date.AddDays(1).ToString("yyyy-MM-dd");
        //    List<Log> logs = new List<Log>();

        //    string sql = "SELECT * FROM log_time WHERE day_of_year = '" + day + "' OR day_of_year = '" + nextday + "'";
        //    using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
        //    {
        //        try
        //        {
        //            c.Open();
        //            using (MySqlCommand cmd = new MySqlCommand(sql, c))
        //            {
        //                MySqlDataReader reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    Log lg = new Log();
        //                    lg.ID = (int)reader["id"];
        //                    lg.Day_of_year = (String)reader["day_of_year"];
        //                    lg.Time_of_day = (String)reader["time_of_day"];
        //                    lg.Employee_id = (int)reader["employee_Id"];
        //                    lg.Ic_card = (string)reader["ic_card"];
        //                    lg.Employee_code = (string)reader["employee_Code"];
        //                    lg.Name = (string)reader["employee_Name"];
        //                    lg.Department = (string)reader["department"];
        //                    lg.Door_name = (string)reader["door_name"];
        //                    lg.Door_type = Convert.ToBoolean((reader)["door_type"]);
        //                    lg.State = Convert.ToBoolean((reader)["state"]);
        //                    lg.Status = Convert.ToBoolean((reader)["status"]);
        //                    logs.Add(lg);
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Error("Error query" + sql);
        //            LogHelper.Error(ex.Message);
        //            LogHelper.Error(ex.StackTrace);
        //            throw ex;
        //        }
        //    }
        //    return logs;
        //}
        public static List<DateTime> Getdate_database()
        {
            List<DateTime> date = new List<DateTime>();
            string sql = "SELECT * FROM check_ts";
            using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
            {
                try
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DateTime Day = Convert.ToDateTime((reader)["day"]);
                            bool status = Convert.ToBoolean((reader)["status"]);
                            date.Add(Day);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error query" + sql);
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    //throw ex;
                    return null;
                }
            }
            return date;
        }
        public static bool Inser_list_date(List<DateTime> dtp)
        {
            //mySqlConnection.Open();

            List<MySqlParameter> sqlParameters;
            string query = "";
            foreach (var dt in dtp)
            {
                sqlParameters = new List<MySqlParameter>();
                query = "INSERT INTO check_ts (day,status) values(@Day, @Status)";
                sqlParameters.Add(new MySqlParameter("@Day", dt.ToString("yyyy-MM-dd")));
                sqlParameters.Add(new MySqlParameter("@Status", "0"));
                try
                {
                    ExecuteNonQuery(query, sqlParameters);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message); LogHelper.Error(ex.StackTrace);
                    //throw ex;
                    return false;
                }
            }
            return true;
        }
        public static List<DateTime> Getdate_insert_timesheet()
        {
            List<DateTime> date = new List<DateTime>();
            string sql = "SELECT * FROM check_ts WHERE status = '0'";
            using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
            {
                try
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, c))
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DateTime Day = Convert.ToDateTime((reader)["day"]);
                            bool status = Convert.ToBoolean((reader)["status"]);
                            date.Add(Day);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error query" + sql);
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    //throw ex;
                    return null;
                }
            }
            return date;
        }
        public static bool Delete_timsheet(List<DateTime> dateTimes)
        {
            string query;
            string arrayDate = "";
            foreach (DateTime dt in dateTimes)
            {
                arrayDate += "'" + dt.ToString("yyyy-MM-dd") + "'" + ",";
            }
            string arDate = arrayDate.Substring(0, arrayDate.Length - 1);
            query = "DELETE FROM timesheetbyday WHERE day_of_year in (" + arDate + ")";
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                return false;
            }
            return true;
        }

        //public static bool Delete_timsheet(List<DateTime> dateTimes)
        //{
        //    string query;
        //    string arrayDate = "";
        //    foreach (DateTime dt in dateTimes)
        //    {
        //        arrayDate += "'" + dt.ToString("yyyy-MM-dd") + "'" + ",";
        //    }
        //    string arDate = arrayDate.Substring(0, arrayDate.Length - 1);
        //    query = "DELETE FROM timesheetbyday WHERE day_of_year in (" + arDate + ")";
        //    using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
        //    {
        //        c.Open();
        //        var transaction = c.BeginTransaction();
        //        try
        //        {


        //            MySqlCommand mySqlCommand = new MySqlCommand(query, c);
        //            mySqlCommand.ExecuteNonQuery();
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Error(ex.Message);
        //            LogHelper.Error(ex.StackTrace);
        //            transaction.Rollback();
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public static bool Update_status_timesheet(DateTime date)
        //{
        //    string query;
        //    string day = date.ToString("yyyy-MM-dd");
        //    using (MySqlConnection c = new MySqlConnection(AppInfor.ConnectionString))
        //    {
        //        c.Open();
        //        var transaction = c.BeginTransaction();
        //        try
        //        {

        //            query = "UPDATE check_ts SET status = @status WHERE day = @day";
        //            MySqlCommand mySqlCommand = new MySqlCommand(query, c);
        //            mySqlCommand.Parameters.Add(new MySqlParameter("@status", "1"));
        //            mySqlCommand.Parameters.Add(new MySqlParameter("@day", day));
        //            mySqlCommand.ExecuteNonQuery();
        //            transaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.Error(ex.Message);
        //            LogHelper.Error(ex.StackTrace);
        //            transaction.Rollback();
        //            //throw ex;
        //            return false;
        //        }
        //    }          
        //    return true;
        //}

        public static bool Update_status_timesheet(DateTime date)
        {
            string query;
            string day = date.ToString("yyyy-MM-dd");
            try
            {
                query = "UPDATE check_ts SET status = @status WHERE day = @day";
                MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlCommand.Parameters.Add(new MySqlParameter("@status", "1"));
                mySqlCommand.Parameters.Add(new MySqlParameter("@day", day));
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                //throw ex;
                return false;
            }
            return true;
        }
    }
}
