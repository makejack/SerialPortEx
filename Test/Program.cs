using System;
using System.Collections.Generic;
using System.Text;
using SerialPortEx;

namespace Test
{
    class Program
    {

        static List<SerialPort> m_serialPorts = new List<SerialPort>();

        static void Main(string[] args)
        {
            SearchPort searchPort = new SearchPort(100);
            searchPort.PortChange += SearchPort_PortChange;
            searchPort.Setup();

            for (int i = 0; i < 3; i++)
            {
                SerialPort serialPort1 = new SerialPort(true)
                {
                    StopBit = PortAPI.StopBits.STOP_1,
                    BaudRate = PortAPI.BaudRates.B19200,
                    DataBit = PortAPI.DataBits.Bit_8,
                    Parity = PortAPI.Paritys.NONE,
                };
                serialPort1.DataReceived += SerialPort1_DataReceived;
                serialPort1.SerialPortChange += SerialPort1_SerialPortChange;

                m_serialPorts.Add(serialPort1);
            }
            

            Console.ReadKey();
        }

        private static void SerialPort1_SerialPortChange(SerialPort sender)
        {
            throw new NotImplementedException();
        }

        private static void SerialPort1_DataReceived(int port)
        {
            throw new NotImplementedException();
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
