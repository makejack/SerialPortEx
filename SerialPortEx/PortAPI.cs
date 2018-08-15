using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SerialPortEx
{
    public class PortAPI
    {
        /// <summary>
        /// 波特率
        /// </summary>
        public enum BaudRates
        {
            B50 = 0x00,
            B75 = 0x01,
            B110 = 0x02,
            B134 = 0x03,
            B150 = 0x04,
            B300 = 0x05,
            B600 = 0x06,
            B1200 = 0x07,
            B1800 = 0x08,
            B2400 = 0x09,
            B4800 = 0x0A,
            B7200 = 0x0B,
            B9600 = 0x0C,
            B19200 = 0x0D,
            B38400 = 0x0E,
            B57600 = 0x0F,
            B115200 = 0x10,
            B230400 = 0x11,
            B460800 = 0x12,
            B921600 = 0x13,
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public enum StopBits
        {
            STOP_1 = 0x00,
            STOP_2 = 0x04,
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public enum DataBits
        {
            BIT_5 = 0x00,
            BIT_6 = 0x01,
            BIT_7 = 0x02,
            Bit_8 = 0x03,
        }

        /// <summary>
        /// 检验位
        /// </summary>
        public enum Paritys
        {
            NONE = 0x00,
            ODD = 0x08,
            SPC = 0x38,
            MRK = 0x28,
            EVEN = 0x18
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public enum ErrorCodes
        {
            /// <summary>
            /// 正确
            /// </summary>
            SIO_OK = 0,
            /// <summary>
            /// 没有此端口或端口未打开
            /// </summary>
            SIO_BADPORT = -1,
            /// <summary>
            /// 无法控制此板
            /// </summary>
            SIO_OUTCONTROL = -2,
            /// <summary>
            /// 没有数据代读取或没有缓冲区供写
            /// </summary>
            SIO_NODATA = -4,
            /// <summary>
            /// 没有此端口或端口已打开
            /// </summary>
            SIO_OPENFAIL = -5,
            /// <summary>
            /// 因为H/W流量控制而不能设置
            /// </summary>
            SIO_RTS_BY_HW = -6,
            /// <summary>
            /// 无效参数
            /// </summary>
            SIO_BADPARM = -7,
            /// <summary>
            /// 调用win32函数失败请调用GetLastError函数以获取错误代码
            /// </summary>
            SIO_WIN32FAIL = -8,
            /// <summary>
            /// 此版本不支持这个函数
            /// </summary>
            SIO_BOARDNOTSUPPORT = -9,
            /// <summary>
            /// PCOMM函数运行结果失败
            /// </summary>
            SIO_FAIL = -10,
            /// <summary>
            /// 写入已被锁定，用户已放弃写入
            /// </summary>
            SIO_ABORT_WRITE = -11,
            /// <summary>
            /// 已发生写入超时
            /// </summary>
            SIO_WRITETIMEOUT = -12,
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="port">端口编号 COM1 - 1 </param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_open(int port);

        /// <summary>
        /// 设置端口波特率、数据位、停止位、检验位
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="mode">数据位|停止位|检验位 - 格式</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_ioctl(int port, int baudRate, int mode);

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_close(int port);

        /// <summary>
        /// 读取端口数据
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="buffer">数据流</param>
        /// <param name="len">数据长度</param>
        /// <returns></returns>
        [DllImport("pcomm.dl")]
        public static extern int sio_read(int port, byte[] buffer, int len);

        /// <summary>
        /// 清除端口缓冲区数据
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="state">0 - 清除写入缓冲区数据  1 - 清除读缓冲区数据    2 - 清除读写缓冲区数据</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_flush(int port, int func);

        /// <summary>
        /// 向缓冲区写入数据
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="buffer">数据流</param>
        /// <param name="len">数据长度</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_write(int port, byte[] buffer, int len);

        /// <summary>
        /// 获取接收缓冲区的数据长度
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_iqueue(int port);

        /// <summary>
        /// 设置端口写入超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totaltimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值 默认0</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_SetWriteTimeouts(int port, int totalTimeouts, int intervalTimeouts);

        /// <summary>
        /// 获取端口写入超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totaltimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_GetWriteTimeouts(int port, out int totalTimeouts);

        /// <summary>
        /// 设置端口读取超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totalTimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值 默认0</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_SetReadTimeouts(int port, int totalTimeouts, int intervalTimeouts);

        /// <summary>
        /// 获取端口读取超时时间
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="totalTimeouts">总超时值</param>
        /// <param name="intervalTimeouts">间隔超时值</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_GetReadTimeouts(int port, out int totalTimeouts, out int intervalTimeouts);

        /// <summary>
        /// 当端口接收到数据时的回调方法
        /// </summary>
        /// <param name="port">端口编号</param>
        /// <param name="func">回调方法</param>
        /// <param name="count">端口接收到指定的数据长度后开始回调</param>
        /// <returns></returns>
        [DllImport("pcomm.dll")]
        public static extern int sio_cnt_irq(int port, DataReceivedEventHandler func, int count);

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="port"></param>
        public delegate void DataReceivedEventHandler(int port);
    }
}
