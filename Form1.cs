using System;
using System.Windows.Forms;

namespace JINZHIZHUANGHUAN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();  // 初始化界面控件

            // 注册窗体加载事件，窗体显示前会触发此事件
            this.Load += Form1_Load;
        }

        /// <summary>
        /// 窗体加载完成时自动执行：获取IP、加密，并显示到文本框
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. 获取本机IPv4地址段（返回4个int）
                int[] ip = Program.GetIp();
                MessageBox.Show("获取到的IP是：" + string.Join(".", ip));
                // 2. 加密处理：每段IP加上日期并转成八进制
                int[] encrypted = Program.Encryption(ip);

                // 3. 将加密结果填入界面上的四个文本框
                textBox1.Text = encrypted[0].ToString();
                textBox2.Text = encrypted[1].ToString();
                textBox3.Text = encrypted[2].ToString();
                textBox4.Text = encrypted[3].ToString();
            }
            catch (Exception ex)
            {
                // 如果处理过程中出错，弹出错误提示
                MessageBox.Show("初始化加密失败：" + ex.Message,
                                "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
