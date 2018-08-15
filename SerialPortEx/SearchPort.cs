using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using Microsoft.VisualBasic.Devices;

namespace SerialPortEx
{
    public class SearchPort
    {
        public delegate void PortChangeEventHandler(List<string> portNames);
        public event PortChangeEventHandler PortChange;

        private Computer m_computer = new Computer();
        private Thread m_searchThread;
        private bool m_stop;
        private int m_portCount = 0;

        public SearchPort() { }

        public SearchPort(int intervalTimeouts)
        {
            this.IntervalTimeouts = intervalTimeouts;
        }

        /// <summary>
        /// 间隔时间
        /// </summary>
        public int IntervalTimeouts { get; set; } = 100;

        public void Setup()
        {
            if (m_stop)
            {
                m_stop = false;
            }

            if(m_searchThread == null)
            {
                m_searchThread = new Thread(SearchPortDevice)
                {
                    IsBackground = true,
                };
                m_searchThread.Start();
            }
        }

        private void SearchPortDevice()
        {
            int count = 0;
            try
            {
                while (!m_stop)
                {
                    count = m_computer.Ports.SerialPortNames.Count;
                    if (count != m_portCount)
                    {
                        m_portCount = count;
                        PortChange?.Invoke(new List<string>(m_computer.Ports.SerialPortNames));
                    }
                    Thread.Sleep(IntervalTimeouts);
                }
            }
            finally
            {
                m_searchThread = null;
            }
        }

        public void Stop()
        {
            m_stop = true;
        }
    }
}
