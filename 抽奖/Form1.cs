using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace 抽奖
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 人员的总名单
        /// </summary>
        Dictionary<int, Person> nameList;

        /// <summary>
        /// 已中奖人员序号列表
        /// </summary>
        List<int> passby = new List<int>();

        /// <summary>
        /// 当前随机得到的人员序号列表
        /// </summary>
        List<int> cur;

        /// <summary>
        /// 当前的组数
        /// </summary>
        int curRound;

        /// <summary>
        /// 当前奖项名称
        /// </summary>
        string curPride;

        /// <summary>
        /// 每组抽奖数
        /// </summary>
        int count;

        /// <summary>
        /// 一共多少组
        /// </summary>
        int round;

        /// <summary>
        /// 图片路径
        /// </summary>
        string picName;

        /// <summary>
        /// 是否循环标志位
        /// </summary>
        bool keepRunning;

        /// <summary>
        /// ComboBox的数据源
        /// </summary>
        DataTable ADt = new DataTable();

        public Form1()
        {
            InitializeComponent();
            loadComboBox();
            this.btnStart.Focus();
            nameList = NameGenerator.ReadFile();
            if (nameList == null)
            {
                DialogResult dr = MessageBox.Show("请将\"总名单.txt\"放入程序文件夹", "错误！", MessageBoxButtons.OK);
                if (dr == DialogResult.OK)
                {
                    System.Environment.Exit(0);
                }
            }

            List<Person> hitted = NameGenerator.ReadHittedNames();
            foreach (Person p in hitted)
            {
                for (int i = 0; i < nameList.Count; i++)
                {
                    if (p.Equals(nameList[i]))
                    {
                        passby.Add(i);
                        break;
                    }
                }
            }
        }

        #region Button Event
        private void btnStart_Click(object sender, EventArgs e)
        {
            keepRunning = true;
            this.curRound++;
            this.label2.Text = "获奖名单（" + curRound + "/" + round + "）";
            //(new System.Threading.Tasks.Task(showName)).Start();
            Thread thread = new Thread(new ThreadStart(showName));
            thread.Start();
            this.btnStop.Enabled = true;
            this.btnStart.Enabled = false;
            this.btnStop.Focus();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            keepRunning = false;
            //(new System.Threading.Tasks.Task(writeName)).Start();
            Thread thread = new Thread(new ThreadStart(writeName));
            thread.Start();
            this.btnStop.Enabled = false;
            if (curRound == round)
            {
                btnStart.Enabled = false;
            }
            else
            {
                this.btnStart.Enabled = true;
                this.btnStart.Focus();
            }
        }
        #endregion

        #region ComboBox Event
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadCountRound();
            this.label2.Text = "获奖名单（" + curRound + "/" + round + "）";
            this.btnStart.Enabled = true;
            this.btnStart.Focus();
            if (picName.Equals(""))
            {
                this.pbReward.Visible = false;
            }
            else
            {
                this.pbReward.Visible = true;
                this.pbReward.Image = Image.FromFile(@"pics\" + picName);
            }            
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 将人员信息显示到UI
        /// </summary>
        private void showName()
        {
            while (keepRunning)
            {
                cur = NameGenerator.getNames(nameList, count, passby);
                StringBuilder sb = new StringBuilder();
                foreach (int j in cur)
                {
                    sb.Append(nameList[j].ToString() + "\r\n");
                }
                this.Invoke(new Action(() =>
                {
                    this.label1.Text = sb.ToString();
                }));
                System.Threading.Thread.Sleep(50);
            }
        }

        private void writeName()
        {
            System.Threading.Thread.Sleep(100);
            String text = "";
            this.Invoke(new Action(() =>
            {
                text = "--" + curPride + "(" + curRound + "/" + round + "):\r\n" + this.label1.Text;
            }));

            passby.AddRange(cur);
            writeLog(text);
        }

        /// <summary>
        /// 填充ComboBox的数据
        /// </summary>
        private void loadComboBox()
        {
            DataColumn ADC1 = new DataColumn("F_ID", typeof(int));
            DataColumn ADC2 = new DataColumn("F_Name", typeof(string));
            DataColumn ADC3 = new DataColumn("F_Pic", typeof(string));
            DataColumn ADC4 = new DataColumn("F_Count", typeof(int));
            DataColumn ADC5 = new DataColumn("F_Round", typeof(int));
            ADt.Columns.Add(ADC1);
            ADt.Columns.Add(ADC2);
            ADt.Columns.Add(ADC3);
            ADt.Columns.Add(ADC4);
            ADt.Columns.Add(ADC5);
            DataRow ADR0 = ADt.NewRow();
            ADR0[0] = 0;
            ADR0[1] = "阳光普照奖(50元现金红包)";
            ADR0[2] = "pic_0.jpg";
            ADR0[3] = 10;
            ADR0[4] = 10;
            ADt.Rows.Add(ADR0);

            DataRow ADR1 = ADt.NewRow();
            ADR1[0] = 1;
            ADR1[1] = "部门三等奖（小米电水壶）";
            ADR1[2] = "pic_1.jpg";
            ADR1[3] = 10;
            ADR1[4] = 1;
            ADt.Rows.Add(ADR1);

            DataRow ADR2 = ADt.NewRow();
            ADR2[0] = 2;
            ADR2[1] = "部门三等奖（小米电动牙刷）";
            ADR2[2] = "pic_2.jpg";
            ADR2[3] = 10;
            ADR2[4] = 1;
            ADt.Rows.Add(ADR2);

            DataRow ADR3 = ADt.NewRow();
            ADR3[0] = 3;
            ADR3[1] = "部门三等奖（小米蓝牙项圈耳机）";
            ADR3[2] = "pic_3.jpg";
            ADR3[3] = 10;
            ADR3[4] = 1;
            ADt.Rows.Add(ADR3);

            DataRow ADR4 = ADt.NewRow();
            ADR4[0] = 4;
            ADR4[1] = "部门三等奖（纯棉四件套）";
            ADR4[2] = "pic_4.jpg";
            ADR4[3] = 6;
            ADR4[4] = 2;
            ADt.Rows.Add(ADR4);

            DataRow ADR5 = ADt.NewRow();
            ADR5[0] = 5;
            ADR5[1] = "部门三等奖（拜格锅具套装）";
            ADR5[2] = "pic_5.jpg";
            ADR5[3] = 8;
            ADR5[4] = 1;
            ADt.Rows.Add(ADR5);

            DataRow ADR6 = ADt.NewRow();
            ADR6[0] = 6;
            ADR6[1] = "部门二等奖（扫地机器人）";
            ADR6[2] = "pic_6.jpg";
            ADR6[3] = 5;
            ADR6[4] = 1;
            ADt.Rows.Add(ADR6);

            DataRow ADR7 = ADt.NewRow();
            ADR7[0] = 7;
            ADR7[1] = "部门二等奖（小熊加湿器）";
            ADR7[2] = "pic_7.jpg";
            ADR7[3] = 5;
            ADR7[4] = 1;
            ADt.Rows.Add(ADR7);

            DataRow ADR8 = ADt.NewRow();
            ADR8[0] = 8;
            ADR8[1] = "部门二等奖（小浣熊破壁机）";
            ADR8[2] = "pic_8.jpg";
            ADR8[3] = 5;
            ADR8[4] = 1;
            ADt.Rows.Add(ADR8);

            DataRow ADR9 = ADt.NewRow();
            ADR9[0] = 9;
            ADR9[1] = "部门二等奖（小爱同学智能音箱）";
            ADR9[2] = "pic_9.jpg";
            ADR9[3] = 5;
            ADR9[4] = 1;
            ADt.Rows.Add(ADR9);

            DataRow ADR10 = ADt.NewRow();
            ADR10[0] = 10;
            ADR10[1] = "工会二等奖（美的榨汁机）";
            ADR10[2] = "pic_10.jpg";
            ADR10[3] = 10;
            ADR10[4] = 3;
            ADt.Rows.Add(ADR10);

            DataRow ADR11 = ADt.NewRow();
            ADR11[0] = 11;
            ADR11[1] = "部门一等奖（长虹净化器）";
            ADR11[2] = "pic_11.jpg";
            ADR11[3] = 5;
            ADR11[4] = 1;
            ADt.Rows.Add(ADR11);

            DataRow ADR12 = ADt.NewRow();
            ADR12[0] = 12;
            ADR12[1] = "部门一等奖（倍轻松足疗机）";
            ADR12[2] = "pic_12.jpg";
            ADR12[3] = 5;
            ADR12[4] = 1;
            ADt.Rows.Add(ADR12);

            DataRow ADR13 = ADt.NewRow();
            ADR13[0] = 13;
            ADR13[1] = "工会一等奖（美的压力锅）";
            ADR13[2] = "pic_13.jpg";
            ADR13[3] = 20;
            ADR13[4] = 1;
            ADt.Rows.Add(ADR13);

            DataRow ADR14 = ADt.NewRow();
            ADR14[0] = 14;
            ADR14[1] = "部门特等奖（Vivo Y93手机）";
            ADR14[2] = "pic_14.jpg";
            ADR14[3] = 5;
            ADR14[4] = 1;
            ADt.Rows.Add(ADR14);

            DataRow ADR15 = ADt.NewRow();
            ADR15[0] = 15;
            ADR15[1] = "总经理特别奖（BOE睡眠仪）";
            ADR15[2] = "pic_15.jpg";
            ADR15[3] = 5;
            ADR15[4] = 1;
            ADt.Rows.Add(ADR15);

            DataRow ADR16 = ADt.NewRow();
            ADR16[0] = 16;
            ADR16[1] = "幸运奖";
            ADR16[2] = "";
            ADR16[3] = 30;
            ADR16[4] = 1;
            ADt.Rows.Add(ADR16);
            //进行绑定  
            comboBox1.DisplayMember = "F_Name";//控件显示的列名  
            comboBox1.ValueMember = "F_ID";//控件值的列名  
            comboBox1.DataSource = ADt;
        }

        /// <summary>
        /// 将抽奖结果写入日志
        /// </summary>
        /// <param name="content"></param>
        private void writeLog(String content)
        {
            StreamWriter sw = new StreamWriter("获奖人员名单.txt", true, Encoding.UTF8);
            sw.WriteLine("--" + DateTime.Now.ToString());
            sw.WriteLine(content);
            sw.Close();
        }

        /// <summary>
        /// 根据下拉列表的选择，设置抽奖每轮抽取人员数量和抽取的轮数
        /// </summary>
        private void loadCountRound()
        {
            int selectedValue = (int)(this.comboBox1.SelectedValue);
            count = Int32.Parse(ADt.Rows[selectedValue]["F_Count"].ToString());
            round = Int32.Parse(ADt.Rows[selectedValue]["F_Round"].ToString());
            picName = ADt.Rows[selectedValue]["F_Pic"].ToString();
            curRound = 0;
            curPride = (string)this.comboBox1.Text;
        }
        #endregion
    }
}
