using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;


namespace dns_updater
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        /// <summary>
        /// The one client mode method, to start & run client mode.
        /// Infinite loop to keep client running, method does not close.
        /// </summary>
        /// <param name="receiverAddress">receiver's IP address</param>
        /// <param name="interval">time to poll receiver</param>
        private void RunClient(object receiverAddress, object interval)
        {
            CheckIp:
            //check if IP has changed
            if (isIpChanged())
            {
                //IP has changed, so send new IP to receiver
                sendNewIpToReceiver();
                
                //wait interval
                waitInterval(interval);

                //check again if IP has changed
                goto CheckIp;
            }
            //if IP is same, not changed
            else
            {
                //wait interval
                waitInterval(interval);

                //goto back to top to check for IP change
                goto CheckIp;
            }
        }

        private void waitInterval(object interval)
        {
            throw new NotImplementedException();
        }

        private void sendNewIpToReceiver()
        {
            throw new NotImplementedException();
        }

        private bool isIpChanged()
        {
            throw new NotImplementedException();
        }

    }

}



