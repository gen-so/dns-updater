using System;
using System.Net;
using System.Net.Sockets;

namespace dns_updater
{
    public class Server
    {
        public static void Run()
        {
            //get data from sender
            string ipData = GetDataFromSender();

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

    }
}