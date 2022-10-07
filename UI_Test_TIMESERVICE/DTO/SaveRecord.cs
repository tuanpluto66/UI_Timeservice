using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    public class SaveRecord
    {
        private TimeSpan _time;
        private bool _state;
        private State_of_work _state_of_work;

        public TimeSpan Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
            }
        }

        public bool State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public State_of_work State_of_work
        {
            get
            {
                return _state_of_work;
            }
            set
            {
                _state_of_work = value;
            }
        }

        public bool CompareRecord(SaveRecord sr)
        {
            if (TimeSpan.Compare(this.Time, sr.Time) == 0 &&
               this.State == sr.State &&
               this.State_of_work == sr.State_of_work)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public enum State_of_work
    {
        before,
        before_lunch_break,
        lunch_break,
        after_lunch_break,
        after,
    }
}

