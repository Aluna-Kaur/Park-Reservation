using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public class Site
    {
        public int SiteId { get; set; }
        public int CampgroundId { get; set; }
        public int SiteNumber { get; set; }
        public int MaxOccupy { get; set; }
        public bool Acessible { get; set; }
        public int MaxRvLength { get; set; }
        public bool Utilities { get; set; }
    }
}
