using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI_Test_TIMESERVICE.DTO;

namespace UI_Test_TIMESERVICE
{
    static class CSVHelper
    {
        public static List<Employee> GetEmployees(string pathEmployeeCSV)
        {
            List<Employee> employees = new List<Employee>();
            int title = 1;
            string strHeader = "";
            int numberEmployee = 1;
            bool checkFormat = false;
            
            using (var fileStream = new FileStream(pathEmployeeCSV, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream, Encoding.GetEncoding(932)))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (title < 4)
                    {
                        title++;
                        strHeader += line;
                    }
                    else
                    {
                        if (!strHeader.Contains("名前") || !strHeader.Contains("開始時間") || !strHeader.Contains("終了時間"))
                        {
                            return null;
                        }
                        var values = line.Split(',');
                        if (!checkFormat)
                        {
                            if (values.Length != 5)
                            {
                                return null;
                            }
                            else
                            {
                                checkFormat = true;
                            }
                        }
                        Employee ep = new Employee();
                        try
                        {
                            try
                            {
                                ep.Id = numberEmployee;
                                ep.Ic_card = values[0];
                                ep.Employee_code = values[1];
                                ep.Name = values[2];
                                ep.Start_work_time = values[3];
                                ep.End_work_time = values[4];
                                ep.Status = true;
                                employees.Add(ep);
                                numberEmployee++;
                            }
                            catch (IndexOutOfRangeException ioore)
                            {
                                LogHelper.Error(ioore.StackTrace);
                                throw ioore;
                            }
                        }
                        catch (FormatException fe)
                        {
                            LogHelper.Error(fe.StackTrace);
                            throw fe;
                        }
                    }

                }

            }
            return employees;
        }
        /// <summary>
        ///  Read file Calendar
        /// </summary>
        /// <param name="pathCalendarCSV"></param>
        /// <returns></returns>

        const string first_tod = "";
        const string second_tod = "〇";
        const string third_tod = "◎";
        const string fourth_tod = "●";
        public static List<Calendar> GetCalendars(string pathCalendarCSV)
        {
            List<Calendar> calendars = new List<Calendar>();
            int title = 1;
            bool click = true;
            string strHeader = "";

            int number_of_weekend = 0;
            int number_of_vacation = 0;
            int number_of_bonus = 0;
            using (var fileStream = new FileStream(pathCalendarCSV, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream, Encoding.GetEncoding(932)))
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (title < 6)
                    {
                        title++;
                        strHeader += line;
                    }
                    else
                    {
                        if (!strHeader.Contains("一斉有休休暇：") || !strHeader.Contains("祝日") || !strHeader.Contains("日数") || !strHeader.Contains("休日"))
                        {

                            return null;
                        }

                        var values = line.Split(',');
                        UI_Test_TIMESERVICE.DTO.Calendar cld = new UI_Test_TIMESERVICE.DTO.Calendar();
                        try
                        {
                            try
                            {
                                DateTime doy = DateTime.Parse(values[0]);
                                if (click)
                                {

                                    click = false;
                                }
                                cld.Day_of_year = doy.ToString("yyyy-MM-dd");
                                cld.Day_of_week = values[1];
                                cld.Status = true;
                                cld.Tod_display = values[2];
                                switch (values[2])
                                {
                                    case first_tod:
                                        cld.Type_of_day = 0;
                                        break;

                                    case second_tod:
                                        cld.Type_of_day = 1;
                                        number_of_weekend++;
                                        break;

                                    case third_tod:
                                        cld.Type_of_day = 2;
                                        number_of_vacation++;
                                        break;

                                    case fourth_tod:
                                        cld.Type_of_day = 3;
                                        number_of_bonus++;
                                        break;

                                    default:
                                        break;
                                }
                                calendars.Add(cld);

                            }
                            catch (IndexOutOfRangeException ioore)
                            {
                                LogHelper.Error(ioore.StackTrace);
                                throw ioore;
                            }
                        }
                        catch (FormatException fe)
                        {
                            LogHelper.Error(fe.StackTrace);
                            throw fe;
                        }
                    }
                }
            return calendars;
        }

        /// <summary>
        /// Read file logs
        /// </summary>
        /// <param name="pathLogCSV"></param>
        /// <returns></returns>
        const string forget_card = "予備";
        const string ignore_door_in = "6F 事務所入口　入室";
        const string ignore_door_out = "6F 事務所入口　退室";
        const string state_in = "入室";
        const string state_out = "退室";
        static int count_log = 1;

        public static List<Log> GetLogs(string pathLogCSV, List<Employee> employee_info)
        {
            List<Log> logs = new List<Log>();

            if (!File.Exists(pathLogCSV))
            {
                LogHelper.Error("PATH EMPLOYEES NOT EXIST with path :" + pathLogCSV, new IOException());
                return null;
            }
            bool title = true;
                using (var fileStream = new FileStream(pathLogCSV, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream, Encoding.GetEncoding(932)))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        if (title)
                        {
                            title = false;
                        }
                        else
                        {
                            var values = line.Split(',');
                            if (values.Length != 8)
                            {
                                continue;
                            }
                            if (!String.IsNullOrEmpty(values[5]))
                            {
                                bool check_ready = false;
                                int id = 0;
                                string ic = "";
                                string code = "";
                                string name = "";
                                //string department = "";
                                if (values[4] != forget_card)
                                {
                                    ic = values[5].Substring(0, 3);
                                    var code_name = values[5].Substring(3).Trim();
                                    code = code_name.Substring(0, 5);
                                    name = code_name.Substring(5);
                                    var employee = employee_info.AsEnumerable()
                                        .Where(x => (x.Ic_card == ic && x.Employee_code == code)).FirstOrDefault();
                                    if (employee != null)
                                    {
                                        check_ready = true;
                                        id = employee.Id;
                                    }
                                    else
                                    {
                                        check_ready = false;
                                    }
                                }
                                else
                                {
                                    ic = values[5].Substring(0, 3);
                                    var eInfo = employee_info.Where(e => (e.Ic_card == ic)).FirstOrDefault();
                                    if (eInfo != null)
                                    {
                                        check_ready = true;
                                        //department = eInfo.Department;
                                        code = eInfo.Employee_code;
                                        name = eInfo.Name;
                                        id = eInfo.Id;
                                    }
                                    else
                                    {
                                        check_ready = false;
                                    }
                                }
                                if (check_ready)
                                {
                                    Log lg = new Log();
                                    try
                                    {
                                        var datetimes = values[0].Split(' ', ' ');
                                        DateTime doy = DateTime.Parse(datetimes[0]);
                                        //DateTime doy = DateTime.Parse(values[0].Substring(0, 10));
                                        lg.Day_of_year = doy.ToString("yyyy-MM-dd");
                                        //lg.Time_of_day = values[0].Substring(values[0].Length - 8);
                                        lg.Time_of_day = datetimes[1];
                                        lg.Door_name = values[1];
                                        if (lg.Door_name.Equals(ignore_door_in) || lg.Door_name.Equals(ignore_door_out))
                                        {
                                            lg.Door_type = false;
                                        }
                                        else
                                        {
                                            lg.Door_type = true;
                                        }
                                        string state = values[1].Substring(values[1].Length - 2, 2);
                                        if (state == state_in)
                                        {
                                            lg.State = true;
                                        }
                                        if (state == state_out)
                                        {
                                            lg.State = false;
                                        }
                                        lg.Ic_card = values[5].Substring(0, 3);
                                        if (values[5] != forget_card)
                                        {
                                            lg.Department = values[4];
                                            lg.Employee_code = code;
                                            lg.Name = name;
                                            lg.Employee_id = id;
                                        }
                                        else
                                        {
                                            //lg.Department = department;
                                            lg.Department = values[4];
                                            lg.Employee_code = code;
                                            lg.Name = name;
                                            lg.Employee_id = id;
                                        }
                                        employee_info.Where(e => e.Id == lg.Employee_id).FirstOrDefault().Department = lg.Department;
                                        lg.Status = true;
                                        lg.ID = count_log;
                                        logs.Add(lg);
                                        count_log++;
                                    }
                                    catch (IndexOutOfRangeException ioore)
                                    {
                                        LogHelper.Error(ioore.StackTrace);
                                        throw ioore;
                                    }
                                    catch (FormatException fe)
                                    {
                                        LogHelper.Error(fe.StackTrace);
                                        throw fe;
                                    }
                                }
                            }
                        }
                    }
                }


                return logs;
            }
        

    }
    
    

}
     

