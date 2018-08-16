using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SerialPortEx
{
    public class SerialPort
    {

        public SerialPort(bool autoClose = false)
        {
            if (autoClose)
            {
                SearchPort searchPort = new SearchPort();
                searchPort.PortChange += OnPortChange;
                searchPort.Setup();
            }
        }

        private void OnPortChange(List<string> portNames)
        {
            if (IsOpen)
            {
                bool contains = portNames.Contains(PortName);
                if (!contains)
                {
                    try
                    {
                        Close();
                    }
                    catch
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 端口编号
        /// </summary>
        /// <value></value>
        internal int Port { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        /// <value></value>
        private string m_portName;
        public string PortName
        {
            get { return m_portName; }
            set
            {
                if (m_portName != value)
                {
                    m_portName = value;

                    if (!string.IsNullOrEmpty(m_portName))
                    {
                        try
                        {
                            Port = Convert.ToInt32(m_portName.Replace("COM", ""));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        Port = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 数据位
        /// </summary>
        /// <value></value>
        public PortAPI.DataBits DataBit { get; set; }

        /// <summary>
        /// 停止位
        /// </summary>
        /// <value></value>
        public PortAPI.StopBits StopBit { get; set; }

        /// <summary>
        /// 检验位
        /// </summary>
        /// <value></value>
        public PortAPI.Paritys Parity { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        /// <value></value>
        public PortAPI.BaudRates BaudRate { get; set; }

        /// <summary>
        /// 是否打开端口
        /// </summary>
        /// <value></value>
        public bool IsOpen { get; set; }

        /// <summary>
        /// 打开端口
        /// </summary>
        public void Open()
        {
            PortAPI.ErrorCodes code = Open(Port);
            if (code == PortAPI.ErrorCodes.SIO_OK)
            {
                IsOpen = true;

                Setioctl();

                SetDataReceived();

                SerialPortChange?.Invoke(this);
            }
            else
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="port">端口编号</param>
        public static PortAPI.ErrorCodes Open(int port)
        {
            int ret = PortAPI.sio_open(port);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 设置端口波特率、数据位、停止位、检验位
        /// </summary>
        public void Setioctl()
        {
            PortAPI.ErrorCodes code = Setioctl(Port, BaudRate, (int)StopBit | (int)DataBit | (int)Parity);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 设置端口波特率、数据位、停止位、检验位
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="mode">数据位|停止位|检验位 - 格式</param>
        /// <returns></returns>
        public static PortAPI.ErrorCodes Setioctl(int port, PortAPI.BaudRates baudRate, int mode)
        {
            int ret = PortAPI.sio_ioctl(port, (int)baudRate, mode);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        public void Close()
        {
            PortAPI.ErrorCodes code = Close(Port);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
            IsOpen = false;
            SerialPortChange?.Invoke(this);
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <returns></returns>
        public static PortAPI.ErrorCodes Close(int port)
        {
            int ret = PortAPI.sio_close(port);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 读取端口数据
        /// </summary>
        /// <param name="buffer">数据流</param>
        public void Read(byte[] buffer)
        {
            PortAPI.ErrorCodes code = Read(Port, buffer, buffer.Length);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 读取端口数据
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="buffer">数据流</param>
        /// <param name="len">数据长度</param>
        public static PortAPI.ErrorCodes Read(int port, byte[] buffer, int len)
        {
            int ret = PortAPI.sio_read(port, buffer, len);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 清除端口缓冲区数据
        /// </summary>
        /// <param name="func">0 - 清除写入缓冲区数据  1 - 清除读缓冲区数据    2 - 清除读写缓冲区数据</param>
        public void Flush(int func)
        {
            PortAPI.ErrorCodes code = Flush(Port, func);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 清除端口缓冲区数据
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="state">0 - 清除写入缓冲区数据  1 - 清除读缓冲区数据    2 - 清除读写缓冲区数据</param>
        public static PortAPI.ErrorCodes Flush(int port, int func)
        {
            int ret = PortAPI.sio_flush(port, func);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="str">发送的字符数据</param>
        public void Write(string str)
        {
            PortAPI.ErrorCodes code = Write(Port, str, false);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="str">发送的字符数据</param>
        /// <param name="hex">是否十六进制 true - 十六进制发送</param>
        public void Write(string str, bool hex)
        {
            PortAPI.ErrorCodes code = Write(Port, str, hex);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="buffer">数据流</param>
        public void Write(byte[] buffer)
        {
            PortAPI.ErrorCodes code = Write(Port, buffer);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="str">发送的字符数据</param>
        public static PortAPI.ErrorCodes Write(int port, string str)
        {
            return Write(port, str, false);
        }

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="str">发送的字符数据</param>
        /// <param name="hex">是否十六进制 true - 十六进制发送</param>
        public static PortAPI.ErrorCodes Write(int port, string str, bool hex)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }
            byte[] buffer;
            if (!hex)
            {
                buffer = Encoding.GetEncoding("GB2312").GetBytes(str);
            }
            else
            {
                str = str.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(".", "").Replace(",", "");
                if (!Regex.IsMatch(str, "^[0-9A-Fa-f]*$"))
                {
                    throw new Exception("字符内包容特殊的字符或字母");
                }
                if (str.Length % 2 != 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                buffer = new byte[str.Length / 2];
                int index = 0;
                for (int i = 0; i < str.Length; i += 2)
                {
                    buffer[index] = Convert.ToByte(str.Substring(i, 2), 16);
                    index += 1;
                }
            }
            return Write(port, buffer);
        }

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="buffer">数据流</param>
        /// <returns></returns>
        public static PortAPI.ErrorCodes Write(int port, byte[] buffer)
        {
            int ret = PortAPI.sio_write(port, buffer, buffer.Length);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 获取接收缓冲区的数据长度
        /// </summary>
        /// <returns>-1缓冲区内没有数据</returns>
        public int Iqueue()
        {
            int ret = Iqueue(Port);
            if (ret < 0)
            {
                throw new Exception(GetErrorStr((PortAPI.ErrorCodes)ret));
            }
            return ret;
        }

        /// <summary>
        /// 获取接收缓冲区的数据长度
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <returns>-1缓冲区内没有数据</returns>
        public static int Iqueue(int port)
        {
            int ret = PortAPI.sio_iqueue(port);
            return ret;
        }

        /// <summary>
        /// 设置端口写入超时时间
        /// </summary>
        /// <param name="totaltimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值 默认0</param>
        public void SetWriteTimeouts(int totalTimeouts, int intervalTimeouts)
        {
            PortAPI.ErrorCodes code = SetWriteTimeouts(Port, totalTimeouts, intervalTimeouts);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 获取端口写入超时时间
        /// </summary>
        /// <returns></returns>
        public int GetWriteTimeouts()
        {
            int totalTimeout;
            PortAPI.ErrorCodes code = GetWriteTimeouts(Port, out totalTimeout);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
            return totalTimeout;
        }

        /// <summary>
        /// 获取端口写入超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <returns></returns>
        public static PortAPI.ErrorCodes GetWriteTimeouts(int port, out int totalTimeout)
        {
            int ret = PortAPI.sio_GetWriteTimeouts(port, out totalTimeout);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 设置端口写入超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totaltimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值 默认0</param>
        public static PortAPI.ErrorCodes SetWriteTimeouts(int port, int totalTimeouts, int intervalTimeouts)
        {
            int ret = PortAPI.sio_SetWriteTimeouts(port, totalTimeouts, intervalTimeouts);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 设置端口读取超时时间
        /// </summary>
        /// <param name="totalTimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值 默认0</param>
        public void SetReadTimeouts(int totalTimeouts, int intervalTimeouts)
        {
            PortAPI.ErrorCodes code = SetReadTimeouts(Port, totalTimeouts, intervalTimeouts);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 设置端口读取超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totalTimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值 默认0</param>
        public static PortAPI.ErrorCodes SetReadTimeouts(int port, int totalTimeouts, int intervalTimeouts)
        {
            int ret = PortAPI.sio_SetReadTimeouts(port, totalTimeouts, intervalTimeouts);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 获取端口读取超时时间
        /// </summary>
        /// <param name="totalTimeouts">超时总值</param>
        /// <param name="intervalTimeouts">间隔超时值</param>
        public void GetReadTimeouts(out int totalTimeouts, out int intervalTimeouts)
        {
            PortAPI.ErrorCodes code = GetReadTimeouts(Port, out totalTimeouts, out intervalTimeouts);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 获取端口读取超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totalTimeouts">超时总值</param>
        /// <param name="intervalTimeouts">间隔超时值</param>
        /// <returns></returns>
        public static PortAPI.ErrorCodes GetReadTimeouts(int port, out int totalTimeouts, out int intervalTimeouts)
        {
            int ret = PortAPI.sio_GetReadTimeouts(port, out totalTimeouts, out intervalTimeouts);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 设置端口数据接收回调
        /// </summary>
        private void SetDataReceived()
        {
            PortAPI.ErrorCodes code = SetDataReceived(Port, m_dataReceived);
            if (code != PortAPI.ErrorCodes.SIO_OK)
            {
                throw new Exception(GetErrorStr(code));
            }
        }

        /// <summary>
        /// 当端口接收到数据时的回调方法
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="func">回调方法</param>
        /// <param name="len">数据长度超过len开始回调 默认1</param>
        /// <returns></returns>
        public static PortAPI.ErrorCodes SetDataReceived(int port, PortAPI.DataReceivedEventHandler func, int len = 1)
        {
            int ret = PortAPI.sio_cnt_irq(port, func, len);
            return (PortAPI.ErrorCodes)ret;
        }

        /// <summary>
        /// 获取端口错误的字符
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetErrorStr(PortAPI.ErrorCodes code)
        {
            switch (code)
            {
                default: return "OK";
                case PortAPI.ErrorCodes.SIO_BADPORT: return "没有此端口或端口未打开";
                case PortAPI.ErrorCodes.SIO_OUTCONTROL: return "无法控制此板";
                case PortAPI.ErrorCodes.SIO_NODATA: return "没有数据代读取或没有缓冲区供写";
                case PortAPI.ErrorCodes.SIO_OPENFAIL: return "没有此端口或端口已打开";
                case PortAPI.ErrorCodes.SIO_RTS_BY_HW: return "因为H/W流量控制而不能设置";
                case PortAPI.ErrorCodes.SIO_BADPARM: return "无效参数";
                case PortAPI.ErrorCodes.SIO_WIN32FAIL: return "调用win32函数失败请调用GetLastError函数以获取错误代码";
                case PortAPI.ErrorCodes.SIO_BOARDNOTSUPPORT: return "此版本不支持这个函数";
                case PortAPI.ErrorCodes.SIO_FAIL: return "PCOMM函数运行结果失败";
                case PortAPI.ErrorCodes.SIO_ABORT_WRITE: return "写入已被锁定，用户已放弃写入";
                case PortAPI.ErrorCodes.SIO_WRITETIMEOUT: return "已发生写入超时";
            }
        }

        private event PortAPI.DataReceivedEventHandler m_dataReceived;
        /// <summary>
        /// 端口数据接收事件
        /// </summary>
        public event PortAPI.DataReceivedEventHandler DataReceived
        {
            add
            {
                m_dataReceived += value;
                if (IsOpen)
                {
                    SetDataReceived();
                }
            }
            remove
            {
                m_dataReceived -= value;
                if (IsOpen)
                {
                    SetDataReceived();
                }
            }
        }

        public delegate void SerialPortChangeEventHandler(SerialPort sender);

        /// <summary>
        /// 端口变化事件
        /// </summary>
        public event SerialPortChangeEventHandler SerialPortChange;

    }
}