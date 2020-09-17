using System;
using System.Net;

namespace dns_updater
{
    public class View
    {
        //PUBLIC METHODS
        /// <summary>
        /// Gets server's IP address from user
        /// </summary>
        public static string GetReceiverAddress()
        {
            string ipAddress;

            GetIp:
            //prompt user to type IP address
            Console.WriteLine("Type the IP address of DNS server");

            //store user input
            ipAddress = Console.ReadLine();

            //check IP, if invalid, try again
            if (!isIpValid(ipAddress)) { goto GetIp; }

            //return IP address to caller
            return ipAddress;
        }
        
        public static int GetReceiverPort()
        {
            string port;

            GetIp:
            //prompt user to type port
            Console.WriteLine("Which port to connect to?");

            //store user input
            port = Console.ReadLine();

            //check port, if invalid, try again
            //if (!isIpValid(port)) { goto GetIp; }

            //return port to caller
            return int.Parse(port);
        }


        //PRIVATE METHODS
        private static bool isIpValid(string receiverAddress)
        {
            try
            {
                //try to parse IP address
                IPAddress ipAddress = IPAddress.Parse(receiverAddress);
            }
            catch (FormatException)
            {
                //let user know IP address is invalid
                Console.WriteLine("Server IP address not valid!");

                //return as invalid
                return false;
            }

            //if no exception, then IP is valid
            return true;
        }

    }
}