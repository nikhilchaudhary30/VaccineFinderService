//------------------------------------------------------------------------------
// <self-generated>
//    This code was developed by Nikhil Chaudhary on 19-05-2021.
//    Vaccine Notification Service
//    Manual changes to this file may cause unexpected behavior in your application.
// </self-generated>
//------------------------------------------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Timers;

namespace VaccineFinderService
{
    public class ServiceMethods
    {
        #region Service Helping Components

        #region Service Variables
        private readonly Timer _timer;
        private DateTime dateTime;
        private DateTime dateTime_1 = DateTime.Now.AddHours(Convert.ToDouble(ConfigurationManager.AppSettings["SystemMailTimeInHours"]));
        private static Dictionary<int, string> URLs { get; set; }
        #endregion

        public ServiceMethods()
        {
            _timer = new Timer(Convert.ToInt32(ConfigurationManager.AppSettings["Timer"])) //1800000 - 30Minutes
            { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Timmer Function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            dateTime = DateTime.Now;
            if (dateTime > dateTime_1)
            {
                dateTime_1 = dateTime.AddHours(Convert.ToDouble(ConfigurationManager.AppSettings["SystemMailTimeInHours"]));
                Utility(dateTime_1);
            }

            APIInitiator_3();
        }

        public void Start()
        {
            SendMail(null, "START", null);
            _timer.Start();
        }

        public void Stop()
        {
            SendMail(null, "STOP", null);
            _timer.Stop();
        }

        #endregion

        #region API and Email Components 

        /// <summary>
        /// Method used for sending system updates like Battery, Internet connectivity, CPU/ RAM/ Disk usage etc
        /// </summary>
        /// <param name="nextUpdate"></param>
        public static void Utility(DateTime nextUpdate)
        {
            List<string> list = new List<string>();
            #region Battery Info
            System.Windows.Forms.PowerStatus pwr = System.Windows.Forms.SystemInformation.PowerStatus;
            list.Add("Please find the below system details.");
            list.Add("Battery Charge Status: " + pwr.BatteryChargeStatus.ToString());
            list.Add("Battery Life Percent: " + (int)(pwr.BatteryLifePercent * 100) + "%");
            list.Add("Battery Life Remaining: " + pwr.BatteryLifeRemaining.ToString());
            list.Add("Power Line Status: " + pwr.PowerLineStatus.ToString());
            #endregion

            #region Internet Info
            System.Net.WebClient wc = new System.Net.WebClient();
            DateTime dt1 = DateTime.Now;
            byte[] data = wc.DownloadData("http://google.com");
            DateTime dt2 = DateTime.Now;
            list.Add("Internet Status: " + Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2) + "Kb/Sec");
            list.Add("Host Name: " + Dns.GetHostName().ToString());
            //list.Add("Host Address: " + Dns.GetHostAddresses(Dns.GetHostName()).GetValue(0).ToString());
            #endregion

            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;
            PerformanceCounter diskCounter;

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            ramCounter.NextValue(); ramCounter.NextValue();
            cpuCounter.NextValue(); cpuCounter.NextValue();
            diskCounter.NextValue(); diskCounter.NextValue();
            list.Add("CPU Usage: " + cpuCounter.NextValue() + "%");
            list.Add("RAM Usage: " + ramCounter.NextValue() + " MB");
            list.Add("DISK Usage: " + diskCounter.NextValue() + "%");
            list.Add("***** Service is running *****");
            list.Add("Next update at: " + nextUpdate.ToString());
            SendMail(list.ToArray(), "System");
        }

        //public static void APIInitiator()
        //{
        //    List<Root> root = new List<Root>();
        //    string date = DateTime.UtcNow.AddDays(19).ToString("dd-MM-yyyy");
        //    string urlParameter = "api/v2/appointment/sessions/public/calendarByDistrict?district_id=108&date=" + date + "";
        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri("http://cdn-api.co-vin.in/");
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        //    try
        //    {
        //        var task = client.GetAsync(urlParameter);
        //        task.Wait();
        //        var response = task.Result;
        //        bool sent = false;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var readTask = response.Content.ReadAsStringAsync();
        //            readTask.Wait();
        //            var result = JsonConvert.DeserializeObject<Root>(readTask.Result);
        //            root.Add(result);
        //        }

        //        foreach (var r in root)
        //        {
        //            foreach (var center in r.centers)
        //            {
        //                foreach (var sess in center.sessions)
        //                {
        //                    if (sess.min_age_limit == 18 && sess.available_capacity > 0 && sess.available_capacity_dose1 > 0)
        //                    {
        //                        List<string> list = new List<string>();
        //                        list.Add("Please find below details for Date: " + date);
        //                        list.Add("Vaccine: " + sess.vaccine);
        //                        list.Add("Center Name: " + center.name);
        //                        list.Add("Center Address: " + center.address);
        //                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
        //                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
        //                        list.Add("Available Capacity for Dose 1: " + sess.available_capacity_dose1.ToString());
        //                        sent = SendMail(list.ToArray());
        //                        //break;
        //                    }
        //                }
        //                //if (sent)
        //                //    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //public static void APIInitiator_2()
        //{
        //    string date = DateTime.UtcNow.ToString("dd-MM-yyyy");
        //    string urlParameter = "api/v2/appointment/sessions/public/calendarByDistrict?district_id=108&date=" + date + "";
        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri("http://cdn-api.co-vin.in/");
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        //    try
        //    {
        //        var task = client.GetAsync(urlParameter);
        //        task.Wait();
        //        var response = task.Result;
        //        bool sent = false;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var readTask = response.Content.ReadAsStringAsync();
        //            readTask.Wait();
        //            var result = JsonConvert.DeserializeObject<Root>(readTask.Result);
        //            foreach (var center in result.centers)
        //            {
        //                foreach (var sess in center.sessions)
        //                {
        //                    if (sess.min_age_limit == 18 && sess.available_capacity > 0 && sess.available_capacity_dose1 > 0)
        //                    {
        //                        List<string> list = new List<string>();
        //                        list.Add("Please find below details for Date: " + date);
        //                        list.Add("Vaccine: " + sess.vaccine);
        //                        list.Add("Center Name: " + center.name);
        //                        list.Add("Center Address: " + center.address);
        //                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
        //                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
        //                        list.Add("Available Capacity for Dose 1: " + sess.available_capacity_dose1.ToString());
        //                        sent = SendMail(list.ToArray());
        //                        //break;
        //                    }
        //                }
        //                //if (sent)
        //                //    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        /// <summary>
        /// Method used for initiating Public API 
        /// </summary>
        public static void APIInitiator_3()
        {
            string _DistrictIDs = Convert.ToString(ConfigurationManager.AppSettings["DistrictID"]);
            if (!string.IsNullOrWhiteSpace(_DistrictIDs))
            {
                LogWrite("\r\n\n\n\n****************** Log Started ******************");
                int _APIHitCounter = 1;
                string urlParameter = string.Empty;
                bool sent = false;
                Root result = new Root();
                List<Root> root = new List<Root>();
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://cdn-api.co-vin.in/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                URLs = new Dictionary<int, string>();
                foreach (var id in _DistrictIDs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    URLs.Add(Convert.ToInt32(id), "api/v2/appointment/sessions/public/calendarByDistrict?district_id=" + id + "&date=#DateHere");
                }
                try
                {
                    foreach (var URL in URLs)
                    {
                        string date = DateTime.Now.ToString("dd-MM-yyyy");
                        string _DateFor108 = DateTime.Now.AddDays(Convert.ToDouble(ConfigurationManager.AppSettings["DateFor108"])).ToString("dd-MM-yyyy");
                        for (int a = 0; a < 5; a--)
                        {
                            if (URL.Key == 108)
                                urlParameter = URL.Value.Replace("#DateHere", _DateFor108);
                            else
                                urlParameter = URL.Value.Replace("#DateHere", date);
                            var task = client.GetAsync(urlParameter);
                            task.Wait();
                            var response = task.Result;
                            if (response.IsSuccessStatusCode)
                            {
                                LogWrite("Number of API Hit for URL: [" + urlParameter + "] is " + Convert.ToString(_APIHitCounter++));
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                result = JsonConvert.DeserializeObject<Root>(readTask.Result);

                                if (result.centers.Count > 0)
                                {
                                    root.Add(result);
                                    DateTime date1 = Convert.ToDateTime(date).AddDays(1);
                                    date = date1.ToString("dd-MM-yyyy");
                                    if (URL.Key == 108)
                                    {
                                        DateTime _DateFor108_2 = Convert.ToDateTime(_DateFor108).AddDays(1);
                                        _DateFor108 = _DateFor108_2.ToString("dd-MM-yyyy");
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                LogWrite("API Error StatusCode: " + response.StatusCode.ToString());
                                LogWrite("API Error RequestMessage: " + response.RequestMessage.ToString());
                                break;
                            }
                        }
                    }

                    #region Final List Variables
                    List<List<string>> finalList = new List<List<string>>();
                    List<List<string>> ritvikFinalList = new List<List<string>>();
                    List<List<string>> agraFinalList = new List<List<string>>();
                    List<List<string>> delhiFinalList = new List<List<string>>();
                    List<List<string>> karnalFinalList = new List<List<string>>();
                    List<List<string>> faridabadFinalList = new List<List<string>>();
                    List<List<string>> sonipatFinalList = new List<List<string>>();
                    #endregion

                    #region Data Filtering 
                    foreach (var r in root)
                    {
                        foreach (var center in r.centers)
                        {
                            #region Chandigarh & Mohali
                            if (center.block_name.ToLower() == "mohali" || center.block_name.ToLower() == "chandigarh")
                            {
                                foreach (var sess in center.sessions)
                                {
                                    if (sess.min_age_limit == 18 && sess.available_capacity_dose1 > 5)
                                    {
                                        List<string> list = new List<string>();
                                        list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                        list.Add("Vaccine: " + sess.vaccine);
                                        list.Add("Center Name: " + center.name);
                                        list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                        list.Add("Available Capacity of Dose 1 for " + sess.min_age_limit + "+: " + Convert.ToString(sess.available_capacity_dose1));
                                        finalList.Add(list);
                                    }
                                }
                            }
                            foreach (var sess in center.sessions)
                            {
                                if (sess.min_age_limit == 18 && sess.available_capacity_dose2 > 5 && sess.vaccine.ToUpper() == "COVAXIN")
                                {
                                    List<string> list = new List<string>();
                                    list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                    list.Add("Vaccine: " + sess.vaccine);
                                    list.Add("Center Name: " + center.name);
                                    list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                    list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                    list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                    list.Add("Available Capacity of Dose 2 for " + sess.min_age_limit + "+: " + sess.available_capacity_dose2.ToString());
                                    ritvikFinalList.Add(list);
                                }
                            }
                            #endregion

                            #region Agra
                            if (center.district_name.ToLower() == "agra")
                            {
                                foreach (var sess in center.sessions)
                                {
                                    if (sess.min_age_limit == 18 && sess.available_capacity_dose1 > 5)
                                    {
                                        List<string> list = new List<string>();
                                        list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                        list.Add("Vaccine: " + sess.vaccine);
                                        list.Add("Center Name: " + center.name);
                                        list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                        list.Add("Available Capacity of Dose 1 for " + sess.min_age_limit + "+: " + Convert.ToString(sess.available_capacity_dose1));
                                        agraFinalList.Add(list);
                                    }
                                }
                            }
                            #endregion

                            #region Haryana: Karnal, Faridabad, Sonipat
                            if (center.district_name.ToLower() == "karnal")
                            {
                                foreach (var sess in center.sessions)
                                {
                                    if (sess.min_age_limit == 18 && sess.available_capacity_dose1 > 5)
                                    {
                                        List<string> list = new List<string>();
                                        list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                        list.Add("Vaccine: " + sess.vaccine);
                                        list.Add("Center Name: " + center.name);
                                        list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                        list.Add("Available Capacity of Dose 1 for " + sess.min_age_limit + "+: " + Convert.ToString(sess.available_capacity_dose1));
                                        karnalFinalList.Add(list);
                                    }
                                }
                            }
                            if (center.district_name.ToLower() == "faridabad")
                            {
                                foreach (var sess in center.sessions)
                                {
                                    if (sess.min_age_limit == 18 && sess.available_capacity_dose1 > 5)
                                    {
                                        List<string> list = new List<string>();
                                        list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                        list.Add("Vaccine: " + sess.vaccine);
                                        list.Add("Center Name: " + center.name);
                                        list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                        list.Add("Available Capacity of Dose 1 for " + sess.min_age_limit + "+: " + Convert.ToString(sess.available_capacity_dose1));
                                        faridabadFinalList.Add(list);
                                    }
                                }
                            }
                            if (center.district_name.ToLower() == "sonipat")
                            {
                                foreach (var sess in center.sessions)
                                {
                                    if (sess.min_age_limit == 18 && sess.available_capacity_dose1 > 5)
                                    {
                                        List<string> list = new List<string>();
                                        list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                        list.Add("Vaccine: " + sess.vaccine);
                                        list.Add("Center Name: " + center.name);
                                        list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                        list.Add("Available Capacity of Dose 1 for " + sess.min_age_limit + "+: " + Convert.ToString(sess.available_capacity_dose1));
                                        sonipatFinalList.Add(list);
                                    }
                                }
                            }
                            #endregion

                            #region Delhi
                            if (center.district_name.ToLower() == "delhi")
                            {
                                foreach (var sess in center.sessions)
                                {
                                    if (sess.min_age_limit == 18 && sess.available_capacity_dose1 > 5)
                                    {
                                        List<string> list = new List<string>();
                                        list.Add("Please find below details for " + center.district_name + " for Date: " + sess.date);
                                        list.Add("Vaccine: " + sess.vaccine);
                                        list.Add("Center Name: " + center.name);
                                        list.Add("Center Address: " + center.address + ", Pincode: " + center.pincode);
                                        list.Add("Slots: " + String.Join(" | ", sess.slots.ToArray()));
                                        list.Add("Available Capacity: " + sess.available_capacity.ToString());
                                        list.Add("Available Capacity of Dose 1 for " + sess.min_age_limit + "+: " + Convert.ToString(sess.available_capacity_dose1));
                                        delhiFinalList.Add(list);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region Email Trigger
                    if (finalList.Count > 0)
                    {
                        sent = SendMail(null, null, finalList.ToList());
                    }
                    if (ritvikFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Ritvik", ritvikFinalList.ToList());
                    }
                    if (agraFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Agra", agraFinalList.ToList());
                    }
                    if (karnalFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Karnal", karnalFinalList.ToList());
                    }
                    if (faridabadFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Faridabad", faridabadFinalList.ToList());
                    }
                    if (sonipatFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Sonipat", sonipatFinalList.ToList());
                    }
                    if (delhiFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Delhi", delhiFinalList.ToList());
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    #region Exception
                    LogWrite("APIInitiator_3 Exception Message: " + ex.Message);
                    LogWrite("APIInitiator_3 Exception StackTrace: " + ex.StackTrace);
                    List<string> list = new List<string>();
                    list.Add("Message:  " + ex.Message.ToString());
                    list.Add("StackTrace:  " + ex.StackTrace.ToString());
                    list.Add("InnerException.Message:  " + ex.InnerException.Message.ToString());
                    list.Add("InnerException.StackTrace:  " + ex.InnerException.StackTrace.ToString());
                    SendMail(list.ToArray(), "Exception", null);
                    #endregion
                }
            }
        }

        /// <summary>
        /// Method used for sending emails 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="emailType"></param>
        /// <param name="listString"></param>
        /// <returns></returns>
        public static bool SendMail(String[] str = null, string emailType = "", List<List<string>> listString = null)
        {
            bool isSendSuccess = false;
            try
            {
                #region Email: From, To, BCC, Username, Password etc
                string fromaddr = Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]);
                string _ChandigarhMohaliEmailTo = Convert.ToString(ConfigurationManager.AppSettings["ChandigarhMohaliEmailTo"]);
                string password = Convert.ToString(ConfigurationManager.AppSettings["EmailPassword"]);
                string _SystemEmailTo = Convert.ToString(ConfigurationManager.AppSettings["SystemEmailTo"]);
                string _RitvikEmailTo = Convert.ToString(ConfigurationManager.AppSettings["RitvikEmailTo"]);
                string _AgraEmailTo = Convert.ToString(ConfigurationManager.AppSettings["AgraEmailTo"]);
                string _DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["DelhiEmailTo"]);
                string _KarnalEmailTo = Convert.ToString(ConfigurationManager.AppSettings["KarnalEmailTo"]);
                string _FaridabadEmailTo = Convert.ToString(ConfigurationManager.AppSettings["FaridabadEmailTo"]);
                string _SonipatEmailTo = Convert.ToString(ConfigurationManager.AppSettings["SonipatEmailTo"]);
                #endregion
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(fromaddr);
                #region Email Type: System, Ritvik, Agra, Delhi, Karnal, Faridabad, Sonipat, STOP, START, Exception
                if (emailType == "System")
                {
                    msg.Subject = "System alert at: " + DateTime.Now.ToString();
                    msg.Body = StringBuilder(str);
                    foreach (var address in _SystemEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* System Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Ritvik")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _RitvikEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Agra")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _AgraEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Karnal")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _KarnalEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Faridabad")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _FaridabadEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Sonipat")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _SonipatEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "STOP")
                {
                    msg.Subject = "System alert at: " + DateTime.Now.ToString();
                    msg.Body = "***** Service has been stopped *****";
                    foreach (var address in _SystemEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* System Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "START")
                {
                    msg.Subject = "System alert at: " + DateTime.Now.ToString();
                    msg.Body = "***** Service has been started *****";
                    foreach (var address in _SystemEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* System Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Exception")
                {
                    msg.Subject = "Exception alert at: " + DateTime.Now.ToString();
                    msg.Body = StringBuilder(str); ;
                    foreach (var address in _SystemEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Exception Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _ChandigarhMohaliEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                #endregion
                //msg.To.Add(address);
                //msg.IsBodyHtml = true;
                //msg.To.Add(new MailAddress(toaddr));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential(fromaddr, password);
                smtp.Credentials = nc;
                if (msg.Bcc.Count > 0)
                    smtp.Send(msg);
                isSendSuccess = true;
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                #region Exception
                LogWrite("Email Exception Message: " + ex.Message);
                LogWrite("Email Exception StackTrace: " + ex.StackTrace);
                isSendSuccess = false;
                #endregion
            }

            return isSendSuccess;
        }

        #region API Models
        public class Session
        {
            public string session_id { get; set; }
            public string date { get; set; }
            public int available_capacity { get; set; }
            public int min_age_limit { get; set; }
            public string vaccine { get; set; }
            public List<string> slots { get; set; }
            public int available_capacity_dose1 { get; set; }
            public int available_capacity_dose2 { get; set; }
        }

        public class Center
        {
            public int center_id { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public string state_name { get; set; }
            public string district_name { get; set; }
            public string block_name { get; set; }
            public int pincode { get; set; }
            public int lat { get; set; }
            public int @long { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string fee_type { get; set; }
            public List<Session> sessions { get; set; }
        }

        public class Root
        {
            public List<Center> centers { get; set; }
        }
        #endregion

        #region String Builders
        public static string emailStringBuilder(String[] str = null, List<List<string>> listString = null)
        {
            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine();
            errMsg.AppendLine("Hi All,");
            errMsg.AppendLine();
            foreach (var l in listString)
            {
                foreach (var i in l)
                {
                    errMsg.AppendLine(i);
                }
                errMsg.AppendLine();
                errMsg.AppendLine();
            }
            errMsg.AppendLine("Regards,");
            errMsg.AppendLine("Service Bot");
            return errMsg.ToString();
        }

        public static string StringBuilder(String[] str)
        {
            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine();
            errMsg.AppendLine("Hi All,");
            errMsg.AppendLine();
            foreach (var i in str)
            {
                errMsg.AppendLine(i);
            }
            errMsg.AppendLine();
            errMsg.AppendLine("Regards,");
            errMsg.AppendLine("Service Bot");
            return errMsg.ToString();
        }
        #endregion

        #endregion

        #region Log Writing
        public static void LogWrite(string logMessage)
        {
            if (Convert.ToInt32(ConfigurationManager.AppSettings["isLogging"]) == 1)
            {
                string _Path = Convert.ToString(ConfigurationManager.AppSettings["LogPath"]);
                try
                {
                    if (!string.IsNullOrWhiteSpace(_Path))
                    {
                        using (StreamWriter w = File.AppendText(_Path + "\\" + "VaccineNotificationService_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
                        {
                            Log(logMessage, w);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\n" + logMessage + " Date: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}