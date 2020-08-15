using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace UpdateRaceCard
{
    public class clcRaceCard
    {
        Form1 _form1;
        private OperateForm cOperateForm;
        private ClassLog cLog;
        private ClcRaceCardStock cRaceCardStock;
        private clcRaceCardRT cRaceCardRT;
        ClassCSV cCSV;

        string[] arrAddHead = { "単勝配当", "1着複勝配当", "2着複勝配当",
            "3着複勝配当", "枠連配当", "馬連配当",
            "馬単配当", "3連複配当", "3連単配当",
            "ワイド1", "ワイド1配当", "ワイド2",
            "ワイド2配当", "ワイド3", "ワイド3配当" };

        public clcRaceCard(Form1 form1)
        {
            _form1 = form1;
            cOperateForm = new OperateForm(form1);
            cLog = new ClassLog();
            cRaceCardStock = new ClcRaceCardStock(form1);
            cRaceCardRT = new clcRaceCardRT(form1);
            cCSV = new ClassCSV();
        }

        public void update()
        {
            cLog.writeLog("update");
            cOperateForm.disableButton();

            DateTime datetimeTarg;
            string pathTarg;
            string pathFileR;
            List<string> listRcsv;

            pathTarg = _form1.textBox1.Text;

            // 出馬表の読み込み
            pathFileR = GetRaceCardFile(pathTarg);
            if (pathFileR == "")
            {
                cOperateForm.enableButton();
                return;
            }

            // 出馬表の読み込み
            //listRcsv = ReadCSV(pathFileR);

            var encoding = Encoding.GetEncoding("shift_jis");
            cCSV.dataCsvAll = File.ReadAllText(pathFileR, encoding);

            string tmp;
            tmp = cCSV.getData(2, 1);
            datetimeTarg = DateTime.Parse(tmp);

            // 追加項目を記入
            //listRcsv = writeHeadData(cCSV);
            writeHeadData(cCSV);

            // 速報開催情報(一括)の呼び出し
            int retval = 0;
            retval = checkJVRTOpen(datetimeTarg);
            if (retval < -1)
                return;

            if (retval == -1)
            {
                cRaceCardStock.GetStockDataDetailData(cCSV, datetimeTarg);
            }
            else
            {
                cRaceCardRT.GetRTDataDetailData(cCSV, datetimeTarg);
            }

            deleteZanteiData(cCSV);

            // ファイル出力
            File.WriteAllText(pathFileR, cCSV.dataCsvAll, encoding);

            _form1.rtbData.Text = datetimeTarg.ToShortDateString() + 
                " 出馬表更新完了しました。";

            _form1.axJVLink1.JVClose();
            System.Media.SystemSounds.Asterisk.Play();
            cOperateForm.enableButton();

        }
        public string GetRaceCardFile(string pathTarg)
        {
            string path = pathTarg + "01出馬表.csv";
            if (!File.Exists(path))
            {
                MessageBox.Show("出馬表が見つかりません。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return "";
            }
            return path;
        }

        List<string> ReadCSV(string pathTarg)
        {
            List<string> lists = new List<string>();
            var encoding = Encoding.GetEncoding("shift_jis");
            StreamReader sr = new StreamReader(pathTarg,
                Encoding.GetEncoding("Shift_JIS"));
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    lists.Add(line);
                }
            }
            sr.Close();

            return lists;
        }


        private int checkJVRTOpen(DateTime datetimeTarg)
        {
            string dataspec = "0B14";
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
            string strDate =
                (datetimeTarg - timeSpan).ToString("yyyyMMdd");

            int num2 = _form1.axJVLink1.JVClose();
            if (num2 != 0)
                MessageBox.Show("JVClose エラー：" + num2);
            int num1 = _form1.axJVLink1.JVRTOpen(dataspec, strDate);

            return num1;
        }

        void writeHeadData(ClassCSV cCSV)
        {
            long rowTarget = 2;
            while (rowTarget < cCSV.getDataMaxRow())
            {
                cCSV.setData(rowTarget + 1, 29, "馬体重");
                cCSV.setData(rowTarget + 1, 30, "増減");
                rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
            }

        }

        void deleteZanteiData(ClassCSV cCSV)
        {
            long rowTarget = 2;
            string tmp;
            DateTime datecheck;
            while (rowTarget < cCSV.getDataMaxRow())
            {
                tmp = cCSV.getData(2, 1) + " " + cCSV.getData(rowTarget, 5);
                datecheck = DateTime.Parse(tmp);
                if(DateTime.Now > datecheck)
                {
                    if(cCSV.getData(rowTarget - 1, 13).Contains("(暫定)"))
                    {
                        cCSV.setData(rowTarget - 1, 13, 
                            cCSV.getData(rowTarget - 1, 13).Replace("(暫定)", ""));
                    }
                    if (cCSV.getData(rowTarget - 1, 14).Contains("(暫定)"))
                    {
                        cCSV.setData(rowTarget - 1, 14,
                            cCSV.getData(rowTarget - 1, 14).Replace("(暫定)", ""));
                    }
                    if (cCSV.getData(rowTarget, 13).Contains("(暫定)"))
                    {
                        cCSV.setData(rowTarget, 13,
                            cCSV.getData(rowTarget, 13).Replace("(暫定)", ""));
                    }
                    if (cCSV.getData(rowTarget, 14).Contains("(暫定)"))
                    {
                        cCSV.setData(rowTarget, 14,
                            cCSV.getData(rowTarget, 14).Replace("(暫定)", ""));
                    }
                }

                rowTarget += long.Parse(cCSV.getData(rowTarget, 4)) + 3;
            }

        }

        


    }
}
