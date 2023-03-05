using System;
using System.Collections.Generic;

namespace ScheduleWebApp.Types
{
    public class ListDateRange
    {
        public List<Date> dates { get; set; }


        public class Date
        {
            public DateTime dateTimeStart { get; set; }
            public DateTime dateTimeEnd { get; set; }
        }
    }


}
