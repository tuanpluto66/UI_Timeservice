using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    public class Log
    {
        private int _id;
        private string _employee_code;
        private string _name;
        private string _ic_card;
        private string _department;
        private string _day_of_year;
        private string _time_of_day;
        private string _door_name;
        private bool _door_type;
        private bool _state;
        private bool _status;
        private int _employee_id;

        public Log()
        { }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Employee_code
        {
            get { return _employee_code; }
            set { _employee_code = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Ic_card
        {
            get { return _ic_card; }
            set { _ic_card = value; }
        }
        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }
        public string Day_of_year
        {
            get { return _day_of_year; }
            set { _day_of_year = value; }
        }
        public string Time_of_day
        {
            get { return _time_of_day; }
            set { _time_of_day = value; }
        }
        public string Door_name
        {
            get { return _door_name; }
            set { _door_name = value; }
        }
        public bool Door_type
        {
            get { return _door_type; }
            set { _door_type = value; }
        }
        public bool State
        {
            get { return _state; }
            set { _state = value; }
        }
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public int Employee_id
        {
            get { return _employee_id; }
            set { _employee_id = value; }
        }
    }
}
