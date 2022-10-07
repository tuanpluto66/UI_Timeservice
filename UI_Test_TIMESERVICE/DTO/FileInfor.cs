using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_TIMESERVICE.DTO
{
    internal class FileInfor
    {
        private bool _isRemove;
        private int _no;
        private string _name;
        private string _capacity;
        private string _path;
        private int _day;

        public FileInfor() { }

        public bool IsRemove
        {
            get
            {
                return _isRemove;
            }
            set
            {
                _isRemove = value;
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

        public string Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                _capacity = value;
            }
        }
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        public int Day
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
    }
}
