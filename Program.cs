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
            //set software mode
            Console.WriteLine("Which mode? Server (S) / Client (C)");
            string mode = Console.ReadLine();

            switch (mode)
            {
                case "S":
                    Server.Run();
                    break;
                case "C":
                    Client client = new Client();
                    client.Run();
                    break;
            }
        }
    }
}