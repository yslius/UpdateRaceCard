using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.IO;

namespace UpdateRaceCard
{
    class ClassJVLink
    {
        Form1 _form1;
        private clsCodeConv objCodeConv;
        private string sid = "Test";
        private ClassLog cLog = new ClassLog();
        private OperateForm cOperateForm;
        private int nDownloadCount;
        private bool JVOpenFlg;
        string dataspec;
        int size = 0;
        int count = 0;
        int option = 0;
        int readcount = 0;
        int downloadcount = 0;

        public ClassJVLink(Form1 form1)
        {
            _form1 = form1;
            cOperateForm = new OperateForm(form1);
        }

        public int checkInit()
        {
            cLog.writeLog("checkInit");

            int num = _form1.axJVLink1.JVInit(sid);
            if (num != 0)
            {
                MessageBox.Show("JVInit エラー コード：" + num + "：", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                _form1.Cursor = Cursors.Default;
            }
            this.objCodeConv = new clsCodeConv();
            this.objCodeConv.FileName = System.Windows.Forms.Application.StartupPath + "\\CodeTable.csv";

            return num;
        }

        public void callMenu()
        {
            try
            {
                int nReturnCode = _form1.axJVLink1.JVSetUIProperties();
                if (nReturnCode != 0)
                {
                    MessageBox.Show("JVSetUIPropertiesエラー コード：" +
                        nReturnCode + "：", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }


        private bool isRunRaceReal(DateTime datetimeTarg)
        {
            string dataspec = "0B14";
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
            string strDate =
                (datetimeTarg - timeSpan).ToString("yyyyMMdd");
            int num1 = _form1.axJVLink1.JVRTOpen(dataspec, strDate);
            int num2 = _form1.axJVLink1.JVClose();
            if (num2 != 0)
            {
                MessageBox.Show("JVClose エラー：" + num2);
            }
            if (num1 != 0)
            {
                return false;
            }
            return true;
        }

        bool isJVOpen(string strDate, int option)
        {
            int retJVOpen = _form1.axJVLink1.JVOpen(dataspec,
                    strDate + "000000", option,
                    ref readcount, ref downloadcount,
                    out string _);
            if (retJVOpen != 0)
            {
                cLog.writeLog("[isJVOpen]JVOpen エラー：" +
                    retJVOpen);
                return false;
            }
            if (readcount == 0)
            {
                cLog.writeLog("[isJVOpen]readcount エラー：" +
                    retJVOpen);
                return false;
            }
            return true;
        }

        bool isJVOpenReal(string strDate)
        {
            int retJVRTOpen = _form1.axJVLink1.JVRTOpen(dataspec, strDate);
            if (retJVRTOpen != 0)
            {
                cLog.writeLog("[isJVOpenReal]JVOpen エラー：" +
                    retJVRTOpen);
                return false;
            }
            if (readcount == 0)
            {
                cLog.writeLog("[isJVOpenReal]readcount エラー：" +
                    retJVRTOpen);
                return false;
            }
            return true;
        }

        string loopJVRead()
        {
            string buff;
            string filename;
            bool isLoopEnd = false;
            do
            {
                System.Windows.Forms.Application.DoEvents();
                buff = new string(char.MinValue, size);
                filename = new string(char.MinValue, count);
                switch (_form1.axJVLink1.JVRead(
                    out buff,
                    out size,
                    out filename))
                {
                    case -503:
                        cLog.writeLog("[loopJVRead] case -503 " +
                            filename + "が存在しません。");
                        return "";
                    case -203:
                        cLog.writeLog("[loopJVRead] case -203 " +
                            "JVOpen が行われていません。");
                        return "";
                    case -201:
                        cLog.writeLog("[loopJVRead] case -201 " +
                            "JVInit が行われていません。");
                        return "";
                    case -3:
                        continue;
                    case -1:
                        ++_form1.prgJVRead.Value;
                        continue;
                    case 0:
                        _form1.prgJVRead.Value =
                            _form1.prgJVRead.Maximum;
                        isLoopEnd = true;
                        return "END";
                    default:
                        isLoopEnd = true;
                        break;
                }
            }
            while (!isLoopEnd);
            return buff;
        }

        

    }
}
