using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    public class Calendar
    {
            private int _id;
            private string _day_of_year;
            private string _day_of_week;
            private int _type_of_day;
            private bool _status;
            private string _tod_display;

            public Calendar() { }

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

            public string Day_of_year
            {
                get
                {
                    return _day_of_year;
                }
                set
                {
                    _day_of_year = value;
                }
            }

            public string Day_of_week
            {
                get
                {
                    return _day_of_week;
                }
                set
                {
                    _day_of_week = value;
                }
            }

            public int Type_of_day
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

            public bool Status
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

            public string Tod_display
            {
                get
                {
                    return _tod_display;
                }
                set
                {
                    _tod_display = value;
                }
            }
        }
    }

