using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VaccineFinderService
{
    public class NetworkManagement
    {
        public static void RandomIPGenerate()
        {
            var data = new byte[4];
            new Random().NextBytes(data);
            IPAddress ip = new IPAddress(data);

            //r.nextInt(256) + "." + r.nextInt(256) + "." + r.nextInt(256) + "." + r.nextInt(256);
            var r = new Random();
            var ipAddress = r.Next(256) + "." + r.Next(256) + "." + r.Next(256) + "." + r.Next(256);
            setIP(ipAddress);
            IEnumerable<IPAddress> address = Test();
            var abc = GetPublicIP();
        }

        public static void setIP(string ip_address)
        {
            string subnet_mask = "255.255.255.0";
            ManagementClass objMC =
              new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    ManagementBaseObject setIP;
                    ManagementBaseObject newIP =
                      objMO.GetMethodParameters("EnableStatic");

                    newIP["IPAddress"] = new string[] { ip_address };
                    newIP["SubnetMask"] = new string[] { subnet_mask };

                    objMO.InvokeMethod("EnableStatic", newIP, null);
                }
            }
        }

        public static IEnumerable<IPAddress> Test()
        {
            // Get the list of network interfaces for the local computer.
            var adapters = NetworkInterface.GetAllNetworkInterfaces();

            // Return the list of local IPv4 addresses excluding the local
            // host, disconnected, and virtual addresses.
            return (from adapter in adapters
                    let properties = adapter.GetIPProperties()
                    from address in properties.UnicastAddresses
                    where adapter.OperationalStatus == OperationalStatus.Up &&
                          address.Address.AddressFamily == AddressFamily.InterNetwork &&
                          !address.Equals(IPAddress.Loopback) &&
                          address.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred &&
                          address.AddressPreferredLifetime != UInt32.MaxValue
                    select address.Address);
        }

        public static string GetPublicIP()
        {
            String direction = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);

            return direction;
        }

        public static void setGateway(string gateway)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    ManagementBaseObject setGateway;
                    ManagementBaseObject newGateway =
                      objMO.GetMethodParameters("SetGateways");

                    newGateway["DefaultIPGateway"] = new string[] { gateway };
                    newGateway["GatewayCostMetric"] = new int[] { 1 };

                    setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
                }
            }
        }

        public static void setDNS(string NIC, string DNS)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    // if you are using the System.Net.NetworkInformation.NetworkInterface
                    // you'll need to change this line to
                    // if (objMO["Caption"].ToString().Contains(NIC))
                    // and pass in the Description property instead of the name 
                    if (objMO["Caption"].Equals(NIC))
                    {
                        ManagementBaseObject newDNS =
                          objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        newDNS["DNSServerSearchOrder"] = DNS.Split(',');
                        ManagementBaseObject setDNS =
                          objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                    }
                }
            }
        }

        public static void setWINS(string NIC, string priWINS, string secWINS)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].Equals(NIC))
                    {
                        ManagementBaseObject setWINS;
                        ManagementBaseObject wins =
                        objMO.GetMethodParameters("SetWINSServer");
                        wins.SetPropertyValue("WINSPrimaryServer", priWINS);
                        wins.SetPropertyValue("WINSSecondaryServer", secWINS);

                        setWINS = objMO.InvokeMethod("SetWINSServer", wins, null);
                    }
                }
            }
        }
    }
}
