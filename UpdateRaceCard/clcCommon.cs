using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateRaceCard
{
    public class clcCommon
    {
        private string sid = "Test";
        Form1 _form1;
        //AxJVDTLabLib.AxJVLink _axJVLink1;
        //private clsCodeConv objCodeConv;
        //private OperateForm cOperateForm;
        //private ClassLog cLog;
        int readcount = 0;
        int downloadcount = 0;

        public clcCommon(Form1 form1)
        //public clcCommon(AxJVDTLabLib.AxJVLink axJVLink1)
        {
            //_axJVLink1 = axJVLink1;
            _form1 = form1;
            //cOperateForm = new OperateForm(form1);
            //cLog = new ClassLog();
        }

        public int checkInit()
        {
            //cLog.writeLog("checkInit");

            int num = _form1.axJVLink1.JVInit(sid);
            if (num != 0)
            {
                MessageBox.Show("JVInit エラー コード：" + num + "：", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                //_form1.Cursor = Cursors.Default;
            }
            //this.objCodeConv = new clsCodeConv();
            //this.objCodeConv.FileName = System.Windows.Forms.Application.StartupPath + "\\CodeTable.csv";

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

        public bool isJVOpen(string dataspec, string strDate, int option)
        {
            int retJVOpen = _form1.axJVLink1.JVOpen(dataspec,
                    strDate + "000000", option,
                    ref readcount, ref downloadcount,
                    out string _);
            if (retJVOpen != 0)
            {
                //cLog.writeLog("[isJVOpen]JVOpen エラー：" +
                //    retJVOpen);
                return false;
            }
            if (readcount == 0)
            {
                //cLog.writeLog("[isJVOpen]readcount エラー：" +
                //    retJVOpen);
                return false;
            }
            //_form1.prgJVRead.Maximum = readcount;

            return true;
        }

        public bool isJVOpenReal(string dataspec, string strDate)
        {
            int retJVRTOpen = _form1.axJVLink1.JVRTOpen(dataspec, strDate);
            if (retJVRTOpen != 0)
            {
                //cLog.writeLog("[isJVOpenReal]JVOpen エラー：" +
                //    retJVRTOpen);
                return false;
            }

            return true;
        }

        public string loopJVRead(int size, int count, bool isProgress)
        {
            string buff;
            bool isLoopEnd = false;
            do
            {
                //System.Windows.Forms.Application.DoEvents();
                //buff = new string(char.MinValue, size);
                //filename = new string(char.MinValue, count);
                switch (_form1.axJVLink1.JVRead(
                    out buff,
                    out size,
                    out _))
                {
                    case -503:
                        //cLog.writeLog("[loopJVRead] case -503 " +
                        //    filename + "が存在しません。");
                        return "";
                    case -203:
                        //cLog.writeLog("[loopJVRead] case -203 " +
                        //    "JVOpen が行われていません。");
                        return "";
                    case -201:
                        //cLog.writeLog("[loopJVRead] case -201 " +
                        //    "JVInit が行われていません。");
                        return "";
                    case -3:
                        continue;
                    case -1:
                        if (isProgress)
                        {
                            if (_form1.prgJVRead.Value + 1 > _form1.prgJVRead.Maximum)
                                _form1.prgJVRead.Maximum = _form1.prgJVRead.Value + 1;
                            _form1.prgJVRead.Value++;
                        }
                        continue;
                    case 0:
                        return "END";
                    default:
                        isLoopEnd = true;
                        break;
                }
            }
            while (!isLoopEnd);
            return buff;
        }

        public void writeHaitouData(ClassCSV cCSV, JVData_Struct.JV_HR_PAY mHrData, long rowTarget)
        {
            int res;
            string tmp;

            // 単勝配当
            cCSV.setData(rowTarget - 1, 16,
                createPayData(mHrData.PayTansyo[0].Pay,
                mHrData.PayTansyo[0].Umaban));
            if (int.TryParse(mHrData.PayTansyo[1].Pay, out res))
            {
                cCSV.setData(rowTarget + 0, 16,
                createPayData(mHrData.PayTansyo[1].Pay,
                mHrData.PayTansyo[1].Umaban));
                cCSV.setData(rowTarget - 1, 17,
                createPayData(mHrData.PayTansyo[2].Pay,
                mHrData.PayTansyo[2].Umaban));
            }
            // 1着複勝配当
            cCSV.setData(rowTarget + 0, 17,
                createPayData(mHrData.PayFukusyo[0].Pay,
                mHrData.PayFukusyo[0].Umaban));
            cCSV.setData(rowTarget - 1, 18,
                createPayData(mHrData.PayFukusyo[1].Pay,
                mHrData.PayFukusyo[1].Umaban));
            cCSV.setData(rowTarget + 0, 18,
                createPayData(mHrData.PayFukusyo[2].Pay,
                mHrData.PayFukusyo[2].Umaban));
            if (int.TryParse(mHrData.PayFukusyo[3].Pay, out res))
            {
                cCSV.setData(rowTarget - 1, 19,
                createPayData(mHrData.PayFukusyo[3].Pay,
                mHrData.PayFukusyo[3].Umaban));
                cCSV.setData(rowTarget + 0, 19,
                createPayData(mHrData.PayFukusyo[4].Pay,
                mHrData.PayFukusyo[4].Umaban));
            }
            // 枠連配当
            tmp = "0" + mHrData.PayWakuren[0].Umaban.Substring(0, 1) +
                "0" + mHrData.PayWakuren[0].Umaban.Substring(1, 1);
            cCSV.setData(rowTarget - 1, 20,
                createPayData(mHrData.PayWakuren[0].Pay, tmp));
            if (int.TryParse(mHrData.PayWakuren[1].Pay, out res))
            {
                tmp = "0" + mHrData.PayWakuren[1].Umaban.Substring(0, 1) +
                "0" + mHrData.PayWakuren[1].Umaban.Substring(1, 1);
                cCSV.setData(rowTarget + 0, 20,
                createPayData(mHrData.PayWakuren[1].Pay, tmp));
                tmp = "0" + mHrData.PayWakuren[2].Umaban.Substring(0, 1) +
                "0" + mHrData.PayWakuren[2].Umaban.Substring(1, 1);
                cCSV.setData(rowTarget - 1, 21,
                createPayData(mHrData.PayWakuren[2].Pay, tmp));
            }
            // 馬連配当
            cCSV.setData(rowTarget + 0, 21,
                createPayData(mHrData.PayUmaren[0].Pay,
                mHrData.PayUmaren[0].Kumi));
            if (int.TryParse(mHrData.PayUmaren[1].Pay, out res))
            {
                cCSV.setData(rowTarget - 1, 22,
                createPayData(mHrData.PayUmaren[1].Pay,
                mHrData.PayUmaren[1].Kumi));
                cCSV.setData(rowTarget + 0, 22,
                createPayData(mHrData.PayUmaren[2].Pay,
                mHrData.PayUmaren[2].Kumi));
            }
            // 馬単配当
            cCSV.setData(rowTarget - 1, 23,
                createPayData(mHrData.PayUmatan[0].Pay,
                mHrData.PayUmatan[0].Kumi));
            if (int.TryParse(mHrData.PayUmatan[1].Pay, out res))
            {
                cCSV.setData(rowTarget + 0, 23,
                createPayData(mHrData.PayUmatan[1].Pay,
                mHrData.PayUmatan[1].Kumi));
                cCSV.setData(rowTarget - 1, 24,
                createPayData(mHrData.PayUmatan[2].Pay,
                mHrData.PayUmatan[2].Kumi));
                cCSV.setData(rowTarget + 0, 24,
                createPayData(mHrData.PayUmatan[3].Pay,
                mHrData.PayUmatan[3].Kumi));
                cCSV.setData(rowTarget - 1, 25,
                createPayData(mHrData.PayUmatan[4].Pay,
                mHrData.PayUmatan[4].Kumi));
                cCSV.setData(rowTarget - 1, 25,
                createPayData(mHrData.PayUmatan[5].Pay,
                mHrData.PayUmatan[5].Kumi));
            }
            // 3連複配当
            cCSV.setData(rowTarget - 1, 26,
                createPayData(mHrData.PaySanrenpuku[0].Pay,
                mHrData.PaySanrenpuku[0].Kumi));
            if (int.TryParse(mHrData.PaySanrenpuku[1].Pay, out res))
            {
                cCSV.setData(rowTarget + 0, 26,
                createPayData(mHrData.PaySanrenpuku[1].Pay,
                mHrData.PaySanrenpuku[1].Kumi));
                cCSV.setData(rowTarget - 1, 27,
                createPayData(mHrData.PaySanrenpuku[2].Pay,
                mHrData.PaySanrenpuku[2].Kumi));
            }
            // 3連単配当
            cCSV.setData(rowTarget + 0, 27,
                createPayData(mHrData.PaySanrentan[0].Pay,
                mHrData.PaySanrentan[0].Kumi));
            if (int.TryParse(mHrData.PaySanrentan[1].Pay, out res))
            {
                cCSV.setData(rowTarget - 1, 28,
                createPayData(mHrData.PaySanrentan[1].Pay,
                mHrData.PaySanrentan[1].Kumi));
                cCSV.setData(rowTarget + 0, 28,
                createPayData(mHrData.PaySanrentan[2].Pay,
                mHrData.PaySanrentan[2].Kumi));
                cCSV.setData(rowTarget - 1, 29,
                createPayData(mHrData.PaySanrentan[3].Pay,
                mHrData.PaySanrentan[3].Kumi));
                cCSV.setData(rowTarget - 1, 29,
                createPayData(mHrData.PaySanrentan[4].Pay,
                mHrData.PaySanrentan[4].Kumi));
                cCSV.setData(rowTarget + 0, 30,
                createPayData(mHrData.PaySanrentan[5].Pay,
                mHrData.PaySanrentan[5].Kumi));
            }
            // ワイド
            cCSV.setData(rowTarget + 0, 30,
                createPayData(mHrData.PayWide[0].Pay,
                mHrData.PayWide[0].Kumi));
            cCSV.setData(rowTarget - 1, 31,
                createPayData(mHrData.PayWide[1].Pay,
                mHrData.PayWide[1].Kumi));
            cCSV.setData(rowTarget + 0, 31,
                createPayData(mHrData.PayWide[2].Pay,
                mHrData.PayWide[2].Kumi));
            if (int.TryParse(mHrData.PayWide[3].Pay, out res))
            {
                cCSV.setData(rowTarget - 1, 32,
                createPayData(mHrData.PayWide[3].Pay,
                mHrData.PayWide[3].Kumi));
                cCSV.setData(rowTarget + 0, 32,
                createPayData(mHrData.PayWide[4].Pay,
                mHrData.PayWide[4].Kumi));
                cCSV.setData(rowTarget - 1, 33,
                createPayData(mHrData.PayWide[5].Pay,
                mHrData.PayWide[5].Kumi));
                cCSV.setData(rowTarget + 0, 33,
                createPayData(mHrData.PayWide[6].Pay,
                mHrData.PayWide[6].Kumi));
            }
        }

        public string JyoCord(string cvt)
        {
            string ret = "";
            if (cvt == "01")
                ret = "札幌";
            else if (cvt == "02")
                ret = "函館";
            else if (cvt == "03")
                ret = "福島";
            else if (cvt == "04")
                ret = "新潟";
            else if (cvt == "05")
                ret = "東京";
            else if (cvt == "06")
                ret = "中山";
            else if (cvt == "07")
                ret = "中京";
            else if (cvt == "08")
                ret = "京都";
            else if (cvt == "09")
                ret = "阪神";
            else if (cvt == "10")
                ret = "小倉";
            else if (cvt == "30")
                ret = "門別";
            else if (cvt == "31")
                ret = "北見";
            else if (cvt == "32")
                ret = "岩見沢";
            else if (cvt == "33")
                ret = "帯広";
            else if (cvt == "34")
                ret = "旭川";
            else if (cvt == "35")
                ret = "盛岡";
            else if (cvt == "36")
                ret = "水沢";
            else if (cvt == "37")
                ret = "上山";
            else if (cvt == "38")
                ret = "三条";
            else if (cvt == "39")
                ret = "足利";
            else if (cvt == "40")
                ret = "宇都宮";
            else if (cvt == "41")
                ret = "高崎";
            else if (cvt == "42")
                ret = "浦和";
            else if (cvt == "43")
                ret = "船橋";
            else if (cvt == "44")
                ret = "大井";
            else if (cvt == "45")
                ret = "川崎";
            else if (cvt == "46")
                ret = "金沢";
            else if (cvt == "47")
                ret = "笠松";
            else if (cvt == "48")
                ret = "名古屋";
            else if (cvt == "49")
                ret = "紀伊三井寺";
            else if (cvt == "50")
                ret = "園田";
            else if (cvt == "51")
                ret = "姫路";
            else if (cvt == "52")
                ret = "益田";
            else if (cvt == "53")
                ret = "福山";
            else if (cvt == "54")
                ret = "高知";
            else if (cvt == "55")
                ret = "佐賀";
            else if (cvt == "56")
                ret = "荒尾";
            else if (cvt == "57")
                ret = "中津";
            else if (cvt == "58")
                ret = "札幌(地方)";
            else if (cvt == "59")
                ret = "函館(地方)";
            else if (cvt == "60")
                ret = "新潟(地方)";
            else if (cvt == "61")
                ret = "中京(地方)";

            return ret;
        }

        public string JyogyakuCord(string cvt)
        {
            string ret = "";
            if (cvt == "札幌")
                ret = "01";
            else if (cvt == "函館")
                ret = "02";
            else if (cvt == "福島")
                ret = "03";
            else if (cvt == "新潟")
                ret = "04";
            else if (cvt == "東京")
                ret = "05";
            else if (cvt == "中山")
                ret = "06";
            else if (cvt == "中京")
                ret = "07";
            else if (cvt == "京都")
                ret = "08";
            else if (cvt == "阪神")
                ret = "09";
            else if (cvt == "小倉")
                ret = "10";
            else if (cvt == "30")
                ret = "門別";
            else if (cvt == "31")
                ret = "北見";
            else if (cvt == "32")
                ret = "岩見沢";
            else if (cvt == "33")
                ret = "帯広";
            else if (cvt == "34")
                ret = "旭川";
            else if (cvt == "35")
                ret = "盛岡";
            else if (cvt == "36")
                ret = "水沢";
            else if (cvt == "37")
                ret = "上山";
            else if (cvt == "38")
                ret = "三条";
            else if (cvt == "39")
                ret = "足利";
            else if (cvt == "40")
                ret = "宇都宮";
            else if (cvt == "41")
                ret = "高崎";
            else if (cvt == "42")
                ret = "浦和";
            else if (cvt == "43")
                ret = "船橋";
            else if (cvt == "44")
                ret = "大井";
            else if (cvt == "45")
                ret = "川崎";
            else if (cvt == "46")
                ret = "金沢";
            else if (cvt == "47")
                ret = "笠松";
            else if (cvt == "48")
                ret = "名古屋";
            else if (cvt == "49")
                ret = "紀伊三井寺";
            else if (cvt == "50")
                ret = "園田";
            else if (cvt == "51")
                ret = "姫路";
            else if (cvt == "52")
                ret = "益田";
            else if (cvt == "53")
                ret = "福山";
            else if (cvt == "54")
                ret = "高知";
            else if (cvt == "55")
                ret = "佐賀";
            else if (cvt == "56")
                ret = "荒尾";
            else if (cvt == "57")
                ret = "中津";
            else if (cvt == "58")
                ret = "札幌(地方)";
            else if (cvt == "59")
                ret = "函館(地方)";
            else if (cvt == "60")
                ret = "新潟(地方)";
            else if (cvt == "61")
                ret = "中京(地方)";

            return ret;
        }

        public string Jyo2ShortJyo(string cvt)
        {
            string ret = "";
            if (cvt == "札幌")
                ret = "札";
            else if (cvt == "函館")
                ret = "函";
            else if (cvt == "福島")
                ret = "福";
            else if (cvt == "新潟")
                ret = "新";
            else if (cvt == "東京")
                ret = "東";
            else if (cvt == "中山")
                ret = "中";
            else if (cvt == "中京")
                ret = "名";
            else if (cvt == "京都")
                ret = "京";
            else if (cvt == "阪神")
                ret = "阪";
            else if (cvt == "小倉")
                ret = "小";

            return ret;
        }

        public string ShortJyo2Jyo(string cvt)
        {
            string ret = "";
            if (cvt == "札")
                ret = "札幌";
            else if (cvt == "函")
                ret = "函館";
            else if (cvt == "福")
                ret = "福島";
            else if (cvt == "新")
                ret = "新潟";
            else if (cvt == "東")
                ret = "東京";
            else if (cvt == "中")
                ret = "中山";
            else if (cvt == "名")
                ret = "中京";
            else if (cvt == "京")
                ret = "京都";
            else if (cvt == "阪")
                ret = "阪神";
            else if (cvt == "小")
                ret = "小倉";

            return ret;
        }

        public string TenkoCord(string cvt)
        {
            string ret = "";
            if (cvt == "0")
                ret = "未設定";
            else if (cvt == "1")
                ret = "晴";
            else if (cvt == "2")
                ret = "曇";
            else if (cvt == "3")
                ret = "雨";
            else if (cvt == "4")
                ret = "小雨";
            else if (cvt == "5")
                ret = "雪";
            else if (cvt == "6")
                ret = "小雪";
            return ret;
        }

        public string BabaCord(string cvt)
        {
            string ret = "";
            if (cvt == "0")
                ret = "未設定";
            else if (cvt == "1")
                ret = "良";
            else if (cvt == "2")
                ret = "稍重";
            else if (cvt == "3")
                ret = "重";
            else if (cvt == "4")
                ret = "不良";
            return ret;
        }

        public string createPayData(string strPay, string strKumi)
        {
            string ret = "";
            string tmpstrKumi = "";
            if (strPay.Length == 0)
                return ret;
            if (strPay.Replace(" ", "") == "")
                return ret;
            if (strKumi.Length >= 6)
            {
                tmpstrKumi = strKumi.Substring(0, 2) + "・" +
                    strKumi.Substring(2, 2) + "・" +
                    strKumi.Substring(4, 2);
            }
            else if (strKumi.Length >= 4)
            {
                tmpstrKumi = strKumi.Substring(0, 2) + "・" +
                    strKumi.Substring(2, 2);
            }
            else
            {
                tmpstrKumi = strKumi;
            }
            ret = int.Parse(strPay) + "(" + tmpstrKumi + ")";
            return ret;
        }

    }
}
