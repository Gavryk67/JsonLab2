using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class CPU
    {
        public string Model { get; set; }
        public int Cores { get; set; }
        public string BaseClock { get; set; }
        public string BoostClock { get; set; }
        public string Description { get; set; }
    }
}