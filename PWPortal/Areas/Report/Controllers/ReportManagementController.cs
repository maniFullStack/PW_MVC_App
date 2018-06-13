using PWPortal.Areas.Report.Models;
using PWPortal.Common;
using SharedClass.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PWPortal.Areas.Report.Controllers
{
    public class ReportManagementController : Controller
    {
        //
        // GET: /Report/ReportManagement/
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Load report here
        /// </summary>
        /// <returns></returns>
        public ActionResult ReportDisplay()
        {
            //ViewBag.currentRptType = "GESS";
            //ViewBag.currentVanType = "ALL";
            //ViewBag.defaultFrom = "2016-01-14 21:09:00.000";
            //ViewBag.defaultTo = "2016-12-14 21:09:00.000";
            //var model = new List<ReportModel>();   

            var model = new ReportDisplayModel();
            model.CurrentReport = "GESS";
            model.SelectedVan = "ALL";
            model.FromDate = "2016-01-14 21:09:00.000";
            model.ToDate = "2016-12-14 21:09:00.000";
            model.ReportList = new List<ReportModel>();
          
            return PartialView(model);
        }


        /// <summary>
        /// Load report chart
        /// </summary>
        /// <returns></returns>
        public ActionResult ReportChart(string questionId = null, string frDt = null, string toDt = null)
        {
            TempData["qnId"] = questionId;
            TempData["frDt"] = frDt;
            TempData["toDt"] = toDt;

            var model = new ReportChartModel();
            return PartialView(model);
        }

        /// <summary>
        /// Get Report to display
        /// </summary>
        /// <param name="rptType"></param>
        /// <param name="van"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public JsonResult GetReports(string rptType, string van, DateTime fromDate, DateTime toDate)      
        {            
            var model = new List<ReportModel>();         

            int fromMnth = Convert.ToInt32(fromDate.ToString().Split('/')[0]);
            int toMnth = Convert.ToInt32(toDate.ToString().Split('/')[0]);

            DataTable dtReport = AccountDbAccess.GetReport(rptType, van, fromMnth, toMnth);
            DataTable dtQuestions = AccountDbAccess.GetQuestions(rptType);

            if(dtReport != null && dtQuestions != null)
            {
                DataTable dtResult = SubFunctions.DesignReport(dtReport, dtQuestions, rptType);
                        
                if (dtResult != null)
                {
                    foreach (DataRow row in dtResult.Rows)
                    {
                        model.Add(new ReportModel
                        {
                            OrderId = Convert.ToInt32(row["OrderId"]),
                            QuestionId = row["QuestionId"].ToString(),
                            Question = row["Question"].ToString(),
                            T3B = row["T3B"].ToString(),
                            Details = row["Details"].ToString(),
                            Total = row["Total"].ToString()
                        });
                    }
                    return Json(new { data = model }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        /// <summary>
        /// Load chart data
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChartData(string reportType)
        {
            int fromMnth = Convert.ToInt32(TempData["frDt"].ToString().Split('/')[0]);
            int toMnth = 12;
            //int toMnth = Convert.ToInt32(TempData["toDt"].ToString().Split('/')[0]);

            //Load Report details           
            DataTable dtReport = AccountDbAccess.GetMonthlyRecordOfQuestion(reportType, TempData["qnId"].ToString(), fromMnth, toMnth);

            if (dtReport != null)
            {
                var model = new ReportChartModel();
                model.ChartDataList = new List<ReportChartData>();
                model.QusetionId = TempData["qnId"].ToString();

                switch(dtReport.Columns.Count)
                {
                    case 4:
                        model.FieldType = ChartTypes.OneField.ToString();
                        break;

                    case 5:
                        model.FieldType = ChartTypes.TwoField.ToString();
                        break;

                    case 6:
                        model.FieldType = ChartTypes.ThreeField.ToString();
                        break;

                    case 8:
                        model.FieldType = ChartTypes.FiveField.ToString();
                        break;
                }
                
                foreach(DataRow row in dtReport.Rows)
                {
                    if(model.FieldType == ChartTypes.OneField.ToString())
                    {
                        model.ChartDataList.Add(new ReportChartData
                        {
                            Data1 = row["Ans"].ToString(),                            
                            Total = row["Total"].ToString(),
                            ReportMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(row["ReportMonth"])),
                        });
                    }
                    else if (model.FieldType == ChartTypes.TwoField.ToString())
                    {
                        model.ChartDataList.Add(new ReportChartData
                        {
                            Data1 = row["YES"].ToString(),
                            Data2 = row["NO"].ToString(),
                            Total = row["Total"].ToString(),
                            ReportMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(row["ReportMonth"])),
                        });
                    }
                    else if (model.FieldType == ChartTypes.ThreeField.ToString())
                    {
                        model.ChartDataList.Add(new ReportChartData
                        {
                            Data1 = row["YES"].ToString(),
                            Data2 = row["NO"].ToString(),
                            Data3 = row["NA"].ToString(),
                            Total = row["Total"].ToString(),
                            ReportMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(row["ReportMonth"])),
                        });
                    }
                    else if (model.FieldType == ChartTypes.FiveField.ToString())
                    {
                        model.ChartDataList.Add(new ReportChartData
                        {
                            Data1 = row["VD"].ToString(),
                            Data2 = row["SD"].ToString(),
                            Data3 = row["SS"].ToString(),
                            Data4 = row["VS"].ToString(),
                            Data5 = row["NA"].ToString(),
                            Total = row["Total"].ToString(),
                            ReportMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(row["ReportMonth"])),
                        });
                    }                   
                }
                
                return Json(new { data = model }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;                
            }           
        }


	}
}