using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWPortal.Areas.Report.Models
{
    public class ReportModel
    {
        public int OrderId { get; set; }

        public string QuestionId { get; set; }

        public string Question { get; set; }

        public string T3B { get; set; }

        public string Details { get; set; }

        public string Total { get; set; }                   
    }


    public class ReportDisplayModel
    {
        public List<ReportModel> ReportList{ get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string SelectedVan { get; set; }

        public string CurrentReport { get; set; }
    }


    public class ReportChartData
    {       
        public string ReportMonth { get; set; }              

        public string Total { get; set; }

        public string T3B { get; set; }

        public string Data1 { get; set; }

        public string Data2 { get; set; }

        public string Data3 { get; set; }

        public string Data4 { get; set; }

        public string Data5 { get; set; }
    }


    public class ReportChartModel
    {
        public string QusetionId { get; set; }

        public string FieldType { get; set; }

        public List<ReportChartData> ChartDataList { get; set; }
    }
    
}