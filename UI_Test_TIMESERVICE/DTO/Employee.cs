using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    public class Employee
    {
        private int _id;
        private int _no;
        private string _employee_code;
        private string _name;
        private string _ic_card;
        private string _department;
        private string _start_work_time;
        private string _end_work_time;
        private bool _last_day_status;
        private DateTime _last_day;
        private TimeSpan _last_day_time;
        private bool _status;

        public Employee()
        {
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public int No
        {
            get
            {
                return _no;
            }
            set
            {
                _no = value;
            }
        }

        public string Employee_code
        {
            get
            {
                return _employee_code;
            }
            set
            {
                _employee_code = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Ic_card
        {
            get
            {
                return _ic_card;
            }
            set
            {
                _ic_card = value;
            }
        }

        public string Department
        {
            get
            {
                return _department;
            }
            set
            {
                _department = value;
            }
        }

        public string Start_work_time
        {
            get
            {
                return _start_work_time;
            }
            set
            {
                _start_work_time = value;
            }
        }

        public string End_work_time
        {
            get
            {
                return _end_work_time;
            }
            set
            {
                _end_work_time = value;
            }
        }

        public bool Last_day_status
        {
            get
            {
                return _last_day_status;
            }
            set
            {
                _last_day_status = value;
            }
        }

        public DateTime Last_day
        {
            get
            {
                return _last_day;
            }
            set
            {
                _last_day = value;
            }
        }

        public TimeSpan Last_day_time
        {
            get
            {
                return _last_day_time;
            }
            set
            {
                _last_day_time = value;
            }
        }

        public Boolean Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
    }
}
