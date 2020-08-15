using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateRaceCard
{
    public partial class Form1 : Form
    {

        private int nDownloadCount;
        private bool JVOpenFlg;
        private clsCodeConv objCodeConv;
        private OperateForm cOperateForm;
        private ClassJVLink cJVLink;
        private Timer timer;
        private ClassLog cLog = new ClassLog();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cLog.writeLog("Form1_Load " + this.Text);

            cJVLink = new ClassJVLink(this);
            if (cJVLink.checkInit() != 0)
            {
                //return;
            }
            cOperateForm = new OperateForm(this);

        }

        private void mnuConfJV_Click(object sender, EventArgs e)
        {
            cLog.writeLog("mnuConfJV_Click");
            cJVLink.callMenu();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cLog.writeLog("button1_Click");
            //cOperateForm.readFolder();
            cOperateForm.readDate();
        }

        private void btnGetJVData_Click(object sender, EventArgs e)
        {
            cLog.writeLog("btnGetJVData_Click");

            if (this.textBox1.Text == "")
            {
                System.Media.SystemSounds.Asterisk.Play();
                MessageBox.Show("出馬表を格納しているフォルダを選択してください。");
                cOperateForm.enableButton();
                return;
            }

            clcRaceCard cRaceCard = new clcRaceCard(this);
            cRaceCard.update();

        }

        private void tmrDownload_Tick(object sender, EventArgs e)
        {

        }
    }
}
