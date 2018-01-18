using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMonitor
{

    public static class Device
    {
        public static double L = 0.0;
        public static Boolean chek = false;
        public static Boolean ismax = false;
        public static int time = 1;
        public static SerialPort Port { get; private set; }
        public static String all;
        public static String[] values = new string[3];
        public static int max;
        public static int[] collection = new int[1];


        public static void Open(string portName)
        {
            Port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            Port.Open();
        }
        public static void Read()
        {
            while (values.Length < 3 || values[1] == null || values[0] == null || values[2] == null)
            {
                all = Port.ReadLine();
                all = all.Trim('\r', ' ');
                values = all.Split('\t');
            }
            
            if (values.Length > 3)
            {
                all = Port.ReadLine();
                all = all.Trim('\r', ' ');
                values = all.Split('\t');
            }

            }

        }
}
