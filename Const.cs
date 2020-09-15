using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dns_updater
{
    public class Const
    {
        //location path of application
        public static readonly string AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

        //location & name of ip list file
        public static readonly string OldIpListFile = AppLocation + "\\ip-list.xml";
    }
}
