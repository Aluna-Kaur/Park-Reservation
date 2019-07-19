using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public class Campgrounds
    {
        public int CampGroundId { get; set; }
        public int ParkId { get; set; }
        public string Name { get; set; }
        public int OpenFromMM { get; set; }
        public int OpenToMM { get; set; }
        public decimal DailyFee { get; set; }
    }
}
