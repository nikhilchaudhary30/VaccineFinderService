//------------------------------------------------------------------------------
// <self-generated>
//    This code was developed by Nikhil Chaudhary on 19-05-2021.
//    Vaccine Notification Service
//    Manual changes to this file may cause unexpected behavior in your application.
// </self-generated>
//------------------------------------------------------------------------------
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text;
using System.Threading;
using System.Timers;
using Telegram.Bot;

namespace TelegramBOT
{
    public class ServiceMethods
    {
        #region Service Helping Components

        #region Service Variables
        private static Dictionary<int, string> URLs { get; set; }
        static TelegramBotClient telegramBotClient = new TelegramBotClient(Convert.ToString(ConfigurationManager.AppSettings["TelegramBotToken"]));
        #endregion

        public ServiceMethods()
        {
            TelegramBotInitiator();
        }

        /// <summary>
        /// Timmer Function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TelegramBotInitiator();
        }

        public void Start()
        {
            TelegramBotInitiator();
        }

        public void Stop()
        {
        }

        #endregion

        #region API and Email Components 

        public static List<string> Utility()
        {
            List<string> list = new List<string>();

            #region Battery Info
            //System.Windows.Forms.PowerStatus pwr = System.Windows.Forms.SystemInformation.PowerStatus;
            //list.Add("Please find the below system details.");
            //list.Add("Battery Charge Status: " + pwr.BatteryChargeStatus.ToString());
            //list.Add("Battery Life Percent: " + (int)(pwr.BatteryLifePercent * 100) + "%");
            //list.Add("Battery Life Remaining: " + pwr.BatteryLifeRemaining.ToString());
            //list.Add("Power Line Status: " + pwr.PowerLineStatus.ToString());
            #endregion


            #region Internet Info
            System.Net.WebClient wc = new System.Net.WebClient();
            DateTime dt1 = DateTime.Now;
            byte[] data = wc.DownloadData("http://google.com");
            DateTime dt2 = DateTime.Now;
            list.Add("Internet Status: " + Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2) + " Kb/Sec");
            list.Add("Host Name: " + Dns.GetHostName().ToString());
            #endregion
            #region Memory Info
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
            #endregion

            list.Add("***** Service is running *****");
            return list;
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
            if (listString != null)
            {
                foreach (var l in listString)
                {
                    foreach (var i in l)
                    {
                        errMsg.AppendLine(i);
                    }
                    errMsg.AppendLine();
                    errMsg.AppendLine();
                }
            }
            else if (str != null)
            {
                foreach (var l in str)
                {
                    errMsg.AppendLine(l);
                }
                errMsg.AppendLine("");
            }
            errMsg.AppendLine("Hurry book your slot at: https://selfregistration.cowin.gov.in/");
            errMsg.AppendLine("");
            errMsg.AppendLine("Regards,");
            errMsg.AppendLine("Service Bot");
            return errMsg.ToString();
        }

        public static string StringBuilder(String[] str)
        {
            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine();
            errMsg.AppendLine();
            foreach (var i in str)
            {
                errMsg.AppendLine(i);
            }
            errMsg.AppendLine();
            return errMsg.ToString();
        }

        public static string exceptionStringBuilder(String[] str = null)
        {
            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine();
            errMsg.AppendLine("Exception occured at: " + DateTime.Now);
            errMsg.AppendLine();
            foreach (var l in str)
            {
                errMsg.AppendLine(l);
                errMsg.AppendLine();
            }
            errMsg.AppendLine();
            return errMsg.ToString();
        }

        public static string gameStringBuilder(String[] str = null, List<List<string>> listString = null)
        {
            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine();
            errMsg.AppendLine("Hi Gamer,");
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
            errMsg.AppendLine("EpicGame Bot");
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
                        using (StreamWriter w = File.AppendText(_Path + "\\" + "TelegramBot_" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt"))
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

        public static void TelegramBotInitiator()
        {
            telegramBotClient.StartReceiving();
            telegramBotClient.OnMessage += TelegramBotClient_OnMessage;
            telegramBotClient.OnMessageEdited += TelegramBotClient_OnMessage;
            telegramBotClient.OnInlineQuery += TelegramBotClient_OnInlineQuery;
            Console.ReadLine();
        }

        private static void TelegramBotClient_OnInlineQuery(object sender, Telegram.Bot.Args.InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async static void TelegramBotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //SystemSounds.Beep.Play();
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                Helpercommands(sender, e);
            }
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "This phase is under development...");
            }
        }

        public async static void Helpercommands(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            e.Message.Text.Trim();
            LogWrite("\r\n\n\n\n****************** Log Started ******************");
            LogWrite("Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username + ", Message: " + e.Message.Text + ",");
            if (e.Message.Text.ToLower().Contains("help"))
            {
                List<string> help = new List<string>();
                help.Add("1. For vaccine notifications please type below commands: ");
                help.Add("      > Chandigarh");
                help.Add("      > Mohali");
                help.Add("      > Karnal");
                help.Add("      > Agra");
                help.Add("      > Faridabad");
                help.Add("      > Sonipat");
                help.Add("      > Hyderabad");
                help.Add("      > Mumbai");
                help.Add("      > Pune");
                help.Add("      > New Delhi");
                help.Add("      > Central Delhi");
                help.Add("      > West Delhi");
                help.Add("      > North West Delhi");
                help.Add("      > South East Delhi");
                help.Add("      > East Delhi");
                help.Add("      > North Delhi");
                help.Add("      > North East Delhi");
                help.Add("      > Shahdara");
                help.Add("      > South Delhi");
                help.Add("      > South West Delhi");
                help.Add(" ");
                help.Add("2. For system notifications please type 'Info'.");
                help.Add(" ");
                help.Add("3. For free game noifications please type 'Game'.");
                help.Add(" ");
                help.Add("4. For date noifications please type 'Date'.");
                help.Add(" ");
                help.Add("5. For weather noifications please type 'Weather place_name' or 'W place_name', E.g: Weather Chandigarh or W Delhi");
                help.Add(" ");
                help.Add("6. For corona report noifications please type below commands: ");
                help.Add("      > For all over type: 'Corona All' or 'C All'");
                help.Add("      > For state wise type: 'Corona State_Name' or 'C State_Name' like Corona Chandigarh");

                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, StringBuilder(help.ToArray()));
            }
            else if (e.Message.Text.ToLower() == "chandigarh")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "108", e);
            }
            else if (e.Message.Text.ToLower() == "mohali")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "496", e);
            }
            else if (e.Message.Text.ToLower() == "karnal")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "203", e);
            }
            else if (e.Message.Text.ToLower() == "agra")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "622", e);
            }
            else if (e.Message.Text.ToLower() == "faridabad")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "199", e);
            }
            else if (e.Message.Text.ToLower() == "sonipat")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "198", e);
            }
            else if (e.Message.Text.ToLower() == "hyderabad")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "581", e);
            }
            else if (e.Message.Text.ToLower() == "mumbai")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "395", e);
            }
            else if (e.Message.Text.ToLower() == "pune")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "363", e);
            }
            else if (e.Message.Text.ToLower() == "new delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "140", e);
            }
            else if (e.Message.Text.ToLower() == "central delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "141", e);
            }
            else if (e.Message.Text.ToLower() == "west delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "142", e);
            }
            else if (e.Message.Text.ToLower() == "north west delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "143", e);
            }
            else if (e.Message.Text.ToLower() == "south east delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "144", e);
            }
            else if (e.Message.Text.ToLower() == "east delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "145", e);
            }
            else if (e.Message.Text.ToLower() == "north delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "146", e);
            }
            else if (e.Message.Text.ToLower() == "north east delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "147", e);
            }
            else if (e.Message.Text.ToLower() == "shahdara")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "148", e);
            }
            else if (e.Message.Text.ToLower() == "south delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "149", e);
            }
            else if (e.Message.Text.ToLower() == "south west delhi")
            {
                TelegramBotAPIInitiator(e.Message.Chat.Id, "150", e);
            }
            else if (e.Message.Text.ToLower() == "hi" || e.Message.Text.ToLower() == "hello" || e.Message.Text.ToLower() == "hey" || e.Message.Text.ToLower() == "hola" || e.Message.Text.ToLower() == "whatsup" || e.Message.Text.ToLower() == "wassup" || e.Message.Text.ToLower() == "ssup")
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, e.Message.Text.ToLower() + " " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName);
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "you can type 'Help' for Commands!");
            }
            else if (e.Message.Text.ToLower().Contains("/start"))
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Welcome " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", to Notification Service");
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please type 'Help' for Commands!");
            }
            else if (e.Message.Text.ToLower().Contains("info"))
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, StringBuilder(Utility().ToArray()));
            }
            else if (e.Message.Text.ToLower() == "game")
            {
                TelegramBotGameAPIInitiator(e.Message.Chat.Id, "", e);
            }
            else if (e.Message.Text.ToLower().Contains("date") || e.Message.Text.ToLower().Contains("datetime") || e.Message.Text.ToLower().Contains("time") || e.Message.Text.ToLower().Contains("clock"))
            {
                List<string> list = new List<string>();
                var britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var aus = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
                var can = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
                var uae = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
                var ams = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                var Los_Angeles = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                list.Add("World Date Time");
                list.Add("");
                list.Add("UTC : " + DateTime.UtcNow.ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("India: " + DateTime.Now.ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("UK: " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone).ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("Melbourne: " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, aus).ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("Ontario: " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, can).ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("Dubai: " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, uae).ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("Amsterdam/ Berlin: " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, ams).ToString("dd-MMMM-yyyy  hh:mm tt"));
                list.Add("");
                list.Add("Los Angeles: " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, Los_Angeles).ToString("dd-MMMM-yyyy  hh:mm tt"));
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, StringBuilder(list.ToArray()));
            }
            else if (e.Message.Text.ToLower().Contains("weather") || e.Message.Text.ToLower().Contains("w"))
            {
                var txt = e.Message.Text.Split(" ");
                if (txt.Length > 1)
                {
                    TelegramBotWeatherAPIInitiator(e.Message.Chat.Id, txt[1], e);
                }
                else
                {
                    await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please use keywords like: 'Weather Chandigarh' or 'W Chandigarh'");
                }
            }
            else if (e.Message.Text.ToLower().Contains("covid") || e.Message.Text.ToLower().Contains("c"))
            {
                var txt = e.Message.Text.Split(" ");
                if (txt.Length > 1)
                {
                    if (txt[1].ToLower() == "all")
                        TelegramBotCovidInfoAPIInitiator(e.Message.Chat.Id, "", e, "All");
                    else
                        TelegramBotCovidInfoAPIInitiator(e.Message.Chat.Id, txt[1], e, "State");
                }
                else
                {
                    await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please use keywords like: 'Weather Chandigarh' or 'W Chandigarh'");
                }
            }
            else if (e.Message.Text.ToLower() == "ranzo")
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Ranzo YT: " + "http://www.youtube.com/watch?v=kRUq3vQDPY4");
            }
            else
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Hey " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName);
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please type 'Help' for Commands!");
            }
        }

        public async static void TelegramBotAPIInitiator(long ID, string _DistrictIDs, Telegram.Bot.Args.MessageEventArgs e = null)
        {
            bool isTelegramSent = false;
            if (!string.IsNullOrWhiteSpace(_DistrictIDs))
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please wait while we're processing your request...");
                int _APIHitCounter = 1;
                string urlParameter = string.Empty;
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
                                        var res = emailStringBuilder(null, finalList);
                                        if (res.Length > 4096)
                                        {
                                            //foreach (var i in finalList)
                                            //{
                                            //    await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(i.ToArray(), null));
                                            //}
                                        }
                                        else
                                        {
                                            await telegramBotClient.SendTextMessageAsync(ID, res);
                                        }
                                        //Thread.Sleep(2000);
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, agraFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, karnalFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, faridabadFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, sonipatFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi140FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi141FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi142FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi143FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi144FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi145FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi146FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi147FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi148FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi149FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, delhi150FinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, hyderabadFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, puneFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
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
                                        await telegramBotClient.SendTextMessageAsync(ID, emailStringBuilder(null, mumbaiFinalList));
                                        LogWrite("******************* Alert sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                                        isTelegramSent = true;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    if (!isTelegramSent)
                    {
                        await telegramBotClient.SendTextMessageAsync(ID, "No data available. Please try again later!");
                        LogWrite("******************* NOT sent to: Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username);
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    #region Exception
                    List<string> list = new List<string>();
                    list.Add("Class name: ServiceMethods, Method name: TelegramBotAPIInitiator");
                    list.Add("Exception occurred for Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username + ", Message: " + e.Message.Text);
                    list.Add("Message:  " + ex?.Message?.ToString());
                    list.Add("StackTrace:  " + ex?.StackTrace?.ToString());
                    list.Add("InnerException.Message:  " + Convert.ToString(ex?.InnerException?.Message));
                    list.Add("InnerException.StackTrace:  " + Convert.ToString(ex?.InnerException?.StackTrace));
                    await telegramBotClient.SendTextMessageAsync(1531679242, exceptionStringBuilder(list.ToArray()));
                    #endregion
                }
            }
        }

        public async static void TelegramBotGameAPIInitiator(long ID, string _DistrictIDs, Telegram.Bot.Args.MessageEventArgs e = null)
        {
            try
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please wait while we're processing your request...");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                URLs = new Dictionary<int, string>();
                string urlParameter = "https://store-site-backend-static.ak.epicgames.com/freeGamesPromotions?locale=en-US&country=IN&allowCountries=IN";
                var task = client.GetAsync(urlParameter);
                task.Wait();
                var response = task.Result;
                List<List<string>> finalList = new List<List<string>>();
                string addFileEntry = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync();
                    readTask.Wait();
                    var result = JsonConvert.DeserializeObject<EpicApiModel>(readTask.Result).Data.Catalog.SearchStore;
                    foreach (var i in result.Elements)
                    {
                        List<string> list = new List<string>();
                        foreach (var i_2 in i.Promotions.PromotionalOffers)
                        {
                            list.Add("Please find the below details for free game:");
                            list.Add("Game Name: " + i.Title);
                            addFileEntry = i.Title;
                            list.Add("Game Description: " + i.Description);
                            foreach (var i_3 in i_2.PromotionalOffers)
                            {
                                list.Add("Start Date: " + i_3.StartDate?.DateTime);
                                list.Add("End Date: " + i_3.EndDate?.DateTime);
                            }
                            list.Add("URL: " + "https://www.epicgames.com/store/en-US/");
                        }
                        if (list.Count > 0 && (i.Promotions.PromotionalOffers.Length > 0 || i.Promotions.UpcomingPromotionalOffers.Length > 0))
                            finalList.Add(list);
                    }
                }
                if (finalList.Count > 0)
                {
                    await telegramBotClient.SendTextMessageAsync(ID, gameStringBuilder(null, finalList));
                }
                else
                {
                    await telegramBotClient.SendTextMessageAsync(ID, "No game data available. Please try again later!");
                }
            }
            catch (Exception ex)
            {
                #region Exception
                List<string> list = new List<string>();
                list.Add("Class name: ServiceMethods, Method name: TelegramBotGameAPIInitiator");
                list.Add("Exception occurred for Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username + ", Message: " + e.Message.Text);
                list.Add("Message:  " + ex?.Message?.ToString());
                list.Add("StackTrace:  " + ex?.StackTrace?.ToString());
                list.Add("InnerException.Message:  " + Convert.ToString(ex?.InnerException?.Message));
                list.Add("InnerException.StackTrace:  " + Convert.ToString(ex?.InnerException?.StackTrace));
                await telegramBotClient.SendTextMessageAsync(1531679242, exceptionStringBuilder(list.ToArray()));
                #endregion
            }
        }

        public async static void TelegramBotWeatherAPIInitiator(long ID, string region, Telegram.Bot.Args.MessageEventArgs e = null)
        {
            try
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please wait while we're processing your request...");
                var client = new HttpClient();
                var key = Convert.ToString(ConfigurationManager.AppSettings["WeatherAPIToken"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                URLs = new Dictionary<int, string>();
                string urlParameter = "http://api.weatherapi.com/v1/current.json?key=" + key + "&q=" + region + "&days=1&aqi=yes&alerts=yes";
                var task = client.GetAsync(urlParameter);
                task.Wait();
                var response = task.Result;
                List<string> list = new List<string>();
                string addFileEntry = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync();
                    readTask.Wait();
                    var result = JsonConvert.DeserializeObject<Temperatures>(readTask.Result);
                    if (result != null)
                    {
                        list.Add("Weather forecast for: " + result.Location.Name + ", " + result.Location.Country);
                        list.Add("");
                        list.Add("Temperature in celsius: " + result.Current.temp_c);
                        list.Add("Temperature in fahrenheit: " + result.Current.temp_f);
                        list.Add("Humidity as percentage: " + result.Current.humidity);
                        list.Add("Cloud cover as percentage: " + result.Current.cloud);
                        list.Add("UV Index: " + result.Current.uv);
                        list.Add("");
                        list.Add("Air Quality Data: ");
                        list.Add("   - Carbon Monoxide(μg / m3): " + result.Current.air_quality.co);
                        list.Add("   - Ozone (μg/m3): " + result.Current.air_quality.o3);
                        list.Add("   - Sulphur dioxide (μg/m3): " + result.Current.air_quality.so2);
                        list.Add("   - Nitrogen dioxide (μg/m3): " + result.Current.air_quality.no2);
                        list.Add("   - M2.5 (μg/m3): " + result.Current.air_quality.pm2_5);
                        list.Add("   - PM10 (μg/m3): " + result.Current.air_quality.pm10);
                        list.Add("");
                        list.Add("Weather condition: " + result.Current.condition.Text);
                        await telegramBotClient.SendPhotoAsync(ID, photo: "http:" + result.Current.condition.Icon, StringBuilder(list.ToArray()));
                    }
                }
            }
            catch (Exception ex)
            {
                #region Exception
                List<string> list = new List<string>();
                list.Add("Class name: ServiceMethods, Method name: TelegramBotGameAPIInitiator");
                list.Add("Exception occurred for Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username + ", Message: " + e.Message.Text);
                list.Add("Message:  " + ex?.Message?.ToString());
                list.Add("StackTrace:  " + ex?.StackTrace?.ToString());
                list.Add("InnerException.Message:  " + Convert.ToString(ex?.InnerException?.Message));
                list.Add("InnerException.StackTrace:  " + Convert.ToString(ex?.InnerException?.StackTrace));
                await telegramBotClient.SendTextMessageAsync(1531679242, exceptionStringBuilder(list.ToArray()));
                #endregion
            }
        }

        public async static void TelegramBotCovidInfoAPIInitiator(long ID, string region, Telegram.Bot.Args.MessageEventArgs e = null, string type = "")
        {
            try
            {
                await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Please wait while we're processing your request...");                
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://corona-virus-world-and-india-data.p.rapidapi.com/api_india"),
                    Headers = {
                                { "x-rapidapi-key", Convert.ToString(ConfigurationManager.AppSettings["CoronaAPIToken"])},
                                { "x-rapidapi-host", "corona-virus-world-and-india-data.p.rapidapi.com" },
                            },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var readTask = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CovidInfoModel>(readTask);
                    if (result != null)
                    {
                        if (type == "All")
                        {
                            List<string> list = new List<string>();
                            list.Add("Covid data at: " + result.total_values.lastupdatedtime);
                            list.Add("Active: " + result.total_values.active);
                            list.Add("Confirmed: " + result.total_values.confirmed);
                            list.Add("Deaths: " + result.total_values.deaths);
                            list.Add("Recovered: " + result.total_values.recovered);
                            await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, StringBuilder(list.ToArray()));
                        }
                        if (type == "State")
                        {
                            List<string> list = new List<string>();
                            list.Add("Covid data at: " + result.total_values.lastupdatedtime);
                            list.Add("Active: " + result.total_values.active);
                            list.Add("Confirmed: " + result.total_values.confirmed);
                            list.Add("Deaths: " + result.total_values.deaths);
                            list.Add("Recovered: " + result.total_values.recovered);
                            await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, StringBuilder(list.ToArray()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Exception
                List<string> list = new List<string>();
                list.Add("Class name: ServiceMethods, Method name: TelegramBotGameAPIInitiator");
                list.Add("Exception occurred for Name: " + e.Message.Chat.FirstName + " " + e.Message.Chat.LastName + ", Username: " + e.Message.Chat.Username + ", Message: " + e.Message.Text);
                list.Add("Message:  " + ex?.Message?.ToString());
                list.Add("StackTrace:  " + ex?.StackTrace?.ToString());
                list.Add("InnerException.Message:  " + Convert.ToString(ex?.InnerException?.Message));
                list.Add("InnerException.StackTrace:  " + Convert.ToString(ex?.InnerException?.StackTrace));
                await telegramBotClient.SendTextMessageAsync(1531679242, exceptionStringBuilder(list.ToArray()));
                #endregion
            }
        }
    }
}