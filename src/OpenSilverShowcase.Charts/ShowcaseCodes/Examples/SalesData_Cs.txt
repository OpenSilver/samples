using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSilverShowcase.Charts.Examples
{
    public class SalesDataPoint
    {
        public string Month { get; set; }
        public int Sales { get; set; }
    }

    public static class Sales
    {
        public static List<SalesDataPoint> Chairs
        {
            get
            {
                return new List<SalesDataPoint>
                {
                    new SalesDataPoint { Month = "Jan", Sales = 120 },
                    new SalesDataPoint { Month = "Feb", Sales = 180 },
                    new SalesDataPoint { Month = "Mar", Sales = 150 },
                    new SalesDataPoint { Month = "Apr", Sales = 90 },
                    new SalesDataPoint { Month = "May", Sales = 110 },
                    new SalesDataPoint { Month = "Jun", Sales = 200 }
                };
            }
        }

        public static List<SalesDataPoint> Tables
        {
            get
            {
                return new List<SalesDataPoint>
                {
                    new SalesDataPoint { Month = "Jan", Sales = 80 },
                    new SalesDataPoint { Month = "Feb", Sales = 60 },
                    new SalesDataPoint { Month = "Mar", Sales = 95 },
                    new SalesDataPoint { Month = "Apr", Sales = 140 },
                    new SalesDataPoint { Month = "May", Sales = 170 },
                    new SalesDataPoint { Month = "Jun", Sales = 130 }
                };
            }
        }
    }
}