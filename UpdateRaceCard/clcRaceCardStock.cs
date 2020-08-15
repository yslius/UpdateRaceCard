using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateRaceCard
{
    class classDM
    {
        public int Umaban;
        public int DMTime;

        public classDM(int umaban, int dmTime)
        {
            this.Umaban = umaban;
            this.DMTime = dmTime;
        }
    }
    class ClcRaceCardStock
    {
        private string sid = "Test";
        Form1 _form1;
        private clsCodeConv objCodeConv;

        private ClassLog cLog;
        int size = 0;
        int count = 0;
        clcCommon cCommon;

        public ClcRaceCardStock(Form1 form1)
        {
            _form1 = form1;
            cCommon = new clcCommon(_form1);
            cLog = new ClassLog();
            this.objCodeConv = new clsCodeConv();
        }

        public void GetStockDataDetailData(ClassCSV cCSV, DateTime datetimeTarg)
        {
            _form1.axJVLink1.JVClose();
            if (cCommon.checkInit() != 0)
                return;

            if (GetStockDataDetailData1(cCSV, datetimeTarg) < 0)
            {
                _form1.axJVLink1.JVClose();
                return;
            }
            _form1.prgDownload.Value = 51;
            _form1.prgDownload.Value--;
            if (GetStockDataDetailData2(cCSV, datetimeTarg) < 0)
            {
                _form1.axJVLink1.JVClose();
                return;
            }
            _form1.prgDownload.Maximum++;
            _form1.prgDownload.Value = _form1.prgDownload.Maximum;
            _form1.prgDownload.Maximum--;

        }


        int GetStockDataDetailData1(ClassCSV cCSV, DateTime datetimeTarg)
        {
            TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
            string strDate =
                (datetimeTarg - timeSpan).ToString("yyyyMMdd");
            bool isFind = false;
            string retbuff;
            long cntLoop = 0;

            size = 40000;
            count = 256;
            int option = DateTime.Now >
                datetimeTarg.AddYears(1) ? 4 : 1;

            //try
            //{
                
                _form1.tmrDownload.Enabled = false;
                _form1.prgJVRead.Value = 0;
                 if (!cCommon.isJVOpen("RACE", strDate, option))
                {
                    return -1;
                }
                do
                {
                    retbuff = cCommon.loopJVRead(size, count);
                    if (retbuff == "" || retbuff == "END")
                        break;
                    if (retbuff.Substring(0, 2) == "HR")
                    {
                        setDataHR(cCSV, retbuff, datetimeTarg);
                    }
                    if (retbuff.Substring(0, 2) == "O1")
                    {
                        setDataO1(cCSV, retbuff, datetimeTarg);
                    }
                    if (retbuff.Substring(0, 2) == "RA")
                    {
                        setDataRA(cCSV, retbuff, datetimeTarg);
                    }
                    if (retbuff.Substring(0, 2) == "SE")
                    {
                        if (setDataSE(cCSV, retbuff, datetimeTarg, isFind))
                            break;
                        isFind = true;
                    }
                    cntLoop++;
                }
                while (cntLoop <= 100000);
                _form1.prgJVRead.Maximum++;
                _form1.prgJVRead.Value =
                    _form1.prgJVRead.Maximum;
                _form1.prgJVRead.Maximum--;
                _form1.prgJVRead.Value--;

            if (!isFind)
                {
                    cLog.writeLog("[GetStockDataDetailData1]Not Find：" +
                    cntLoop);
                }

            //}
            //catch (Exception ex)
            //{
            //    cLog.writeLog("[GetStockDataDetailData1]エラー：" +
            //        ex.Message);
            //}

            int retJVClose = _form1.axJVLink1.JVClose();
            if (retJVClose != 0)
            {
                cLog.writeLog("[GetStockDataDetailData1]JVClose エラー：" +
                    retJVClose);
            }
            return 1;
        }

        int GetStockDataDetailData2(ClassCSV cCSV, DateTime datetimeTarg)
        {
            
            List<string> stringList = new List<string>();
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
            string strDate =
                (datetimeTarg - timeSpan).ToString("yyyyMMdd");
            bool isFind = false;
            string retbuff;
            long cntLoop = 0;
            size = 40000;
            count = 256;
            int option = DateTime.Now >
                datetimeTarg.AddYears(1) ? 4 : 1;

            try
            {
                _form1.tmrDownload.Enabled = false;
                _form1.prgJVRead.Value = 0;
                if (!cCommon.isJVOpen("MING", strDate, option))
                {
                    return -1;
                }
                do
                {
                    retbuff = cCommon.loopJVRead(size, count);
                    if (retbuff == "" || retbuff == "END")
                        break;
                    if (retbuff.Substring(0, 2) == "DM")
                    {
                        if (setDataDM(cCSV, retbuff, datetimeTarg, isFind))
                            break;
                        isFind = true;
                    }
                    
                    cntLoop++;
                }
                while (cntLoop <= 10000);
                _form1.prgJVRead.Maximum++;
                _form1.prgJVRead.Value =
                    _form1.prgJVRead.Maximum;
                _form1.prgJVRead.Maximum--;

                if (!isFind)
                {
                    cLog.writeLog("[GetStockDataDetailData2]Not Find：" +
                    cntLoop);
                }

            }
            catch (Exception ex)
            {
                cLog.writeLog("[GetStockDataDetailData2]エラー：" +
                    ex.Message);
            }

            int retJVClose = _form1.axJVLink1.JVClose();
            if (retJVClose != 0)
            {
                cLog.writeLog("[GetStockDataDetailData2]JVClose エラー：" +
                    retJVClose);
            }
            return 1;
        }

        bool setDataDM(ClassCSV cCSV, string retbuff, DateTime datetimeTarg, bool isFind)
        {
            List<classDM> listDM = new List<classDM>();

            DateTime dateTime;
            string strShortJyo;
            string strJyo;
            long rowTarget;
            int numUma;
            int cnt = 0;

            JVData_Struct.JV_DM_INFO mDMData =
                new JVData_Struct.JV_DM_INFO();
            mDMData.SetDataB(ref retbuff);
            dateTime = DateTime.Parse(
            (mDMData.id.Year +
            mDMData.id.MonthDay).Insert(4, "/").Insert(7, "/"));
            if (dateTime.ToShortDateString() != datetimeTarg.ToShortDateString())
                return false;
            if (isFind && dateTime > datetimeTarg)
                return true;
            strJyo = cCommon.JyoCord(mDMData.id.JyoCD);
            if (strJyo == "")
                return false;
            strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
            rowTarget = cCSV.getDataRow(strShortJyo,
                int.Parse(mDMData.id.RaceNum));
            numUma = int.Parse(cCSV.getData(rowTarget, 4));
            for (int i = 0; i < numUma; i++)
            {
                if (int.Parse(mDMData.DMInfo[cnt].Umaban) == 0 ||
                    int.Parse(mDMData.DMInfo[cnt].Umaban) > 18)
                    break;
                listDM.Add(new classDM(int.Parse(mDMData.DMInfo[cnt].Umaban),
                    int.Parse(mDMData.DMInfo[cnt].DMTime)));
                cnt++;
            }
            listDM.Sort((a, b) => a.DMTime - b.DMTime);
            // 書き込み
            for (int i = 0; i < numUma; i++)
            {
                if(listDM[i].Umaban <= 18)
                {
                    cCSV.setData(rowTarget + 1 + listDM[i].Umaban, 9, 
                        (i + 1).ToString());
                }
            }

            return false;

        }

        void setDataHR(ClassCSV cCSV, string retbuff, DateTime datetimeTarg)
        {
            DateTime dateTime;
            string strShortJyo;
            string strJyo;
            string tmp;
            long rowTarget;
            int res;

            JVData_Struct.JV_HR_PAY mHrData =
                new JVData_Struct.JV_HR_PAY();
            mHrData.SetDataB(ref retbuff);
            dateTime = DateTime.Parse(
            (mHrData.id.Year +
            mHrData.id.MonthDay).Insert(4, "/").Insert(7, "/"));
            if (dateTime.ToShortDateString() != datetimeTarg.ToShortDateString())
                return;
            string codeName = objCodeConv.GetCodeName("2001",
            mHrData.id.JyoCD, 1);
            strJyo = cCommon.JyoCord(mHrData.id.JyoCD);
            if (strJyo == "")
                return;
            strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
            rowTarget = cCSV.getDataRow(strShortJyo,
                int.Parse(mHrData.id.RaceNum));
            if (int.Parse(mHrData.head.DataKubun) != 2)
                return;
            cCommon.writeHaitouData(cCSV, mHrData, rowTarget);
        }

        void setDataO1(ClassCSV cCSV, string retbuff, DateTime datetimeTarg)
        {
            DateTime dateTime;
            string strShortJyo;
            string strJyo;
            long rowTarget;
            long rowWrite;
            int numUma;
            double odds;
            double oddslow;
            int ninki;

            JVData_Struct.JV_O1_ODDS_TANFUKUWAKU mO1Data =
                new JVData_Struct.JV_O1_ODDS_TANFUKUWAKU();
            mO1Data.SetDataB(ref retbuff);
            dateTime = DateTime.Parse(
            (mO1Data.id.Year +
            mO1Data.id.MonthDay).Insert(4, "/").Insert(7, "/"));
            if (dateTime.ToShortDateString() != datetimeTarg.ToShortDateString())
                return;
            strJyo = cCommon.JyoCord(mO1Data.id.JyoCD);
            if (strJyo == "")
                return;
            strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
            rowTarget = cCSV.getDataRow(strShortJyo,
                int.Parse(mO1Data.id.RaceNum));
            numUma = int.Parse(cCSV.getData(rowTarget, 4));
            for(int i = 0;i < numUma; i++)
            {
                rowWrite = rowTarget + 1 +
                    int.Parse(mO1Data.OddsTansyoInfo[i].Umaban);
                if(mO1Data.OddsTansyoInfo[i].Odds.Contains("*") ||
                    mO1Data.OddsTansyoInfo[i].Odds.Contains("-"))
                    odds = 0;
                else
                    odds = double.Parse(mO1Data.OddsTansyoInfo[i].Odds) / 10;
                cCSV.setData(rowWrite, 5, String.Format("{0:0.0}", odds));
                if (mO1Data.OddsFukusyoInfo[i].OddsLow.Contains("*") ||
                    mO1Data.OddsFukusyoInfo[i].OddsLow.Contains("-"))
                    oddslow = 0;
                else
                    oddslow = double.Parse(mO1Data.OddsFukusyoInfo[i].OddsLow) / 10;
                cCSV.setData(rowWrite, 6, String.Format("{0:0.0}", oddslow));
                if (mO1Data.OddsTansyoInfo[i].Ninki.Contains("*") ||
                    mO1Data.OddsTansyoInfo[i].Ninki.Contains("-"))
                    ninki = 0;
                else
                    ninki = int.Parse(mO1Data.OddsTansyoInfo[i].Ninki);
                cCSV.setData(rowWrite, 10, ninki.ToString());
            }
        }

        void setDataRA(ClassCSV cCSV, string retbuff, DateTime datetimeTarg)
        {
            DateTime dateTime;
            string strShortJyo;
            string strJyo;
            long rowTarget;
            string tmpBaba;
            string tmpTenko;

            JVData_Struct.JV_RA_RACE mRaData =
                new JVData_Struct.JV_RA_RACE();
            mRaData.SetDataB(ref retbuff);
            dateTime = DateTime.Parse(
            (mRaData.id.Year +
            mRaData.id.MonthDay).Insert(4, "/").Insert(7, "/"));
            if (dateTime.ToShortDateString() != datetimeTarg.ToShortDateString())
                return;
            strJyo = cCommon.JyoCord(mRaData.id.JyoCD);
            if (strJyo == "")
                return;
            strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
            if (strShortJyo == "")
                return;
            rowTarget = cCSV.getDataRow(strShortJyo,
                int.Parse(mRaData.id.RaceNum));
            // 発走時刻
            cCSV.setData(rowTarget, 5,
                mRaData.HassoTime.Substring(0, 2) + ":" +
                 mRaData.HassoTime.Substring(2, 2));
            // 馬場状態
            if (cCSV.getData(rowTarget, 9).Contains("ダート"))
                tmpBaba = cCommon.BabaCord(mRaData.TenkoBaba.DirtBabaCD.Substring(0, 1));
            else
                tmpBaba = cCommon.BabaCord(mRaData.TenkoBaba.SibaBabaCD.Substring(0, 1));
            if (tmpBaba != "")
                tmpBaba = tmpBaba.Substring(0, 1);
            if (tmpBaba != "未設定")
                cCSV.setData(rowTarget, 13, tmpBaba);
            // 天候
            tmpTenko = cCommon.TenkoCord(mRaData.TenkoBaba.TenkoCD);
            if (tmpTenko != "未設定")
                cCSV.setData(rowTarget, 14, tmpTenko);
            if (cCSV.getData(rowTarget, 13).Contains("(暫定)"))
                cCSV.setData(rowTarget, 13, cCSV.getData(rowTarget, 13).Replace("(暫定)", ""));
            if (cCSV.getData(rowTarget, 14).Contains("(暫定)"))
                cCSV.setData(rowTarget, 14, cCSV.getData(rowTarget, 14).Replace("(暫定)", ""));

        }

        bool setDataSE(ClassCSV cCSV, string retbuff, DateTime datetimeTarg, bool isFind)
        {
            DateTime dateTime;
            string strShortJyo;
            string strJyo;
            long rowTarget;
            long rowWrite;
            long rowadj;
            int zogensa;

            JVData_Struct.JV_SE_RACE_UMA mSeData =
                new JVData_Struct.JV_SE_RACE_UMA();
            mSeData.SetDataB(ref retbuff);
            System.Diagnostics.Debug.WriteLine((mSeData.id.Year +
            mSeData.id.MonthDay).Insert(4, "/").Insert(7, "/"));
            dateTime = DateTime.Parse(
            (mSeData.id.Year +
            mSeData.id.MonthDay).Insert(4, "/").Insert(7, "/"));
            if (isFind && dateTime > datetimeTarg)
                return true;
            if (dateTime.ToShortDateString() != datetimeTarg.ToShortDateString())
                return false;
            string codeName = objCodeConv.GetCodeName("2001",
            mSeData.id.JyoCD, 1);
            strJyo = cCommon.JyoCord(mSeData.id.JyoCD);
            if (strJyo == "")
                return false;
            strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
            if (strShortJyo == "")
                return false;
            rowTarget = cCSV.getDataRow(strShortJyo,
                int.Parse(mSeData.id.RaceNum));
            // 馬体重
            rowadj = long.Parse(mSeData.Umaban) + 1;
            cCSV.setData(rowTarget + rowadj, 29, mSeData.BaTaijyu);
            if (mSeData.ZogenSa.Contains(" "))
                zogensa = 0;
            else
                zogensa = int.Parse(mSeData.ZogenSa);
            if (zogensa == 999)
                cCSV.setData(rowTarget + rowadj, 30, "0");
            else if (zogensa == 0)
                cCSV.setData(rowTarget + rowadj, 30, "0");
            else
                cCSV.setData(rowTarget + rowadj, 30,
                    mSeData.ZogenFugo + String.Format("{0, 2}", zogensa));
            // 着順
            if (int.Parse(mSeData.head.DataKubun) >= 5)
            {
                rowWrite = rowTarget + 1 + long.Parse(mSeData.Umaban);
                cCSV.setData(rowWrite, 11, (int.Parse(mSeData.KakuteiJyuni)).ToString());
            }
               
            return false;

        }
    }
}
