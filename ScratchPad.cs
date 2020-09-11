using System;
using System.Management;

namespace dns_updater
{
    public class ScratchPad
    {
        static void updateARecord()
        {
            //prepare data for 
            //----- PART 1
            Console.WriteLine("Preparing connection data.");

            string _username = "Administrator";
            string _password = "j4$RKnZJGwZcP&INljx@B3xJPULj$cH-";
            //string _path = "3.129.204.60";
            string _path = "dns";
            //string _path = "ec2-3-129-204-60.us-east-2.compute.amazonaws.com";
            string _fullPath = $"\\\\{_path}\\root\\cimv2";
            //string _fullPath = $"\\\\{_path}\\Root\\MicrosoftDNS";
            //string domain = "ntlmdomain:genso.tk";

            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Username = _username;
            options.Password = _password;
            //options.Authority = domain;

            ManagementScope scope = new ManagementScope(_fullPath, options);
            //ManagementScope scope = new ManagementScope(@"\\3.129.204.60\Root\MicrosoftDNS");

            //----- PART 2
            Console.WriteLine("Starting connection to server.");

            try
            {
                scope.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                //throw;
            }


            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery(
                "SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                // Display the remote computer information
                Console.WriteLine("Computer Name : {0}",
                    m["csname"]);
                Console.WriteLine("Windows Directory : {0}",
                    m["WindowsDirectory"]);
                Console.WriteLine("Operating System: {0}",
                    m["Caption"]);
                Console.WriteLine("Version: {0}", m["Version"]);
                Console.WriteLine("Manufacturer : {0}",
                    m["Manufacturer"]);
            }


            Console.WriteLine("Done!");

            Console.ReadLine();

        }

        private ManagementPath UpdateARecord(string strDNSZone, string strHostName, string strIPAddress)
        {
            ManagementScope mgmtScope = new ManagementScope(@"\\.\Root\MicrosoftDNS");
            ManagementClass mgmtClass = null;
            ManagementBaseObject mgmtParams = null;
            ManagementObjectSearcher mgmtSearch = null;
            ManagementObjectCollection mgmtDNSRecords = null;
            string strQuery;

            strQuery = string.Format("SELECT * FROM MicrosoftDNS_AType WHERE OwnerName = '{0}.{1}'", strHostName, strDNSZone);

            mgmtScope.Connect();

            mgmtSearch = new ManagementObjectSearcher(mgmtScope, new ObjectQuery(strQuery));

            mgmtDNSRecords = mgmtSearch.Get();

            //// Multiple A records with the same record name, but different IPv4 addresses, skip.
            //if (mgmtDNSRecords.Count > 1)
            //{
            //    // Take appropriate action here.
            //}
            //// Existing A record found, update record.
            //else
            if (mgmtDNSRecords.Count == 1)
            {
                ManagementObject mo = new ManagementObject();
                foreach (ManagementObject mgmtDNSRecord in mgmtDNSRecords)
                {
                    if (mgmtDNSRecord["RecordData"].ToString() != strIPAddress)
                    {
                        mgmtParams = mgmtDNSRecord.GetMethodParameters("Modify");
                        mgmtParams["IPAddress"] = strIPAddress;

                        mgmtDNSRecord.InvokeMethod("Modify", mgmtParams, null);
                    }
                    mo = mgmtDNSRecord;
                    break;
                }

                return new ManagementPath(mo["RR"].ToString());
            }
            // A record does not exist, create new record.
            else
            {
                mgmtClass = new ManagementClass(mgmtScope, new ManagementPath("MicrosoftDNS_AType"), null);

                mgmtParams = mgmtClass.GetMethodParameters("CreateInstanceFromPropertyData");
                mgmtParams["DnsServerName"] = Environment.MachineName;
                mgmtParams["ContainerName"] = strDNSZone;
                mgmtParams["OwnerName"] = strDNSZone;// string.Format("{0}.{1}", strHostName.ToLower(), strDNSZone);

                mgmtParams["IPAddress"] = strIPAddress;

                var outParams = mgmtClass.InvokeMethod("CreateInstanceFromPropertyData", mgmtParams, null);

                if ((outParams.Properties["RR"] != null))
                {
                    return new ManagementPath(outParams["RR"].ToString());
                }
            }

            return null;
        }

    }
}