using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoHec.Models
{
    public class CompDetails
    {
        public CompanyData companyData { get; set; }
        public List<double> tableData { get; set; }
    }
    public class CompanyData
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string assetType { get; set; }
        public string exchange { get; set; }
        public string currency { get; set; }
        public string country { get; set; }
        public string sector { get; set; }
        public string industry { get; set; }
    }
}
