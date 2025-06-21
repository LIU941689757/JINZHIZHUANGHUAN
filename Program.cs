using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Windows.Forms;

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



    }
}
