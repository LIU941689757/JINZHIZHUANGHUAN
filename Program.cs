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
        /// 对传入的IP地址4段进行加密：
        /// 每段IP加上当日日期（MMdd格式，如0620），然后转为八进制，返回结果
        /// </summary>
        /// <param name="ipParts">长度为4的IP地址整数数组</param>
        /// <returns>加密后的4个八进制整数</returns>
        public static int[] Encryption(int[] ipParts)
        {
            int dateInt = int.Parse(DateTime.Now.ToString("MMdd"));
            int[] encrypted = new int[4];

            for (int i = 0; i < 4; i++)
            {
                int added = ipParts[i] + dateInt;
                string octStr = Convert.ToString(added, 8);
                encrypted[i] = int.Parse(octStr);

                // 调试输出
                MessageBox.Show($"段{i + 1}: 原={ipParts[i]}, 加后={added}, 八进制={octStr}, 最终={encrypted[i]}");
            }

            return encrypted;
        }

    }
}
