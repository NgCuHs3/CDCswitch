using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.net
{
    public class Neter
    {
        public static int MOUSEUSER_PORT = 8989;
        public static int DATATRANFER_PORT = 7979;
        public static int TOUCHER_PORT = 6969;
        public static int CAPTURER_PORT = 5959;
        public static string minicap = "minicap";
        public static string tccclient = "tccclient";
        private static Socket toucher;
        private static Socket capturer;
        private static Socket datatranfer;
        private static Socket mouseuser;

        public static Socket Toucher { get => toucher; set => toucher = value; }
        public static Socket Capturer { get => capturer; set => capturer = value; }
        public static Socket Datatranfer { get => datatranfer; set => datatranfer = value; }
        public static Socket Mouseuser { get => mouseuser; set => mouseuser = value; }
    }
}
