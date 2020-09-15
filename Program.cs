using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using System.Windows;

namespace dns_updater
{
    public class Program
    {
        static void Main(string[] args)
        {
            string receiverIp = "192.168.0.100";
            int intervalSec = 60;

            //set software mode
            Console.WriteLine("Which mode? Server (S) / Client (C)");
            string mode = Console.ReadLine();

            switch (mode)
            {
                case "S":
                    RunServer();
                    break;
                case "C":
                    RunClient();
                    break;
            }


        }

        private static void RunServer()
        {
            //get data from sender
            string ipData =  GetDataFromSender();

            //temp print data 
            Console.WriteLine(ipData);
        }


        /// <summary>
        /// Gets IP data from sender via TCP
        /// Waits for 
        /// </summary>
        /// <returns></returns>
        private static string GetDataFromSender()
        {
            //set the port to listen on
            Int32 port = 80;

            //sender can be from any IP address
            IPAddress senderIp = IPAddress.Any;

            //create a listener for data from sender
            TcpListener server = new TcpListener(senderIp, port);

            //set received data as empty first
            String receivedData = null;

            try
            {
                //start the listener
                server.Start();

                //create empty bytes holder for receiving data
                Byte[] bytes = new Byte[256];

                //enter the listening loop
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        receivedData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //validate received data
                        if (IsIPDataValid(receivedData))
                        {
                            //if valid, reply with success response
                            string responseData = "Success";

                            //convert response string to bytes
                            byte[] byteResponse = System.Text.Encoding.ASCII.GetBytes(responseData);

                            //send response to sender
                            stream.Write(byteResponse, 0, byteResponse.Length);
                        }
                    }

                    //shutdown and end connection
                    client.Close();
                }
            }
            catch (Exception e)
            {
                //if error, show message to user
                Console.WriteLine("Error when receiving data:\n {0}", e);
            }
            finally
            {
                //stop listening for new clients.
                server.Stop();
            }

            //return response data to caller
            return receivedData;
        }

        private static bool IsIPDataValid(string receivedData)
        {
            //temp
            return true;
            throw new NotImplementedException();
        }

        /// <summary>
        /// The one client mode method, to start & run client mode.
        /// Uses infinite loop to keep client running, method does not close.
        /// </summary>
        private static void RunClient()
        {
            
            //get receiver address from user
            string receiverAddress = GetReceiverAddress();
            //update receiver every X (seconds), set default 10
            int interval = 10;

            //check if IP has changed
            CheckIp:
            if (IsIpChanged())
            {
                //IP has changed, so send new IP to receiver
                SendNewIpToReceiver(receiverAddress);

                //save a copy of the sent IP as Old IP
                SaveOldIp(receiverAddress);

                //wait interval
                WaitInterval(interval);

                //check again if IP has changed
                goto CheckIp;
            }
            //if IP is same, not changed
            else
            {
                //wait interval
                WaitInterval(interval);

                //check again if IP has changed
                goto CheckIp;
            }
        }

        private static string GetReceiverAddress()
        {
            //prompt user to type IP address
            Console.WriteLine("Type the IP address of DNS server");
            
            //store user input
            string ipAddress = Console.ReadLine();

            //return IP address to caller
            return ipAddress;
        }

        /// <summary>
        /// Pauses the application for interval time (seconds)
        /// </summary>
        /// <param name="interval">Time to wait in seconds</param>
        private static void WaitInterval(int interval)
        {
            //convert interval seconds to microseconds
            int intervalMs = interval * 1000;

            //execute sleeper
            Thread.Sleep(intervalMs);
        }

        private static void SendNewIpToReceiver(string receiverAddress)
        {
            //get old IP
            string oldIp = GetOldIp();
            //get new IP
            string newIp = GetNewIp();

            //combine old & new IP into a XML
            XElement ipList = new XElement("IP",
                new XElement("OLD", oldIp),
                new XElement("NEW", newIp)
            );

            //send old & new IP to receiver, get response also
            string response = SendDataToReceiver(receiverAddress, ipList);

            //display response temp
            Console.WriteLine(response);
        }

        /// <summary>
        /// Sends data in the XML format to specified IP address
        /// return response from receiver as string
        /// </summary>
        /// <param name="receiverAddress"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string SendDataToReceiver(string receiverAddress, XElement data)
        {
            //construct message to be sent
            string message = data.ToString();

            //set TCP port number
            int port = 80;

            //to store the response from receiver
            String responseData = String.Empty;

            //try to parse receivers IP address
            IPAddress ipAddress = IPAddress.Parse(receiverAddress);

            //if IP address did not parse, return error
            if (ipAddress == null) { throw new Exception("Receiver IP address not valid!"); }

            //prepare tcp connector
            TcpClient client = new TcpClient(receiverAddress, port);

            //translate the passed message into ASCII and store it as a Byte array.
            Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(message);

            try
            {
                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(byteData, 0, byteData.Length);

                // Buffer to store the response bytes.
                byteData = new Byte[256];

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(byteData, 0, byteData.Length);
                
                //save the response data
                responseData = System.Text.Encoding.ASCII.GetString(byteData, 0, bytes);

                //close everything.
                stream.Close();
                client.Close();
            }
            //if fail
            catch (Exception e)
            {
                //show error message to user
                Console.WriteLine("Error when sending data:\n {0}", e);
            }

            return responseData;
        }

        private static bool IsIpChanged()
        {
            //get old IP (represents the IP in receivers current records)
            string oldIp = GetOldIp();
            //get new IP
            string newIp = GetNewIp();

            //if both IPs match
            if (oldIp == newIp)
            {
                //IP is the same, no change
                return false;
            }
            else
            {
                //IP has changed
                return true;
            }
        }

        /// <summary>
        /// Gets computers public IP address from remote site
        /// </summary>
        private static string GetNewIp()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }

        public static string GetOldIp()
        {
            XDocument ipAddressList;
        GetIp:
            try
            {
                //load file which has IP address
                ipAddressList = XDocument.Load(Const.OldIpListFile);
            }
            //if file not found
            catch (FileNotFoundException)
            {
                //make a new file, with blank IP address
                SaveOldIp("0.0.0.0");

                //try to load IP file again
                goto GetIp;
            }

            //extract IP address from loaded file
            string ipAddress = ((XElement) ipAddressList.FirstNode).Value;

            //return ip address to caller
            return ipAddress;
        }

        public static void SaveOldIp(string ipAddress)
        {
            //prepare to save IP address
            XElement ipList = new XElement("OldIP", ipAddress);

            //save ip address to file on disk  (TEMP FILE LOCATION)
            ipList.Save(Const.OldIpListFile);

        }
    }
}