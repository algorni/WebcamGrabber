using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebcamToLobe
{
    public class Data
    {
        public DateTime? lastSnowingStartTime { get; set; }
        public DateTime? lastSnowingNotification { get; set; }
        public DateTime? lastSnowingEndTime { get; set; }
        public bool isSnowing { get; set; }
    }

    public class statusObjects
    {
        public string owner { get; set; }
        public string _id { get; set; }
        public string name { get; set; }
        public Data data { get; set; }
    }


}
