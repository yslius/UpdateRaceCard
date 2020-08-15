using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

class clsCodeConv
{
    // コード名称取得モジュール
    // コードから名称を取得する
    private struct mudtCodeLine
    {
        public string strCodeNo;
        public string strCode;
        public string strNames;
    }

    private string mFileName;
    private mudtCodeLine[] mArrData;
    private bool blnFlag;

    // @(f)
    //
    // 機能　　 : データの格納
    //
    // 引き数　 : ARG1 - ファイル名
    //
    // 返り値　 : なし
    //
    // 機能説明 : 指定されたファイルのデータをメモリ上に格納する
    //
    public string FileName
    {
        set
        {
            try
            {
                mFileName = value;
                SetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    // @(f)
    //
    // 機能　　 : 名称の取得
    //
    // 引き数　 : ARG1 - コードNo.
    // 　　　　   ARG2 - コード
    //
    // 返り値　 : 名称
    //
    // 機能説明 : メモリ上に格納したデータをコードにより検索し名称を取得する
    //
    public string GetCodeName(string strCodeNo, string strCode, short intNo = 1)
    {
        int i;               // ループカウンタ
        int j;               // ループカウンタ
        int ct;              // 名称取得用カウンタ
        string strName = ""; // 名称

        // データが読み込めていない場合
        if (!blnFlag) return "";

        try
        {
            // 名称文字列から指定番目の名称を返す
            for (i = 0; i < mArrData.Length; i++)
            {
                if (mArrData[i].strCodeNo == strCodeNo && mArrData[i].strCode == strCode)
                {
                    ct = 1;
                    for (j = 0; j < mArrData[i].strNames.Length; j++)
                    {
                        if (mArrData[i].strNames.Substring(j, 1) == ",")
                        {
                            ct = ct + 1;
                            if (ct > intNo) break;
                        }
                        else if (ct == intNo)
                        {
                            strName = strName + mArrData[i].strNames.Substring(j, 1);
                        }
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        return strName;
    }

    // @(f)
    //
    // 機能　　 : データの開放
    //
    // 引き数　 : なし
    //
    // 返り値　 : なし
    //
    // 機能説明 : メモリ上に格納したデータを開放する
    //
    //~clsCodeConv()
    //{
    //    try
    //    {
    //        Array.Clear(mArrData, 0, mArrData.Length);
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show(ex.Message);
    //    }
    //}

    // @(f)
    //
    // 機能　　 : データを1行ずつ処理
    //
    // 引き数　 : なし
    //
    // 返り値　 : なし
    //
    // 機能説明 : CSVデータを1行分ずつ区切って処理する
    //
    private void SetData()
    {
        string strRt;     // 改行文字
        int lngLnRt;      // 改行文字の文字数
        string strData;   // CSVファイルを受ける文字列
        int lnglenData;   // strDataの文字数
        int lngRt;        // strData中のstrRtの位置
        int lngCt;        // Rtのカウンタ，mArrDataの行数
        int lngBeforeRt;  // ひとつ前のlngRt
        string strLine;   // CSVファイル一行分
        byte[] bytData;   // ファイルのデータ格納先

        blnFlag = true;

        try
        {
            // 改行文字の決定
            strRt = "\r\n";
            lngLnRt = strRt.Length;

            // ファイルの中身を文字列として取得
            System.IO.FileStream fs = new System.IO.FileStream(mFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            bytData = new byte[fs.Length];
            fs.Read(bytData, 0, bytData.Length);
            fs.Close();

            // エンコード
            strData = System.Text.Encoding.GetEncoding(932).GetString(bytData);

            // 配列クリア
            Array.Clear(bytData, 0, bytData.Length);

            lnglenData = strData.Length;

            // 中身が空，もしくはファイルが存在しない場合
            if (lnglenData == 0)
            {
                blnFlag = false;
                return;
            }

            // 一行ずつ処理
            lngBeforeRt = -lngLnRt; // 一行目の前に改行があると仮定
            lngRt = -1;
            lngCt = 0;
            while (lngRt + lngLnRt < lnglenData)
            {
                lngRt = strData.Substring(lngRt + 1).IndexOf(strRt) + lngRt + 1;
                if (lngRt == 0) break;
                Array.Resize(ref mArrData, lngCt + 1);
                strLine = strData.Substring(lngBeforeRt + lngLnRt, lngRt - lngBeforeRt - lngLnRt);
                SetLine(ref strLine, ref lngCt);
                lngCt = lngCt + 1;
                lngBeforeRt = lngRt;
            }
        }
        catch (Exception ex)
        {
            blnFlag = false;
            MessageBox.Show(ex.Message);
        }
    }

    // @(f)
    //
    // 機能　　 : 配列に格納
    //
    // 引き数　 : ARG1 - 一行分の文字列
    // 　　　　 : ARG2 - 現在の行番号
    //
    // 返り値　 : なし
    //
    // 機能説明 : 1行分を構造体に変換して配列に格納する
    //
    private void SetLine(ref string strLine, ref int lngCt)
    {
        byte bytFieldCt;          //フィールド（列）数
        string strDelimiter;      //区切り子
        int lngDelimiter;         //区切り子の位置
        int lngBeforeDel;         //前の区切り子の位置
        string strWord;           //フィールド1つ分の文字列
        mudtCodeLine udtWords;    //一行分のstrWordを格納
        udtWords = new mudtCodeLine();

        //区切り子の決定
        strDelimiter = ",";
        try
        {
            //ユーザ定義型mudtCodeLineに変換
            bytFieldCt = 0;
            lngDelimiter = -1;
            lngBeforeDel = -1;
            while (bytFieldCt <= 2)
            {
                if (bytFieldCt < 2)
                {
                    lngDelimiter = strLine.Substring(lngDelimiter + 1).IndexOf(strDelimiter) + lngDelimiter + 1;
                }
                else
                {
                    lngDelimiter = strLine.Length;
                }

                // フィールドが2以下の場合
                if (lngDelimiter == 0)
                {
                    MessageBox.Show("CSVファイルが不正です");
                    blnFlag = false;
                    return;
                }

                strWord = strLine.Substring(lngBeforeDel + 1, lngDelimiter - lngBeforeDel - 1);

                switch (bytFieldCt)
                {
                    case 0:
                        udtWords.strCodeNo = strWord;
                        break;
                    case 1:
                        udtWords.strCode = strWord;
                        break;
                    case 2:
                        udtWords.strNames = strWord;
                        break;
                    default:
                        return;
                }

                bytFieldCt = (byte)(bytFieldCt + 1);
                lngBeforeDel = lngDelimiter;
            }

            // ユーザ定義型mudtCodeLineを配列に代入
            mArrData[lngCt] = udtWords;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}