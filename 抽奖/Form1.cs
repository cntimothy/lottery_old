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
        Dictionary<int, Person> nameList; // 人员的总名单
        List<int> passby = new List<int>(); // 已中奖人员序号列表
        List<int> cur; // 当前随机得到的人员序号列表
        int curRound; // 当前的组数
        string curPride; // 当前奖项名称
        int count; // 每组抽奖数
        int round; // 一共多少组
        bool flag;

        public Form1()
        {
            InitializeComponent();
            loadComboBox();
            this.btnStart.Focus();
            nameList = NameGenerator.ReadFile();

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
            flag = true;
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
            flag = false;
            //(new System.Threading.Tasks.Task(writeName)).Start();
            Thread thread = new Thread(new ThreadStart(writeName));
            thread.Start(); 
            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;
            this.btnStart.Focus();
            if (curRound == round) 
            {
                btnStart.Enabled = false;
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
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 将人员信息显示到UI
        /// </summary>
        private void showName()
        {
            while (flag)
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
                text =  "--"+ curPride + "(" + curRound + "/" + round + "):\r\n" + this.label1.Text;
            }));

            passby.AddRange(cur);
            writeLog(text);
        }

        /// <summary>
        /// 填充ComboBox的数据
        /// </summary>
        private void loadComboBox()
        {
            DataTable ADt = new DataTable();
            DataColumn ADC1 = new DataColumn("F_ID", typeof(int));
            DataColumn ADC2 = new DataColumn("F_Name", typeof(string));
            ADt.Columns.Add(ADC1);
            ADt.Columns.Add(ADC2);
            DataRow ADR0 = ADt.NewRow();
            ADR0[0] = 0;
            ADR0[1] = "阳光普照奖";
            ADt.Rows.Add(ADR0);

            DataRow ADR1 = ADt.NewRow();
            ADR1[0] = 1;
            ADR1[1] = "部门三等奖";
            ADt.Rows.Add(ADR1);

            DataRow ADR2 = ADt.NewRow();
            ADR2[0] = 2;
            ADR2[1] = "部门二等奖";
            ADt.Rows.Add(ADR2);

            DataRow ADR3 = ADt.NewRow();
            ADR3[0] = 3;
            ADR3[1] = "部门一等奖";
            ADt.Rows.Add(ADR3);

            DataRow ADR4 = ADt.NewRow();
            ADR4[0] = 4;
            ADR4[1] = "部门特别奖";
            ADt.Rows.Add(ADR4);

            DataRow ADR5 = ADt.NewRow();
            ADR5[0] = 5;
            ADR5[1] = "工会赞助三等奖";
            ADt.Rows.Add(ADR5);

            DataRow ADR6 = ADt.NewRow();
            ADR6[0] = 6;
            ADR6[1] = "工会赞助二等奖";
            ADt.Rows.Add(ADR6);

            DataRow ADR7 = ADt.NewRow();
            ADR7[0] = 7;
            ADR7[1] = "工会赞助一等奖";
            ADt.Rows.Add(ADR7);

            DataRow ADR8 = ADt.NewRow();
            ADR8[0] = 8;
            ADR8[1] = "总经理特别奖";
            ADt.Rows.Add(ADR8);

            DataRow ADR9 = ADt.NewRow();
            ADR9[0] = 9;
            ADR9[1] = "中心赞助奖";
            ADt.Rows.Add(ADR9);

            DataRow ADR10 = ADt.NewRow();
            ADR10[0] = 10;
            ADR10[1] = "抽奖一";
            ADt.Rows.Add(ADR10);

            DataRow ADR11 = ADt.NewRow();
            ADR11[0] = 11;
            ADR11[1] = "抽奖二";
            ADt.Rows.Add(ADR11);
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
            if ((int)this.comboBox1.SelectedValue == 0)
            {
                count = 10;
                round = 8;
            }
            if ((int)this.comboBox1.SelectedValue == 1)
            {
                count = 5;
                round = 10;
            }
            if ((int)this.comboBox1.SelectedValue == 2)
            {
                count = 10;
                round = 2;
            }
            if ((int)this.comboBox1.SelectedValue == 3)
            {
                count = 5;
                round = 2;
            }
            if ((int)this.comboBox1.SelectedValue == 4)
            {
                count = 1;
                round = 5;
            }
            if ((int)this.comboBox1.SelectedValue == 5)
            {
                count = 8;
                round = 3;
            }
            if ((int)this.comboBox1.SelectedValue == 6)
            {
                count = 9;
                round = 2;
            }
            if ((int)this.comboBox1.SelectedValue == 7)
            {
                count = 4;
                round = 3;
            }
            if ((int)this.comboBox1.SelectedValue == 8)
            {
                count = 1;
                round = 6;
            }
            if ((int)this.comboBox1.SelectedValue == 9)
            {
                count = 1;
                round = 30;
            }
            if ((int)this.comboBox1.SelectedValue == 10)
            {
                count = 1;
                round = 20;
            }
            if ((int)this.comboBox1.SelectedValue == 11)
            {
                count = 1;
                round = 20;
            }
            curRound = 0;
            curPride = (string)this.comboBox1.Text;
        }
        #endregion
    }
}
