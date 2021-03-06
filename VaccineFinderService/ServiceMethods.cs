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
        private static Dictionary<string, string> Emails { get; set; }
        private static Dictionary<string, List<List<string>>> DynamicList { get; set; }

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

            //APIInitiator_3();
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
                    List<List<string>> karnalFinalList = new List<List<string>>();
                    List<List<string>> faridabadFinalList = new List<List<string>>();
                    List<List<string>> sonipatFinalList = new List<List<string>>();
                    List<List<string>> hyderabadFinalList = new List<List<string>>();
                    List<List<string>> puneFinalList = new List<List<string>>();
                    List<List<string>> mumbaiFinalList = new List<List<string>>();

                    List<List<string>> delhi140FinalList = new List<List<string>>();
                    List<List<string>> delhi141FinalList = new List<List<string>>();
                    List<List<string>> delhi142FinalList = new List<List<string>>();
                    List<List<string>> delhi143FinalList = new List<List<string>>();
                    List<List<string>> delhi144FinalList = new List<List<string>>();
                    List<List<string>> delhi145FinalList = new List<List<string>>();
                    List<List<string>> delhi146FinalList = new List<List<string>>();
                    List<List<string>> delhi147FinalList = new List<List<string>>();
                    List<List<string>> delhi148FinalList = new List<List<string>>();
                    List<List<string>> delhi149FinalList = new List<List<string>>();
                    List<List<string>> delhi150FinalList = new List<List<string>>();

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

                            #region 140: New Delhi | 141: Central Delhi | 142: West Delhi | 143: North West Delhi | 144: South East Delhi | 145: East Delhi | 146: North Delhi | 147: North East Delhi | 148: Shahdara | 149: South Delhi | 150: South West Delhi
                            if (center.district_name.ToLower() == "new delhi")
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
                                        delhi140FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "central delhi")
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
                                        delhi141FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "west delhi")
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
                                        delhi142FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "north west delhi")
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
                                        delhi143FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "south east delhi")
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
                                        delhi144FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "east delhi")
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
                                        delhi145FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "north delhi")
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
                                        delhi146FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "north east delhi")
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
                                        delhi147FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "shahdara")
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
                                        delhi148FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "south delhi")
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
                                        delhi149FinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "south west delhi")
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
                                        delhi150FinalList.Add(list);
                                    }
                                }
                            }
                            #endregion

                            #region Hyderabad
                            if (center.district_name.ToLower() == "hyderabad")
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
                                        hyderabadFinalList.Add(list);
                                    }
                                }
                            }
                            #endregion

                            #region Maharashtra
                            if (center.district_name.ToLower() == "pune")
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
                                        puneFinalList.Add(list);
                                    }
                                }
                            }

                            if (center.district_name.ToLower() == "mumbai")
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
                                        mumbaiFinalList.Add(list);
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
                    if (hyderabadFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Hyderabad", hyderabadFinalList.ToList());
                    }
                    if (puneFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Pune", puneFinalList.ToList());
                    }
                    if (mumbaiFinalList.Count > 0)
                    {
                        sent = SendMail(null, "Mumbai", mumbaiFinalList.ToList());
                    }

                    if (delhi140FinalList.Count > 0)
                    {
                        sent = SendMail(null, "New Delhi", delhi140FinalList.ToList());
                    }
                    if (delhi141FinalList.Count > 0)
                    {
                        sent = SendMail(null, "Central Delhi", delhi141FinalList.ToList());
                    }
                    if (delhi142FinalList.Count > 0)
                    {
                        sent = SendMail(null, "West Delhi", delhi142FinalList.ToList());
                    }
                    if (delhi143FinalList.Count > 0)
                    {
                        sent = SendMail(null, "North West Delhi", delhi143FinalList.ToList());
                    }
                    if (delhi144FinalList.Count > 0)
                    {
                        sent = SendMail(null, "South East Delhi", delhi144FinalList.ToList());
                    }
                    if (delhi145FinalList.Count > 0)
                    {
                        sent = SendMail(null, "East Delhi", delhi145FinalList.ToList());
                    }
                    if (delhi146FinalList.Count > 0)
                    {
                        sent = SendMail(null, "North Delhi", delhi146FinalList.ToList());
                    }
                    if (delhi147FinalList.Count > 0)
                    {
                        sent = SendMail(null, "North East Delhi", delhi147FinalList.ToList());
                    }
                    if (delhi148FinalList.Count > 0)
                    {
                        sent = SendMail(null, "Shahdara", delhi148FinalList.ToList());
                    }
                    if (delhi149FinalList.Count > 0)
                    {
                        sent = SendMail(null, "South Delhi", delhi149FinalList.ToList());
                    }
                    if (delhi150FinalList.Count > 0)
                    {
                        sent = SendMail(null, "South West Delhi", delhi150FinalList.ToList());
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

        public static void APIInitiator_4()
        {
            string _OneSetting = Convert.ToString(ConfigurationManager.AppSettings["OneSetting"]);
            if (!string.IsNullOrWhiteSpace(_OneSetting))
            {
                LogWrite("\r\n\n\n\n****************** Log Started ******************");
                int _APIHitCounter = 1;
                string urlParameter = string.Empty;
                Root result = new Root();
                List<Root> root = new List<Root>();
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://cdn-api.co-vin.in/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                URLs = new Dictionary<int, string>();
                Emails = new Dictionary<string, string>();
                foreach (var i in _OneSetting.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var j = i.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    URLs.Add(Convert.ToInt32(j[0]), "api/v2/appointment/sessions/public/calendarByDistrict?district_id=" + j[0] + "&date=#DateHere");
                    Emails.Add(j[1], j[2]);
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
                    DynamicList = new Dictionary<string, List<List<string>>>();
                    #region Data Filtering
                    foreach (var i in Emails)
                    {
                        List<List<string>> finalList = new List<List<string>>();
                        foreach (var r in root)
                        {
                            foreach (var center in r.centers)
                            {
                                if (center.district_name.ToLower() == i.Key.ToLower())
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
                            }
                        }
                        DynamicList.Add(i.Key, finalList);
                    }

                    #endregion
                    foreach (var i in Emails)
                    {
                        foreach (var j in DynamicList)
                        {
                            if (i.Key.ToLower() == j.Key.ToLower())
                                SendMail_1(i.Value, j.Key, j.Value.ToList());
                        }
                    }
                    DynamicList = new Dictionary<string, List<List<string>>>();
                }
                catch (Exception ex)
                {
                    #region Exception
                    LogWrite("APIInitiator_4 Exception Message: " + ex.Message);
                    LogWrite("APIInitiator_4 Exception StackTrace: " + ex.StackTrace);
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

        public static bool SendMail_1(string emailTo = "", string emailType = "", List<List<string>> listString = null)
        {
            bool isSendSuccess = false;
            string fromaddr = Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]);
            string password = Convert.ToString(ConfigurationManager.AppSettings["EmailPassword"]);
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(fromaddr);
            msg.Subject = "New vaccine alert at: " + DateTime.Now.ToString();
            msg.Body = emailStringBuilder(null, listString);
            foreach (var address in emailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                LogWrite("******************* Email sent to: " + address);
                msg.Bcc.Add(address);
            }
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
            return isSendSuccess;
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
                string _KarnalEmailTo = Convert.ToString(ConfigurationManager.AppSettings["KarnalEmailTo"]);
                string _FaridabadEmailTo = Convert.ToString(ConfigurationManager.AppSettings["FaridabadEmailTo"]);
                string _SonipatEmailTo = Convert.ToString(ConfigurationManager.AppSettings["SonipatEmailTo"]);
                string _HyderabadEmailTo = Convert.ToString(ConfigurationManager.AppSettings["HyderabadEmailTo"]);
                string _PuneEmailTo = Convert.ToString(ConfigurationManager.AppSettings["PuneEmailTo"]);
                string _MumbaiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["MumbaiEmailTo"]);

                string _140DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["140DelhiEmailTo"]);
                string _141DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["141DelhiEmailTo"]);
                string _142DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["142DelhiEmailTo"]);
                string _143DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["143DelhiEmailTo"]);
                string _144DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["144DelhiEmailTo"]);
                string _145DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["145DelhiEmailTo"]);
                string _146DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["146DelhiEmailTo"]);
                string _147DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["147DelhiEmailTo"]);
                string _148DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["148DelhiEmailTo"]);
                string _149DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["149DelhiEmailTo"]);
                string _150DelhiEmailTo = Convert.ToString(ConfigurationManager.AppSettings["150DelhiEmailTo"]);

                #endregion
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(fromaddr);
                #region Email Type: System, Ritvik, Agra, Delhi, Karnal, Faridabad, Sonipat, Hyderabad, Pune, Mumbai, STOP, START, Exception
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
                else if (emailType == "New Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _140DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Central Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _141DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "West Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _142DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "North West Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _143DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "South East Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _144DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "East Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _145DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "North Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _146DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "North East Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _147DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Shahdara")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _148DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "South Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _149DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "South West Delhi")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _150DelhiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
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
                else if (emailType == "Hyderabad")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _HyderabadEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Pune")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _PuneEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        LogWrite("******************* Email sent to: " + address);
                        msg.Bcc.Add(address);
                    }
                    emailType = string.Empty;
                }
                else if (emailType == "Mumbai")
                {
                    msg.Subject = "New Vaccine alert at: " + System.DateTime.Now.ToString();
                    msg.Body = emailStringBuilder(null, listString);
                    foreach (var address in _MumbaiEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
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
            errMsg.AppendLine("Hurry book your slot at: https://selfregistration.cowin.gov.in/");
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
