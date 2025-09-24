using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OpenSilverShowcase.Charts.Examples
{
    public class ProductSalesDataPoint
    {
        public string ProductName { get; set; }
        public int SalesAmount { get; set; }
    }

    public static class ProductSalesData
    {
        public static List<ProductSalesDataPoint> GetSalesData()
        {
            return new List<ProductSalesDataPoint>
            {
                new ProductSalesDataPoint { ProductName = "Chairs", SalesAmount = 280 },
                new ProductSalesDataPoint { ProductName = "Tables", SalesAmount = 200 },
                new ProductSalesDataPoint { ProductName = "Sofas", SalesAmount = 150 },
                new ProductSalesDataPoint { ProductName = "Cabinets", SalesAmount = 120 }
            };
        }
    }
}
