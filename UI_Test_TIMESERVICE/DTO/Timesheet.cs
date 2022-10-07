using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    public class Timesheet
    {
        private string _department;
        private string _type_of_day;
        private string _ic_card;
        private string _employee_code;
        private string _name;
        private int _employee_id;
        private TimeSpan _start_time_in_rule;
        private TimeSpan _end_time_in_rule;
        private TimeSpan _break_time_in_rule;
        private string _date;
        private TimeSpan _start_time_in_real;
        private TimeSpan _end_time_in_real;
        private TimeSpan _go_on_business_time;
        private TimeSpan _go_on_business_time_in_work_time;
        private TimeSpan _time_before_work;
        private TimeSpan _time_in_work;
        private TimeSpan _time_after_work;
        private TimeSpan _time_before_work_by_block;
        private TimeSpan _time_after_work_by_block;
        private int _ic_error;
        private bool _work;

        public Timesheet()
        { }

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

        public string Type_of_day
        {
            get
            {
                return _type_of_day;
            }
            set
            {
                _type_of_day = value;
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

        public int Employee_id
        {
            get
            {
                return _employee_id;
            }
            set
            {
                _employee_id = value;
            }
        }

        public TimeSpan Start_time_in_rule
        {
            get
            {
                return _start_time_in_rule;
            }
            set
            {
                _start_time_in_rule = value;
            }
        }

        public TimeSpan End_time_in_rule
        {
            get
            {
                return _end_time_in_rule;
            }
            set
            {
                _end_time_in_rule = value;
            }
        }

        public TimeSpan Break_time_in_rule
        {
            get
            {
                return _break_time_in_rule;
            }
            set
            {
                _break_time_in_rule = value;
            }
        }

        public TimeSpan Total_time_in_rule
        {
            get
            {
                return _end_time_in_rule - _start_time_in_rule - _break_time_in_rule;
            }
        }

        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        public TimeSpan Start_time_in_real
        {
            get
            {
                return _start_time_in_real;
            }
            set
            {
                _start_time_in_real = value;
            }
        }

        public TimeSpan End_time_in_real
        {
            get
            {
                return _end_time_in_real;
            }
            set
            {
                _end_time_in_real = value;
            }
        }

        public TimeSpan Go_on_business_time
        {
            get
            {
                return _go_on_business_time;
            }
            set
            {
                _go_on_business_time = value;
            }
        }

        public TimeSpan Go_on_business_time_in_work_time
        {
            get
            {
                return _go_on_business_time_in_work_time;
            }
            set
            {
                _go_on_business_time_in_work_time = value;
            }
        }

        public TimeSpan Total_time_in_real
        {
            get
            {
                TimeSpan result;
                TimeSpan total_time;
                if (_start_time_in_real >= _end_time_in_real)
                    total_time = (_end_time_in_real - _start_time_in_real) + new TimeSpan(24, 0, 0);
                else
                    total_time = _end_time_in_real - _start_time_in_real;

                TimeSpan day_point = new TimeSpan(5, 0, 0);
                TimeSpan start_break = new TimeSpan(12, 0, 0);
                TimeSpan end_break = new TimeSpan(13, 0, 0);

                if (TimeSpan.Compare(Start_time_in_real, start_break) <= 0)
                {
                    if (TimeSpan.Compare(End_time_in_real, start_break) <= 0 && TimeSpan.Compare(End_time_in_real, day_point) > 0)
                    {
                        result = total_time;
                    }
                    else if ((TimeSpan.Compare(End_time_in_real, start_break) > 0 && TimeSpan.Compare(End_time_in_real, end_break) < 0))
                    {
                        result = start_break.Subtract(Start_time_in_real);
                    }
                    else
                    {
                        result = total_time.Subtract(Break_time_in_rule);
                    }
                }
                else if (TimeSpan.Compare(Start_time_in_real, start_break) > 0 && TimeSpan.Compare(Start_time_in_real, end_break) < 0)
                {
                    if ((TimeSpan.Compare(End_time_in_real, start_break) > 0 && TimeSpan.Compare(End_time_in_real, end_break) < 0))
                    {
                        result = new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        if (end_break >= End_time_in_real)
                            result = End_time_in_real.Subtract(end_break) + new TimeSpan(24, 0, 0);
                        else
                            result = End_time_in_real.Subtract(end_break);
                    }
                }
                else
                {
                    result = total_time;
                }
                return result - Go_on_business_time;
            }
        }

        public TimeSpan Total_time_in_rule_2
        {
            get
            {
                TimeSpan result;
                TimeSpan total_time;
                TimeSpan endTime;
                TimeSpan startTime;
                TimeSpan day_point = new TimeSpan(5, 0, 0);
                TimeSpan start_break = new TimeSpan(12, 0, 0);
                TimeSpan end_break = new TimeSpan(13, 0, 0);
                if (_start_time_in_real >= End_time_in_rule && End_time_in_rule > day_point)
                {
                    return TimeSpan.Parse("0");
                }
                if (_end_time_in_real <= Start_time_in_rule && _end_time_in_real > day_point)
                {
                    return TimeSpan.Parse("0");
                }
                if (_start_time_in_real <= _start_time_in_rule || (_start_time_in_real >= day_point && _start_time_in_rule <= day_point))
                {
                    startTime = _start_time_in_rule;
                }
                else
                {
                    startTime = _start_time_in_real;
                }
                if (_end_time_in_real >= _end_time_in_rule || (_end_time_in_real <= day_point && _end_time_in_rule >= day_point))
                {
                    endTime = _end_time_in_rule;
                }
                else
                {
                    endTime = _end_time_in_real;
                }
                if (startTime >= endTime)
                    total_time = (endTime - startTime) + new TimeSpan(24, 0, 0);
                else
                    total_time = endTime - startTime;

                if (TimeSpan.Compare(startTime, start_break) <= 0)
                {
                    if (TimeSpan.Compare(endTime, start_break) <= 0 && TimeSpan.Compare(endTime, day_point) > 0)
                    {
                        result = total_time;
                    }
                    else if ((TimeSpan.Compare(endTime, start_break) > 0 && TimeSpan.Compare(endTime, end_break) < 0))
                    {
                        result = start_break.Subtract(startTime);
                    }
                    else
                    {
                        result = total_time.Subtract(Break_time_in_rule);
                    }
                }
                else if (TimeSpan.Compare(startTime, start_break) > 0 && TimeSpan.Compare(startTime, end_break) < 0)
                {
                    if ((TimeSpan.Compare(endTime, start_break) > 0 && TimeSpan.Compare(endTime, end_break) < 0))
                    {
                        result = new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        if (end_break >= endTime)
                            result = endTime.Subtract(end_break) + new TimeSpan(24, 0, 0);
                        else
                            result = endTime.Subtract(end_break);
                    }
                }
                else
                {
                    result = total_time;
                }
                return result - Go_on_business_time_in_work_time;
            }
        }

        public TimeSpan Time_before_work
        {
            get
            {
                return _time_before_work;
            }
            set
            {
                _time_before_work = value;
            }
        }

        public TimeSpan Time_in_work
        {
            get
            {
                return _time_in_work;
            }
            set
            {
                _time_in_work = value;
            }
        }

        public TimeSpan Time_after_work
        {
            get
            {
                return _time_after_work;
            }
            set
            {
                _time_after_work = value;
            }
        }

        public TimeSpan Time_before_work_by_block
        {
            get
            {
                return _time_before_work_by_block;
            }
            set
            {
                _time_before_work_by_block = value;
            }
        }

        public TimeSpan Time_after_work_by_block
        {
            get
            {
                return _time_after_work_by_block;
            }
            set
            {
                _time_after_work_by_block = value;
            }
        }

        public string Percentage_real_with_rule
        {
            get
            {
                if (Total_time_in_rule_2.TotalSeconds == 0)
                {
                    return String.Format("{0:0.00}", 0) + "%";
                }
                else
                {
                    return String.Format("{0:0.00}", _time_in_work.TotalSeconds / Total_time_in_rule_2.TotalSeconds * 100) + "%";
                }
            }
        }

        public int Ic_error
        {
            get
            {
                return _ic_error;
            }
            set
            {
                _ic_error = value;
            }
        }

        public bool Work
        {
            get
            {
                return _work;
            }
            set
            {
                _work = value;
            }
        }

        public TimeSpan End_time_over_day
        {
            get
            {
                if (End_time_in_real <= new TimeSpan(5, 0, 0))
                {
                    return _end_time_in_real + new TimeSpan(24, 0, 0);
                }
                else
                {
                    return _end_time_in_real;
                }
            }
        }
    }
}
