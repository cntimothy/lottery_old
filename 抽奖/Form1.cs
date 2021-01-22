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
            this.comboBox1.Enabled = false;
            this.btnStop.Focus();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            keepRunning = false;
            //(new System.Threading.Tasks.Task(writeName)).Start();
            Thread thread = new Thread(new ThreadStart(writeName));
            thread.Start();
            this.btnStop.Enabled = false;
            this.comboBox1.Enabled = true;
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
            this.label1.Text = "";
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
            ADR0[1] = "特等奖 vivo Y30";
            ADR0[2] = "pic_0.jpg";
            ADR0[3] = 1;
            ADR0[4] = 3;
            ADt.Rows.Add(ADR0);

            DataRow ADR1 = ADt.NewRow();
            ADR1[0] = 1;
            ADR1[1] = "幸运奖";
            ADR1[2] = "pic_1.jpg";
            ADR1[3] = 1;
            ADR1[4] =30;
            ADt.Rows.Add(ADR1);

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
