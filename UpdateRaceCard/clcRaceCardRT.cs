using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateRaceCard
{
    class clcRaceCardRT
    {
        //private string sid = "Test";
        Form1 _form1;
        //AxJVDTLabLib.AxJVLink _axJVLink1;
        private clsCodeConv objCodeConv;
        //private OperateForm cOperateForm;
        private ClassLog cLog;
        int size = 0;
        int count = 0;
        clcCommon cCommon;

        //public clcRaceCardRT(Form1 form1)
        //public clcRaceCardRT(clcCommon cCommon1, AxJVDTLabLib.AxJVLink axJVLink1)
        public clcRaceCardRT(clcCommon cCommon1, Form1 form1)
        {
            _form1 = form1;
            //_axJVLink1 = axJVLink1;
            //cCommon = new clcCommon(axJVLink1);
            cCommon = cCommon1;
            cLog = new ClassLog();
            objCodeConv = new clsCodeConv();
        }

        public void GetRTDataDetailData(ClassCSV cCSV, DateTime datetimeTarg)
        {
            if (GetRTDataDetailData1(cCSV, datetimeTarg) < 0)
            {
                //_axJVLink1.JVClose();
                _form1.axJVLink1.JVClose();
                return;
            }
            _form1.prgDownload.Value = 51;
            _form1.prgDownload.Value--;
            if (GetRTDataDetailData2(cCSV, datetimeTarg) < 0)
            {
                //_axJVLink1.JVClose();
                _form1.axJVLink1.JVClose();
                return;
            }
            _form1.prgDownload.Maximum++;
            _form1.prgDownload.Value = _form1.prgDownload.Maximum;
            _form1.prgDownload.Maximum--;
        }

        int GetRTDataDetailData1(ClassCSV cCSV, DateTime datetimeTarg)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
            string strDate =
                (datetimeTarg - timeSpan).ToString("yyyyMMdd");
            string retbuff;
            long cntLoop = 0;

            size = 40000;
            count = 256;

            try
            {
                _form1.prgJVRead.Value = 0;
                _form1.prgJVRead.Maximum = 100;
                do
                {
                    retbuff = cCommon.loopJVRead(size, count, false);
                    if (retbuff == "" || retbuff == "END")
                        break;
                    if (retbuff.Substring(0, 2) == "WE")
                    {
                        setDataWE(cCSV, retbuff, datetimeTarg);
                    }
                    if (retbuff.Substring(0, 2) == "TC")
                    {
                        setDataTC(cCSV, retbuff, datetimeTarg);
                    }
                    cntLoop++;
                }
                while (cntLoop <= 10000);

                _form1.prgJVRead.Maximum++;
                _form1.prgJVRead.Value =
                    _form1.prgJVRead.Maximum;
                _form1.prgJVRead.Maximum--;

            }
            catch (Exception ex)
            {
                cLog.writeLog("[GetRTDataDetailData1]エラー：" +
                    ex.Message);
            }

            int retJVClose = _form1.axJVLink1.JVClose();
            if (retJVClose != 0)
            {
                cLog.writeLog("[GetRTDataDetailData1]JVClose エラー：" +
                    retJVClose);
            }
            return 1;
        }

        int GetRTDataDetailData2(ClassCSV cCSV, DateTime datetimeTarg)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 3, 0);
            string strJyo;
            string codeJyo;
            string tmp;
            int tmpint;
            string numRace;
            int numUma;
            long rowTarget;
            long rowMax;

            _form1.prgJVRead.Value = 0;

            rowTarget = 2;
            rowMax = cCSV.getDataMaxRow();
            _form1.prgJVRead.Maximum = (int)rowMax;
            while (rowTarget < rowMax)
            {
                // 場コードの特定
                tmp = cCSV.getData(rowTarget, 3);
                tmp = tmp.Substring(1, 1);
                strJyo = cCommon.ShortJyo2Jyo(tmp);
                codeJyo = cCommon.JyogyakuCord(strJyo);
                // レース番号
                tmpint = int.Parse(cCSV.getData(rowTarget, 6));
                numRace = String.Format("{0:D2}", tmpint);
                // 頭数
                numUma = int.Parse(cCSV.getData(rowTarget, 4));
                operateLoop(cCSV, datetimeTarg, codeJyo, numRace, rowTarget);

                // 速報オッズ（単複枠）の呼び出し
                operateOne(cCSV, datetimeTarg, codeJyo, numRace, numUma, rowTarget, "0B31");

                // 速報タイム型データマイニング予想の呼び出し
                operateOne(cCSV, datetimeTarg, codeJyo, numRace, numUma, rowTarget, "0B13");

                // 速報馬体重の呼び出し
                operateOne(cCSV, datetimeTarg, codeJyo, numRace, numUma, rowTarget, "0B11");

                _form1.prgJVRead.Value = (int)rowTarget;
                _form1.prgJVRead.Value--;

                rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;

            }
            // すべて終了
            _form1.prgJVRead.Maximum++;
            _form1.prgJVRead.Value =
                _form1.prgJVRead.Maximum;
            _form1.prgJVRead.Maximum--;

            int retJVClose = _form1.axJVLink1.JVClose();
            if (retJVClose != 0)
            {
                cLog.writeLog("[GetPlaceInfoX]JVClose エラー：" +
                    retJVClose);
            }
            return 1;
        }

        bool operateLoop(ClassCSV cCSV, DateTime datetimeTarg, string codeJyo, string numRace,
            long rowTarget)
        {
            string retbuff;
            long cntLoop = 0;

            if (cCommon.checkInit() != 0)
                return false;
            if (!cCommon.isJVOpenReal("0B12",
                    datetimeTarg.ToString("yyyyMMdd") + codeJyo + numRace))
            {
                //_axJVLink1.JVClose();
                _form1.axJVLink1.JVClose();
                return true;
            }
            do
            {
                retbuff = cCommon.loopJVRead(size, count, false);
                if (retbuff == "" || retbuff == "END")
                    break;
                if (retbuff.Substring(0, 2) == "SE")
                {
                    setDataSE(cCSV, retbuff, rowTarget);
                }
                if (retbuff.Substring(0, 2) == "HR")
                {
                    setDataHR(cCSV, retbuff, rowTarget);
                }
                cntLoop++;
            }
            while (cntLoop <= 10000);

            _form1.axJVLink1.JVClose();

            return true;
        }

        bool operateOne(ClassCSV cCSV, DateTime datetimeTarg, string codeJyo, string numRace,
            int numUma, long rowTarget, string strID)
        {
            string retbuff;
            if (cCommon.checkInit() != 0)
                return false;
            if (!cCommon.isJVOpenReal(strID,
                    datetimeTarg.ToString("yyyyMMdd") + codeJyo + numRace))
            {
                _form1.axJVLink1.JVClose();
                return true;
            }
            retbuff = cCommon.loopJVRead(size, count, false);
            if (retbuff == "" || retbuff == "END")
            {
                //_axJVLink1.JVClose();
                _form1.axJVLink1.JVClose();
                return true;
            }
            if (strID == "0B31")
            {
                setData0B31(cCSV, retbuff, rowTarget, numUma);
            }
            else if (strID == "0B13")
            {
                setData0B13(cCSV, retbuff, rowTarget, numUma);
            }
            else if (strID == "0B11")
            {
                setData0B11(cCSV, retbuff, rowTarget, numUma);
            }

            //_axJVLink1.JVClose();
            _form1.axJVLink1.JVClose();
            
            return true;
        }


        void setData0B31(ClassCSV cCSV, string retbuff, long rowTarget, int numUma)
        {
            long rowWrite;
            double odds;
            double oddslow;
            int ninki;

            JVData_Struct.JV_O1_ODDS_TANFUKUWAKU mO1Data =
                new JVData_Struct.JV_O1_ODDS_TANFUKUWAKU();
            mO1Data.SetDataB(ref retbuff);
            for (int i = 0; i <= numUma - 1; i++)
            {
                rowWrite = rowTarget + 1 + 
                    int.Parse(mO1Data.OddsTansyoInfo[i].Umaban);
                if (mO1Data.OddsTansyoInfo[i].Odds.Contains("*") ||
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

        void setData0B13(ClassCSV cCSV, string retbuff, long rowTarget, int numUma)
        {
            List<classDM> listDM = new List<classDM>();
            int cnt = 0;

            JVData_Struct.JV_DM_INFO mDMData =
                new JVData_Struct.JV_DM_INFO();
            mDMData.SetDataB(ref retbuff);
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
                if (listDM[i].Umaban <= 18)
                {
                    cCSV.setData(rowTarget + 1 + listDM[i].Umaban, 9,
                        (i + 1).ToString());
                }
            }
        }

        void setData0B11(ClassCSV cCSV, string retbuff, long rowTarget, int numUma)
        {
            List<classDM> listDM = new List<classDM>();
            int cnt = 0;
            int zogensa;

            JVData_Struct.JV_WH_BATAIJYU mWHData =
                new JVData_Struct.JV_WH_BATAIJYU();
            mWHData.SetDataB(ref retbuff);
            // 馬体重
            for (long i = rowTarget + 2; i <= rowTarget + numUma + 1; i++)
            {
                cCSV.setData(i, 29, mWHData.BataijyuInfo[cnt].BaTaijyu);
                if (mWHData.BataijyuInfo[cnt].ZogenSa.Contains(" "))
                    zogensa = 0;
                else
                    zogensa = int.Parse(mWHData.BataijyuInfo[cnt].ZogenSa);
                if (zogensa == 999)
                    cCSV.setData(i, 30, "0");
                else if (zogensa == 0)
                    cCSV.setData(i, 30, "0");
                else
                    cCSV.setData(i, 30,
                        mWHData.BataijyuInfo[cnt].ZogenFugo + 
                        String.Format("{0, 2}", zogensa));
                cnt++;
            }
        }


        void setDataSE(ClassCSV cCSV, string retbuff, long rowTarget)
        {
            long rowWrite;

            JVData_Struct.JV_SE_RACE_UMA mSeData =
                new JVData_Struct.JV_SE_RACE_UMA();
            mSeData.SetDataB(ref retbuff);
            if(int.Parse(mSeData.head.DataKubun) == 3 ||
                int.Parse(mSeData.head.DataKubun) == 4 ||
                int.Parse(mSeData.head.DataKubun) == 5 ||
                int.Parse(mSeData.head.DataKubun) == 6 ||
                int.Parse(mSeData.head.DataKubun) == 7)
            {
                rowWrite = rowTarget + 1 + int.Parse(mSeData.Umaban);
                cCSV.setData(rowWrite, 11, 
                    (int.Parse(mSeData.KakuteiJyuni)).ToString());
            }
        }
        void setDataHR(ClassCSV cCSV, string retbuff, long rowTarget)
        {
            JVData_Struct.JV_HR_PAY mHrData =
                new JVData_Struct.JV_HR_PAY();
            mHrData.SetDataB(ref retbuff);
            if (!(int.Parse(mHrData.head.DataKubun) == 1 ||
                int.Parse(mHrData.head.DataKubun) == 2))
                return;
            cCommon.writeHaitouData(cCSV, mHrData, rowTarget);
        }

        void setDataWE(ClassCSV cCSV, string retbuff, DateTime datetimeTarg)
        {
            DateTime timeHappyo;
            DateTime timeRace;
            string strShortJyo;
            string strJyo;
            string tmp;
            string tmpTenko = "";
            string tmpSiba = "";
            string tmpDirt = "";
            string tmpBaba = "";
            long rowTarget;

            JVData_Struct.JV_WE_WEATHER mWEData =
                new JVData_Struct.JV_WE_WEATHER();
            mWEData.SetDataB(ref retbuff);
            if (int.Parse(mWEData.HenkoID) == 2)
            {
                strJyo = cCommon.JyoCord(mWEData.id.JyoCD);
                if (strJyo == "")
                    return;
                strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
                rowTarget = 2;
                timeHappyo = new DateTime(datetimeTarg.Year,
                    datetimeTarg.Month,
                    datetimeTarg.Day,
                    int.Parse(mWEData.HappyoTime.Hour), 
                    int.Parse(mWEData.HappyoTime.Minute),
                    0);
                while (rowTarget < cCSV.getDataMaxRow())
                {
                    timeRace = new DateTime(datetimeTarg.Year,
                    datetimeTarg.Month,
                    datetimeTarg.Day,
                    int.Parse(cCSV.getData(rowTarget, 5).Substring(0, 2)),
                    int.Parse(cCSV.getData(rowTarget, 5).Substring(3, 2)),
                    0);
                    tmp = int.Parse(mWEData.id.Kaiji).ToString() +
                        strShortJyo +
                        int.Parse(mWEData.id.Nichiji).ToString();
                    if(cCSV.getData(rowTarget, 3) == tmp &&
                        timeHappyo < timeRace)
                    {
                        tmpTenko = cCommon.TenkoCord(mWEData.TenkoBaba.TenkoCD);
                        break;
                    }
                    rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
                }
                rowTarget = 2;
                while (rowTarget < cCSV.getDataMaxRow())
                {
                    tmp = int.Parse(mWEData.id.Kaiji).ToString() +
                        strShortJyo +
                        int.Parse(mWEData.id.Nichiji).ToString();
                    if (cCSV.getData(rowTarget, 3) == tmp &&
                        cCSV.getData(rowTarget, 14) != tmpTenko)
                    {
                        cCSV.setData(rowTarget, 14, tmpTenko);
                    }
                    rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
                }
            }
            if (int.Parse(mWEData.HenkoID) == 3)
            {
                strJyo = cCommon.JyoCord(mWEData.id.JyoCD);
                if (strJyo == "")
                    return;
                strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
                rowTarget = 2;
                timeHappyo = new DateTime(datetimeTarg.Year,
                    datetimeTarg.Month,
                    datetimeTarg.Day,
                    int.Parse(mWEData.HappyoTime.Hour),
                    int.Parse(mWEData.HappyoTime.Minute),
                    0);
                while (rowTarget < cCSV.getDataMaxRow())
                {
                    timeRace = new DateTime(datetimeTarg.Year,
                    datetimeTarg.Month,
                    datetimeTarg.Day,
                    int.Parse(cCSV.getData(rowTarget, 5).Substring(0, 2)),
                    int.Parse(cCSV.getData(rowTarget, 5).Substring(3, 2)),
                    0);
                    tmp = int.Parse(mWEData.id.Kaiji).ToString() +
                        strShortJyo +
                        int.Parse(mWEData.id.Nichiji).ToString();
                    if (cCSV.getData(rowTarget, 3) == tmp &&
                        timeHappyo < timeRace)
                    {
                        tmpSiba = cCommon.BabaCord(mWEData.TenkoBaba.SibaBabaCD)
                            .Substring(0, 1);
                        tmpDirt = cCommon.BabaCord(mWEData.TenkoBaba.DirtBabaCD)
                            .Substring(0, 1);
                        break;
                    }
                    rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
                }
                rowTarget = 2;
                while (rowTarget < cCSV.getDataMaxRow())
                {
                    tmpBaba = "";
                    if (cCSV.getData(rowTarget, 9).Contains("ダート"))
                        tmpBaba = tmpDirt;
                    else
                        tmpBaba = tmpSiba;
                    tmp = int.Parse(mWEData.id.Kaiji).ToString() +
                        strShortJyo +
                        int.Parse(mWEData.id.Nichiji).ToString();
                    if (cCSV.getData(rowTarget, 3) == tmp &&
                        cCSV.getData(rowTarget, 13) != tmpBaba)
                    {
                        cCSV.setData(rowTarget, 13, tmpBaba);
                    }
                    rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
                }
            }
        }

        void setDataTC(ClassCSV cCSV, string retbuff, DateTime datetimeTarg)
        {
            string strShortJyo;
            string strJyo;
            string tmp;
            string tmpTime;
            long rowTarget;

            JVData_Struct.JV_TC_INFO mTCData =
                new JVData_Struct.JV_TC_INFO();
            mTCData.SetDataB(ref retbuff);
            strJyo = cCommon.JyoCord(mTCData.id.JyoCD);
            if (strJyo == "")
                return;
            strShortJyo = cCommon.Jyo2ShortJyo(strJyo);
            rowTarget = 2;
            while (rowTarget < cCSV.getDataMaxRow())
            {
                tmp = int.Parse(mTCData.id.Kaiji).ToString() +
                    strShortJyo +
                    int.Parse(mTCData.id.Nichiji).ToString();
                tmpTime = int.Parse(mTCData.TCInfoAfter.Ji).ToString() +
                    ":" + mTCData.TCInfoAfter.Fun;
                if (cCSV.getData(rowTarget, 3) == tmp &&
                    int.Parse(cCSV.getData(rowTarget, 6)) == 
                    int.Parse(mTCData.id.RaceNum))
                {
                    if(cCSV.getData(rowTarget, 5) != tmpTime)
                    {
                        cCSV.setData(rowTarget, 5, tmpTime);
                        break;
                    }
                    
                }
                rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
            }
        }

    }
}
