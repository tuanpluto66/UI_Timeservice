using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI_Test_TIMESERVICE.DTO;

namespace UI_Test_TIMESERVICE
{

    static class ValidateHelper
    {
        /// <summary>
        /// ValidateEmployees
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public static bool ValidateEmployees(List<Employee> employees)
        {
            if (employees != null && employees.Count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// ValidateCalender
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public static bool ValidateCalender(List<UI_Test_TIMESERVICE.DTO.Calendar> calendars)
        {
            if (calendars != null && calendars.Count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// ValidateLogs
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public static bool ValidateLogs(List<Log> logs)
        {
            if (logs != null && logs.Count > 0)
            {
                return true;
            }
            return false;
        }

    }
}
