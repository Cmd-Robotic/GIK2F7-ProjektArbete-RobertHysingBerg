using System;
using System.Collections.Generic;
using System.Text;

namespace Frontend
{
    class GameInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int grade { get; set; }
        public string image { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}
