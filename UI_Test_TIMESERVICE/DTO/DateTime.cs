using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    public class DateTimeA
    {
        private int _id;
        private string _day;
        private bool _status;
        public DateTimeA() { }
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

        public string Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
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

    }
}
