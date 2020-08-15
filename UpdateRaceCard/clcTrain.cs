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
    public class clsTrain
    {
        Form1 _form1;
        private OperateForm cOperateForm;
        string[] arrAddTrain = { "検出数", "調教状態", "補足", "調教指数", "暗号データ", "暗号データ" };

        public clsTrain(Form1 form1)
        {
            _form1 = form1;
            cOperateForm = new OperateForm(form1);
        }
        public void ReflectTrainMain()
        {
            cOperateForm.disableButton();
            //_form1.rtbData.Text = "";

            //Console.WriteLine(_form1.textBox2.Text);
            string pathTarg;
            string pathFileT;
            string pathFileR;
            List<string> listTcsv;
            List<string> listRcsv;

            pathTarg = _form1.textBox1.Text;

            // 調教データの読み込み
            pathFileT = getTrainDataFile(pathTarg);
            if (pathFileT == "")
            {
                cOperateForm.enableButton();
                return;
            }

            // 出馬表と調教ファイルの日付が合っているかチェック
            if (!checkDateTrainData(pathFileT, pathTarg))
            {
                cOperateForm.enableButton();
                return;
            }

            // 出馬表の読み込み
            pathFileR = GetRaceCardFile(pathTarg);
            if (pathFileR == "")
            {
                cOperateForm.enableButton();
                return;
            }

            // 調教データの読み込み
            listTcsv = ReadCSV(pathFileT);

            // 出馬表の読み込み
            listRcsv = ReadCSV(pathFileR);

            // 追加項目を記入
            //listRcsv = WriteAddHeadData(listRcsv);

            // 追加項目を記入
            listRcsv = CopyTraindataToRacecard(listTcsv, listRcsv);

            // ファイル出力
            OutputCsv(pathFileR, listRcsv);

            //_form1.rtbData.Text = "調教データ反映完了しました。";

            cOperateForm.enableButton();

        }

        List<string> CopyTraindataToRacecard(List<string> listTcsv, List<string> listRcsv)
        {
            int rowTargetR;
            int rowTargetT;
            string strJyoT;
            string strShortJyoT;
            int numRaceT;
            int numUma;

            rowTargetR = 1;
            do
            {
                string[] arrlineR = listRcsv[rowTargetR].Split(',');
                numUma = int.Parse(arrlineR[3]);
                rowTargetT = 0;
                do
                {
                    string[] arrlineT = listTcsv[rowTargetT].Split(',');
                    strJyoT = arrlineT[0].Substring(0, 2);
                    strShortJyoT = Jyo2ShortJyo(strJyoT);
                    numRaceT = int.Parse(arrlineT[0].Substring(2, 2));
                    if (arrlineR[2].Substring(1, 1) == strShortJyoT &&
                        int.Parse(arrlineR[5]) == numRaceT)
                    {
                        listRcsv = copyTraindataToRacecardDetail(listTcsv, rowTargetT,
                            listRcsv, rowTargetR + 1, numUma);
                    }

                    rowTargetT += 22;
                } while (rowTargetT < listTcsv.Count);

                rowTargetR += numUma + 3;
            } while (rowTargetR < listRcsv.Count);

            return listRcsv;
        }

        List<string> copyTraindataToRacecardDetail(List<string> listTcsv, int rowTargetT,
            List<string> listRcsv, int rowTargetR, int numUma)
        {
            string strtmp;
            for (int i = 0; i <= numUma; i++)
            {
                strtmp = listTcsv[rowTargetT + i];
                string[] arrT = strtmp.Split(',');
                string[] arrR = listRcsv[rowTargetR + i].Split(',');
                if (arrR.Length != 30)
                {
                    Array.Resize(ref arrR, 30);
                }

                string[] arrlist = new string[arrT.Length + arrR.Length];
                Array.Copy(arrR, arrlist, arrR.Length);
                Array.Copy(arrT, 0, arrlist, arrR.Length, arrT.Length);

                listRcsv[rowTargetR + i] = String.Join(",", arrlist);

            }

            return listRcsv;
        }


        string Jyo2ShortJyo(string cvt)
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
        string getTrainDataFile(string pathTarg)
        {
            string path = "";
            string[] listfiles = Directory.GetFiles(pathTarg, "*", SearchOption.AllDirectories);

            foreach (string strfile in listfiles)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(strfile, @"c\d{8}.csv"))
                {
                    Console.WriteLine(strfile);
                    path = strfile;
                    break;
                }
            }

            return path;
        }

        bool checkDateTrainData(string pathfileT, string pathTarg)
        {
            string strDateFolder;
            string strDateFile;
            string strTmp;

            strTmp = pathTarg.Substring(pathTarg.Length - 7, 6);
            strTmp = strTmp.Replace("月", "");
            strTmp = strTmp.Replace("日", "");
            strTmp = Strings.StrConv(strTmp, VbStrConv.Narrow);
            strDateFolder = strTmp;

            strTmp = pathfileT.Substring(pathfileT.Length - 8, 8);
            strTmp = strTmp.Replace(".csv", "");
            strDateFile = strTmp;

            if (strDateFolder != strDateFile)
            {
                MessageBox.Show("調教要約ファイルとフォルダの日付が異なります｡", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }

            return true;
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
                    //string[] values = line.Split(',');
                    //lists.AddRange(values);
                    lists.Add(line);
                }
            }
            sr.Close();

            return lists;
        }

        List<string> WriteAddHeadData(List<string> listcsv)
        {
            string[] arrAddTrain = new string[7];
            Microsoft.Office.Interop.Excel.Application appExl = null;
            Workbook workbook1 = null;
            Worksheet worksheet1 = null;

            string pathThis = System.Windows.Forms.Application.StartupPath + "\\";
            string nameBook = "Template.xlsx";
            string nameSheet = "Template";
            appExl = new Microsoft.Office.Interop.Excel.Application();
            workbook1 = appExl.Workbooks.Open(
                Path.GetFullPath(pathThis + nameBook));
            worksheet1 = workbook1.Sheets[nameSheet];

            for (int i = 0; i <= 6; i++)
                arrAddTrain[i] = worksheet1.Cells[2, i + 152].value;


            appExl.DisplayAlerts = false;
            workbook1.Close(System.Type.Missing, System.Type.Missing, System.Type.Missing);
            appExl.DisplayAlerts = true;
            appExl.Quit();

            List<string> lists = new List<string>();
            string strlist;
            foreach (string list in listcsv)
            {
                string[] arrlist = list.Split(',');
                if (arrlist[0] == "馬名S")
                {
                    if (arrlist.Length == 37)
                    {
                        for (int i = 0; i <= 6; i++)
                        {
                            arrlist[30 + i] = arrAddTrain[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= 6; i++)
                        {
                            Array.Resize(ref arrlist, arrlist.Length + 1);
                            arrlist[arrlist.Length - 1] = arrAddTrain[i];
                        }
                    }

                }
                strlist = String.Join(",", arrlist);
                lists.Add(strlist);
            }
            return lists;
        }

        void OutputCsv(string pathFileR, List<string> listTcsv)
        {
            var encoding = Encoding.GetEncoding("shift_jis");
            File.WriteAllLines(pathFileR, listTcsv, encoding);
        }
    }
}
