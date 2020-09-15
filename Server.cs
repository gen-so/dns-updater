using System;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace dns_updater
{
    public class Server
    {
        public static void Run()
        {
            GetData:
            //get data from sender
            string ipData = GetDataFromSender();

            //parse data to XML
            XElement ipList = XElement.Parse(ipData);

            //extract old IP from XML
            XElement oldIp = ipList.Element("OLD");

            //extract new IP from XML
            XElement newIp = ipList.Element("NEW");

            Console.WriteLine($"OLD:{oldIp.Value}\nNEW:{newIp.Value}");
            
            //go back to getting data from sender
            goto GetData;
        }

        /// <summary>
        /// Gets IP data from sender via TCP
        /// </summary>
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

                Console.Write("Waiting for a updates...\n");

                // Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();
                
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                //receive all the data sent by the client
                int i = stream.Read(bytes, 0, bytes.Length);

                //translate data bytes to a ASCII string.
                receivedData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                //let user know, updates received
                Console.WriteLine("Updates received");

                //close connection
                client.Close();


                //enter the listening loop
                //while (true)
                //{
                //    Console.Write("Waiting for a connection... ");

                //    // Perform a blocking call to accept requests.
                //    TcpClient client = server.AcceptTcpClient();
                //    Console.WriteLine("Connected!");

                //    // Get a stream object for reading and writing
                //    NetworkStream stream = client.GetStream();

                //    //receive all the data sent by the client
                //    stream.Read(bytes, 0, bytes.Length);

                //    //translate data bytes to a ASCII string.
                //    receivedData = System.Text.Encoding.ASCII.GetString(bytes, 0, 0);


                //    //int i;

                //    //loop to receive all the data sent by the client.
                //    //while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                //    //{
                //    //    //translate data bytes to a ASCII string.
                //    //    //receivedData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                //    //    //validate received data
                //    //    //if (!IsIPDataValid(receivedData)) continue;

                //    //    //if valid, reply with success response
                //    //    //string responseData = "Success";

                //    //    //convert response string to bytes
                //    //    //byte[] byteResponse = System.Text.Encoding.ASCII.GetBytes(responseData);

                //    //    //send response to sender
                //    //    //stream.Write(byteResponse, 0, byteResponse.Length);
                //    //}

                //    //shutdown and end connection
                //    client.Close();
                //}
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

    }
}