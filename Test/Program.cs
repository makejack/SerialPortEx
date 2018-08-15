using System;
using System.Collections.Generic;
using System.Text;
using SerialPortEx;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SearchPort searchPort = new SearchPort(100);
            searchPort.PortChange += SearchPort_PortChange;
            searchPort.Setup();

            

            Console.ReadKey();
        }

        private static void SearchPort_PortChange(List<string> portNames)
        {
            foreach (string item in portNames)
            {
                Console.WriteLine(item);
            }
        }
    }
}
