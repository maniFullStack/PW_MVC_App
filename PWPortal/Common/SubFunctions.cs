using PWPortal.Models;
using SharedClass;
using SharedClass.Enums;
using SharedClass.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebsiteUtilities;

namespace PWPortal.Common
{
    public static class SubFunctions
    {
        public static PasswordResetCode SendResetEmail(string email)
        {
            try
            {
                PasswordResetCode responseCode;
                int outputMessage;

                if (!Validation.RegExCheck(email, ValidationType.Email))
                {
                    return PasswordResetCode.InvalidEmail;
                }

                //Check for valid user account email and Get new GUID for password reset                 
                string newGuid = AccountDbAccess.ResetPassword(email, out outputMessage);               

                if (string.IsNullOrEmpty(newGuid))
                {
                    return PasswordResetCode.InvalidEmail;
                }

                string mail_Content = AccountResource.ResourceManager.GetString("mailContent");
                string subject = AccountResource.ResourceManager.GetString("mailSubject");
                string frmAddr = AccountResource.ResourceManager.GetString("fromAddress");
                string frmName = AccountResource.ResourceManager.GetString("displayName");

                if (CommonFunctions.SendMail((int)MailType.mail_pswdReset, subject, email, email, frmAddr, frmName, mail_Content, newGuid))
                {
                    responseCode = PasswordResetCode.Success;
                }
                else
                {
                    responseCode = PasswordResetCode.CriticalError;
                }

                //responseCode = (MailCode)outputMessage;

                return responseCode;
            }
            catch (Exception ex)
            {
                //SubFunctions.LogErrors("TarionForum.ResetPassword", HttpContext.GetLocalResourceObject("~/Users/ResetPassword.aspx", "err_SendResetMail").ToString(), ErrorTypes.General, ex);
                return PasswordResetCode.CriticalError;
            }         
        }

        /// <summary>
        /// Validating user with userID and Username properties
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ValidateUser(out string message)
        {
            message = null;

            if (UserInfoModel.UserID < -1 || UserInfoModel.UserID == 0)
            {
                message = AccountResource.Error_InvalidUser;
                return false;
            }
            if (String.IsNullOrEmpty(UserInfoModel.Username))
            {
                message = AccountResource.Error_NoUsernameSpecified;
                return false;
            }
            if (UserInfoModel.Username.Length < 4 || UserInfoModel.Username.Length > 150)
            {
                message = AccountResource.Error_InvalidEmailLength;
                return false;
            }
            if (!Validation.RegExCheck(UserInfoModel.Email, ValidationType.Email))
            {
                message = AccountResource.Error_InvalidEmail;
                return false;
            }

            return (message == null);
        }


        /// <summary>
        /// Sending mail to new user
        /// </summary>
        /// <returns></returns>
        public static bool SendNewAccountMail(string userName, string firstName, string lastName, string email, string pswd)
        {
            try
            {
                string mail_Content = UserResource.mailContent;
                string subject = UserResource.mailSubject;
                string frmAddr = UserResource.fromAddress;
                string frmName = UserResource.displayName;

                return CommonFunctions.SendMail((int)MailType.mail_login, subject, email, firstName + " " + lastName, frmAddr, frmName, mail_Content, null, userName, pswd);
            }
            catch (Exception ex)
            {
                //CommonFunctions.LogErrors("TarionForum.AddUser", HttpContext.GetLocalResourceObject("~/Users/AddUser.aspx", "err_NewMail").ToString(), ErrorTypes.General, ex);
                return false;
            }
        }


        /// <summary>
        /// Design and generate report with data
        /// </summary>
        /// <param name="dtReport"></param>
        /// <param name="dtQuestions"></param>
        /// 
        /// <param name="rptType"></param>
        /// <returns></returns>
        public static DataTable DesignReport(DataTable dtReport, DataTable dtQuestions, string reportType)
        {
            try
            {
                int orderId = 0;
                string numbering = null;
                DataTable designDt = new DataTable();
                designDt.Columns.AddRange(new DataColumn[6] { new DataColumn("OrderId", typeof(Int32)), new DataColumn("QuestionId", typeof(string)), new DataColumn("Question", typeof(string)), 
                                            new DataColumn("T3B", typeof(string)), new DataColumn("Details", typeof(string)), new DataColumn("Total", typeof(string)) });

                //Gess report
                if (reportType == "GESS")
                {
                    foreach (DataRow row in dtReport.Rows)
                    {
                        int top3Total = 0;
                        string resultQ = null;                        

                        if (row["QuestionNumber"].ToString() == "QA2")
                        {
                            resultQ = "YES%&emsp;&emsp;" + "NO%";
                            designDt.Rows.Add(orderId, null, null, "", resultQ, row["Total"]);
                            orderId++;

                            resultQ = row["YES %"].ToString() + "&emsp;&emsp;&emsp;&nbsp;" + row["NO %"].ToString();
                            numbering = row["QuestionNumber"].ToString().Substring(1);
                            designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + ". " + row["Question"], "", resultQ);
                            orderId++;
                        }

                        else if (row["QuestionNumber"].ToString().Contains("QA") && row["QuestionNumber"].ToString() != "QA2")
                        {
                            if (row["QuestionNumber"].ToString() == "QA2A")
                            {
                                designDt.Rows.Add(orderId, null, null, "", "CodedList%", row["Total"]);
                                orderId++;
                            }

                            resultQ = row["ANS %"].ToString(); 

                            if (row["QuestionNumber"].ToString() == "QA3")
                            {
                                numbering = row["QuestionNumber"].ToString().Substring(1);
                                designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + ". " + row["Question"], "", resultQ);
                            }
                            else
                            {
                                numbering = row["QuestionNumber"].ToString().Substring(3);
                                designDt.Rows.Add(orderId, row["QuestionNumber"], numbering.ToLower() + ") " + row["Question"], "", resultQ);
                            }                          
                            orderId++;
                        }

                        else if (row["QuestionNumber"].ToString().Contains("QB1"))
                        {
                            if (row["QuestionNumber"].ToString() == "QB1")
                            {
                                resultQ = "VD%&emsp;" + "SD%&emsp;" + "SS%&emsp;" + "VS%&emsp;" + "NA%";
                                numbering = row["QuestionNumber"].ToString().Substring(1);
                                designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + ". " + row["Question"], "%", resultQ, row["Total"]);
                                orderId++;
                            }
                            else
                            {
                                top3Total = Convert.ToInt32(row["VD %"]) + Convert.ToInt32(row["SD %"]) + Convert.ToInt32(row["SS %"]);
                                resultQ = row["VD %"].ToString() + "&emsp;&emsp;&nbsp;" + row["SD %"].ToString() + "&emsp;&emsp;&nbsp;" + row["SS %"].ToString() + "&emsp;&emsp;&nbsp;" + row["VS %"].ToString() + "&emsp;&emsp;&nbsp;" + row["NA %"].ToString();
                                numbering = row["QuestionNumber"].ToString().Substring(3);
                                designDt.Rows.Add(orderId, row["QuestionNumber"], numbering.ToLower() + ") " + row["Question"], top3Total.ToString(), resultQ);
                                orderId++;
                            }                          
                        }

                        else if (row["QuestionNumber"].ToString() == "QB2" || row["QuestionNumber"].ToString() == "QB3")
                        {
                            if (row["QuestionNumber"].ToString() == "QB2")
                            {
                                resultQ = "YES%&emsp;" + "NO%&emsp;" + "NA%";
                                designDt.Rows.Add(orderId, null, null, "", resultQ, row["Total"]);
                                orderId++;
                            }
                            resultQ = row["YES %"].ToString() + "&emsp;&emsp;&nbsp;" + row["NO %"].ToString() + "&emsp;&emsp;&nbsp;" + row["NA %"].ToString();
                            numbering = row["QuestionNumber"].ToString().Substring(1);
                            designDt.Rows.Add(orderId, "Q" + numbering, numbering + ". " + row["Question"], "", resultQ);
                            orderId++;
                        }

                        else if (row["QuestionNumber"].ToString() == "QB4")
                        {
                            resultQ = "YES%&emsp;&emsp;" + "NO%";
                            designDt.Rows.Add(orderId, null, null, "", resultQ, row["Total"]);
                            orderId++;

                            resultQ = row["YES %"].ToString() + "&emsp;&emsp;&emsp;&nbsp;" + row["NO %"].ToString();
                            numbering = row["QuestionNumber"].ToString().Substring(1);
                            designDt.Rows.Add(orderId, "Q" + numbering, numbering + ". " + row["Question"], "", resultQ);
                            orderId++;
                        }

                        else if (row["QuestionNumber"].ToString() == "QC1A")
                        {
                            foreach (DataRow rq in dtQuestions.Rows)
                            {
                                if (rq["QuestionNumber"].ToString().Contains("QC1A"))
                                {
                                    if (rq["QuestionNumber"].ToString() == "QC1A")
                                    {
                                        numbering = row["QuestionNumber"].ToString().Substring(1);
                                        designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + ". " + rq["Qn_English"].ToString(), "", "CodedList%", row["Total"]);
                                        orderId++;
                                    }
                                    else
                                    {
                                        if (rq["QuestionNumber"].ToString() == "QC1A01")
                                            resultQ = row["QC1A01 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A02")
                                            resultQ = row["QC1A02 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A03")
                                            resultQ = row["QC1A03 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A04")
                                            resultQ = row["QC1A04 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A05")
                                            resultQ = row["QC1A05 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A06")
                                            resultQ = row["QC1A06 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A07")
                                            resultQ = row["QC1A07 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A08")
                                            resultQ = row["QC1A08 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A09")
                                            resultQ = row["QC1A09 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A10")
                                            resultQ = row["QC1A10 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A11")
                                            resultQ = row["QC1A11 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A12")
                                            resultQ = row["QC1A12 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A13")
                                            resultQ = row["QC1A13 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A14")
                                            resultQ = row["QC1A14 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A15")
                                            resultQ = row["QC1A15 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A16")
                                            resultQ = row["QC1A16 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A17")
                                            resultQ = row["QC1A17 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A50")
                                            resultQ = row["QC1A50 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A95")
                                            resultQ = row["QC1A95 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1A98")
                                            resultQ = row["QC1A98 %"].ToString();

                                        if (resultQ != null)
                                        {
                                            numbering = rq["QuestionNumber"].ToString().Substring(4);
                                            designDt.Rows.Add(orderId, rq["QuestionNumber"], numbering.ToLower() + ") " + rq["Qn_English"].ToString(), "", resultQ);
                                            orderId++;
                                        }
                                    }
                                }
                            }
                        }

                        else if (row["QuestionNumber"].ToString() == "QC1B")
                        {
                            foreach (DataRow rq in dtQuestions.Rows)
                            {
                                if (rq["QuestionNumber"].ToString().Contains("QC1B"))
                                {
                                    if (rq["QuestionNumber"].ToString() == "QC1B")
                                    {
                                        numbering = row["QuestionNumber"].ToString().Substring(1);
                                        designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + ". " + rq["Qn_English"].ToString(), "", "CodedList%", row["Total"]);
                                        orderId++;
                                    }
                                    else
                                    {
                                        if (rq["QuestionNumber"].ToString() == "QC1B01")
                                            resultQ = row["QC1B01 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B02")
                                            resultQ = row["QC1B02 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B03")
                                            resultQ = row["QC1B03 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B04")
                                            resultQ = row["QC1B04 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B05")
                                            resultQ = row["QC1B05 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B06")
                                            resultQ = row["QC1B06 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B07")
                                            resultQ = row["QC1B07 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B08")
                                            resultQ = row["QC1B08 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B09")
                                            resultQ = row["QC1B09 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B10")
                                            resultQ = row["QC1B10 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B11")
                                            resultQ = row["QC1B11 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B50")
                                            resultQ = row["QC1B50 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1B96")
                                            resultQ = row["QC1B96 %"].ToString();

                                        if (resultQ != null)
                                        {
                                            numbering = rq["QuestionNumber"].ToString().Substring(4);
                                            designDt.Rows.Add(orderId, rq["QuestionNumber"], numbering.ToLower() + ") " + rq["Qn_English"].ToString(), "", resultQ);
                                            orderId++;
                                        }
                                    }
                                }
                            }
                        }

                        else if (row["QuestionNumber"].ToString() == "QC1C")
                        {
                            foreach (DataRow rq in dtQuestions.Rows)
                            {
                                if (rq["QuestionNumber"].ToString().Contains("QC1C"))
                                {
                                    if (rq["QuestionNumber"].ToString() == "QC1C")
                                    {
                                        numbering = row["QuestionNumber"].ToString().Substring(1);
                                        designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + "." + rq["Qn_English"].ToString(), "", "CodedList%", row["Total"]);
                                        orderId++;
                                    }
                                    else
                                    {
                                        if (rq["QuestionNumber"].ToString() == "QC1C01")
                                            resultQ = row["QC1C01 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C02")
                                            resultQ = row["QC1C02 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C03")
                                            resultQ = row["QC1C03 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C04")
                                            resultQ = row["QC1C04 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C05")
                                            resultQ = row["QC1C05 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C06")
                                            resultQ = row["QC1C06 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C07")
                                            resultQ = row["QC1C07 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C08")
                                            resultQ = row["QC1C08 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C50")
                                            resultQ = row["QC1C50 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC1C96")
                                            resultQ = row["QC1C96 %"].ToString();

                                        if (resultQ != null)
                                        {
                                            numbering = rq["QuestionNumber"].ToString().Substring(4);
                                            designDt.Rows.Add(orderId, rq["QuestionNumber"], numbering.ToLower() + ") " + rq["Qn_English"].ToString(), "", resultQ);
                                            orderId++;
                                        }
                                    }
                                }
                            }
                        }

                        else if (row["QuestionNumber"].ToString() == "QC2")
                        {
                            foreach (DataRow rq in dtQuestions.Rows)
                            {
                                if (rq["QuestionNumber"].ToString().Contains("QC2"))
                                {
                                    if (rq["QuestionNumber"].ToString() == "QC2")
                                    {
                                        numbering = row["QuestionNumber"].ToString().Substring(1);
                                        designDt.Rows.Add(orderId, row["QuestionNumber"], numbering + ". " + rq["Qn_English"].ToString(), "", "CodedList%", row["Total"]);
                                        orderId++;
                                    }
                                    else
                                    {
                                        if (rq["QuestionNumber"].ToString() == "QC201")
                                            resultQ = row["QC201 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC202")
                                            resultQ = row["QC202 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC203")
                                            resultQ = row["QC203 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC204")
                                            resultQ = row["QC204 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC205")
                                            resultQ = row["QC205 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC206")
                                            resultQ = row["QC206 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC207")
                                            resultQ = row["QC207 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC208")
                                            resultQ = row["QC208 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC209")
                                            resultQ = row["QC209 %"].ToString();
                                        else if (rq["QuestionNumber"].ToString() == "QC210")
                                            resultQ = row["QC210 %"].ToString();

                                        if (resultQ != null)
                                        {
                                            numbering = rq["QuestionNumber"].ToString().Substring(3);
                                            designDt.Rows.Add(orderId, rq["QuestionNumber"], numbering.ToLower() + ") " + rq["Qn_English"].ToString(), "", resultQ);
                                            orderId++;
                                        }
                                    }
                                }
                            }
                        }

                        else if (row["QuestionNumber"].ToString() == "QC3")
                        {
                            resultQ = "YES%&emsp;&emsp;" + "NO%";
                            designDt.Rows.Add(orderId, null, null, "", resultQ, row["Total"]);
                            orderId++;

                            resultQ = row["YES %"].ToString() + "&emsp;&emsp;&emsp;&nbsp;" + row["NO %"].ToString();
                            numbering = row["QuestionNumber"].ToString().Substring(1);
                            designDt.Rows.Add(orderId, "Q" + numbering, numbering + ". " + row["Question"], "", resultQ);
                            orderId++;
                        }
                    }                   
                }

                //Csss report
                if (reportType == "CSSS")
                {
                }

                return designDt;
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

    }
}