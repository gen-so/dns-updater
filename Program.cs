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
            TcpListener server = null;
            try
            {
                // Set the TcpListener port.
                Int32 port = 80;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
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

            CheckIp:
            //check if IP has changed
            if (IsIpChanged())
            {
                //IP has changed, so send new IP to receiver
                SendNewIpToReceiver(receiverAddress);

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

                //goto back to top to check for IP change
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

        private static void WaitInterval(object interval)
        {
            return;
            throw new NotImplementedException();
        }

        private static void SendNewIpToReceiver(string receiverAddress)
        {
            //get old IP
            string oldIp = GetOldIp();
            //get new IP
            string newIp = GetNewIp();

            //prepare IPs to be sent receiver
            var ipData = new {OldIp = oldIp, NewIp = newIp};

            //send old & new IP to receiver
            SendDataToReceiver(receiverAddress, ipData);
        }

        private static void SendDataToReceiver(string receiverAddress, object ipData)
        {
            //setup connection parameters
            string message = "Hello";
            int port = 80; //TCP port number


            //try to parse receivers IP address
            IPAddress ipAddress = IPAddress.Parse(receiverAddress);

            //if IP address did not parse, return error
            if (ipAddress == null)
            {
                throw new Exception("Receiver IP address not valid!");
            }


            TcpClient client = new TcpClient(receiverAddress, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            try
            {
                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();

            Console.WriteLine("Sent Successfully");
        }

        private static void SendDataToReceiver2(string receiverAddress, object ipData)
        {
            //setup connection parameters
            string response = "Hello";
            int port = 80; //TCP port number


            //try to parse receivers IP address
            IPAddress ipAddress = IPAddress.Parse(receiverAddress);

            //if IP address did not parse, return error
            if (ipAddress == null)
            {
                throw new Exception("Receiver IP address not valid!");
            }


            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);
            byte[] receiveBuffer = new byte[100];

            try
            {
                using (TcpClient client = new TcpClient(serverEndPoint))
                {
                    using (Socket socket = client.Client)
                    {
                        socket.Connect(serverEndPoint);

                        byte[] data = Encoding.ASCII.GetBytes(response);

                        socket.Send(data, data.Length, SocketFlags.None);

                        socket.Receive(receiveBuffer);

                        Console.WriteLine(Encoding.ASCII.GetString(receiveBuffer));
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket Exception : ", socketException.Message);
                throw;
            }

            Console.WriteLine("Sent Successfully");
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
            //load file which has IP address
            XDocument ipAddressList = XDocument.Load(Const.OldIpListFile);

            //extract IP address from loaded file
            string ipAddress = ((XElement)ipAddressList.FirstNode).Value;

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