using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Management;
using System.Net;

namespace MadCow
{
    public class DNShoudini
    {
        public static void checkDNS()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nics)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    var ip = Dns.GetHostAddresses("www.d3sharp.com");
                    IPAddressCollection ips = ni.GetIPProperties().DnsAddresses;
                    if (ips[0].ToString().Contains(ip[0].ToString()))
                    {
                        Console.WriteLine("Found correct DNS settings.");
                        break;
                    }
                    SetCustomNameservers(ip[0] + "," + ips[0]);                   
                    break;
                }
            }
        }

        /// <summary>
        /// Set DNS for active NIC.
        /// </summary>
        /// <param name="dnsServers">String of dns ip's divided by ','</param>
        static void SetCustomNameservers(String dnsServers)
        {
            using (var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (var networkConfigs = networkConfigMng.GetInstances())
                {
                    foreach (var managementObject in networkConfigs.Cast<ManagementObject>().Where(objMO => (bool)objMO["IPEnabled"]))
                    {
                        using (var newDNS = managementObject.GetMethodParameters("SetDNSServerSearchOrder"))
                        {
                            newDNS["DNSServerSearchOrder"] = dnsServers.Split(',');
                            managementObject.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                        }
                    }
                }
            }
            Console.WriteLine("Succesfully modified DNS records.");
        }

        public static void RestoreNameservers()
        {
            using (var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (var networkConfigs = networkConfigMng.GetInstances())
                {
                    foreach (var managementObject in networkConfigs.Cast<ManagementObject>().Where(objMO => (bool)objMO["IPEnabled"]))
                    {
                        using (var newDNS = managementObject.GetMethodParameters("SetDNSServerSearchOrder"))
                        {
                            newDNS["DNSServerSearchOrder"] = null;
                            managementObject.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                        }
                    }
                }
            }
        }
    }
}
