using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows.Forms;

namespace UpdateRaceCard
{
    public class OperateForm
    {
        Form1 _form1;
        public OperateForm(Form1 form1)
        {
            _form1 = form1;
        }
        public void enableButton()
        {
            _form1.button1.Enabled = true;
            _form1.btnGetJVData.Enabled = true;
        }

        public void disableButton()
        {
            _form1.button1.Enabled = false;
            _form1.btnGetJVData.Enabled = false;
        }

        public void readFolder()
        {
            CommonOpenFileDialog commonOpenFileDialog =
                new CommonOpenFileDialog("保存するフォルダを選択してください");
            commonOpenFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            commonOpenFileDialog.IsFolderPicker = true;
            if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            _form1.textBox1.Text = commonOpenFileDialog.FileName + "\\";
            string pathDoc = commonOpenFileDialog.FileName + "\\01出馬表.csv";

            if (!File.Exists(pathDoc))
            {
                MessageBox.Show("フォルダ内に出馬表が見つかりませんでした。", "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public void readDate()
        {
            _form1.rtbData.Text = "";
            CommonOpenFileDialog commonOpenFileDialog =
                new CommonOpenFileDialog("保存するフォルダを選択してください");
            //commonOpenFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            commonOpenFileDialog.IsFolderPicker = true;
            if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            _form1.textBox1.Text = commonOpenFileDialog.FileName + "\\";
            string pathDoc = commonOpenFileDialog.FileName + "\\01出馬表.csv";

            if (File.Exists(pathDoc))
            {
                bool flag = false;
                try
                {
                    int cnt = 0;
                    StreamReader streamReader =
                        new StreamReader(pathDoc, Encoding.GetEncoding("shift_jis"));
                    while (cnt <= 2)
                    {
                        string[] strArray = streamReader.ReadLine().Split(',');
                        cnt++;
                        if (cnt != 2)
                            continue;
                        DateTime dateTime = DateTime.Parse(strArray[0]);
                        _form1.rtbData.Text =
                            "フォルダ内の出馬表から日付を読み取りました。 " +
                            dateTime.ToLongDateString();
                        flag = true;
                    }
                    streamReader.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("別のプロセスで"))
                    {
                        MessageBox.Show("出馬表を閉じてください。", "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        MessageBox.Show(ex.Message, "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    return;

                }
                if (flag)
                    return;
                _form1.rtbData.Text = "フォルダ内の出馬表から日付を読み取れませんでした。";
            }
            else
                _form1.rtbData.Text = "フォルダ内に出馬表が見つかりませんでした。";

        }
    }
}
