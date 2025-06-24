using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace JINZHIZHUANGHUAN
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // 启用可视样式和兼容文本渲染，启动主窗体
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1());
            //int[] ip = GetIp();
            //Console.WriteLine("原始IP段：");
            //Console.WriteLine(string.Join(".", ip));

            //int[] encrypted = Encryption(ip);
            //Console.WriteLine("加密后的八进制结果：");
            //Console.WriteLine(string.Join(".", encrypted));

            //Console.WriteLine("按任意键退出...");
            //Console.ReadKey();
        }

        /// <summary>
        /// 获取本机 IPv4 地址，并将其以点分隔的4段数字形式返回
        /// 例如：192.168.1.10 → 返回 [192, 168, 1, 10]
        /// </summary>
        /// <returns>长度为4的 int 数组，表示IP的四段</returns>
        public static int[] GetIp()
        {
            // 获取主机名对应的所有IP地址
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                // 仅处理 IPv4 地址（InterNetwork）
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string[] parts = ip.ToString().Split('.');
                    if (parts.Length == 4)
                    {
                        // 将每段字符串转换为整数返回
                        return parts.Select(p => int.Parse(p)).ToArray();
                    }
                }
            }

            // 未找到有效IPv4地址，返回默认值
            return new int[] { 0, 0, 0, 0 };
        }

        /// <summary>
        /// 对传入的 IP 地址 4 段进行简单加密：
        /// 每段加上当天日期（MMdd 格式，如 0620），再将结果转换为八进制字符串形式，作为混淆处理。
        /// </summary>
        /// <param name="ipParts">长度为 4 的 IP 地址整数数组（例如 [192, 168, 0, 1]）</param>
        /// <returns>每段转换后的八进制字符串数组</returns>
        public static string[] Encryption(int[] ipParts)
        {
            // 获取当前日期（MMdd 形式），例如 6月20日 → "0620" → 620
            int dateInt = int.Parse(DateTime.Now.ToString("MMdd"));

            // 初始化结果数组，保存每段加密后的八进制字符串
            string[] encrypted = new string[4];

            for (int i = 0; i < 4; i++)
            {
                // 当前 IP 段加上日期偏移
                int added = ipParts[i] + dateInt;

                // 转为八进制字符串（如 "1474"），作为混淆后的结果
                string octStr = Convert.ToString(added, 8);

                // 存入结果数组
                encrypted[i] = octStr;
            }

            // 返回加密结果
            return encrypted;
        }

        public static string Encryption1(int[] ipParts)
        {
            int dateOffset = int.Parse(DateTime.Now.ToString("MMdd"));
            return string.Concat(ipParts.Select(p => Convert.ToString(p + dateOffset, 8)));
        }


        public static int[] GetIPv4_NetworkInterface()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;

                foreach (var addr in ni.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return addr.Address.ToString().Split('.').Select(int.Parse).ToArray();
                    }
                }
            }
            return null;
        }

        public static int[] GetIPv4_Dns()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var ip in addresses)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString().Split('.').Select(int.Parse).ToArray();
                }
            }
            return null;
        }


        public static int[] GetIPv4_Socket()//正在工作的网卡
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                try
                {
                    socket.Connect("8.8.8.8", 65530);
                    var localIP = ((IPEndPoint)socket.LocalEndPoint).Address;
                    return localIP.ToString().Split('.').Select(int.Parse).ToArray();
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// 这个方法只会获取状态为 OperationalStatus.Up（已启用且正在运行）的网卡的 IPv4 地址，并且过滤了如下情况：
        ///排除回环接口（Loopback）
        ///排除隧道接口（Tunnel）
        ///排除未知类型（Unknown）
        ///排除无效的自动私有地址（169.254.x.x）
        /// </summary>
        /// <returns></returns>
        public static int[] GetActiveIPv4_FromNetworkInterface()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 1. 网卡必须启用且正在运行
                if (ni.OperationalStatus != OperationalStatus.Up) continue;

                // 2. 过滤掉虚拟网卡、Loopback、Tunnel、Unknown 类型
                //if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                //    ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                //    ni.NetworkInterfaceType == NetworkInterfaceType.Unknown)
                //    continue;

                // 3. 获取 IPv4 地址
                var ipProps = ni.GetIPProperties();
                foreach (var addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ipString = addr.Address.ToString();

                        // 4. 排除 APIPA 地址（如 169.254.x.x）
                        //if (ipString.StartsWith("169.254")) continue;

                        // ✅ 返回有效 IPv4 地址数组
                        return ipString.Split('.').Select(int.Parse).ToArray();
                    }
                }
            }

            // ❌ 找不到合适地址时
            return null;
        }
        /// <summary>
        /// EXCEL解密 C3到F3的格子
        /// =IF(C3="", "", OCT2DEC(C3) - (MONTH(TODAY())*100 + DAY(TODAY())))    
    }
}
