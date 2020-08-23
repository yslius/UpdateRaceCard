using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace UpdateRaceCard
{
    public class ClassCSV
    {
        public string dataCsvAll;
        public string[] linedataCsvAll;
        public string[,] arrdataCsvAll;

        public void createCSVarrdata()
        {
            string[] arrdataCsv;
            int colmax = 0;
            int cnt1 = 0;
            int cnt2 = 0;
            foreach (string item in linedataCsvAll)
            {
                arrdataCsv = item.Split(',');
                if (colmax < arrdataCsv.Length)
                    colmax = arrdataCsv.Length;
            }
            if (colmax < 31)
                colmax = 31;
            arrdataCsvAll = new string[linedataCsvAll.Length, colmax];
            //arrdataCsvAll = new string[linedataCsvAll.Length, 37];

            foreach (string item1 in linedataCsvAll)
            {
                arrdataCsv = item1.Split(',');
                cnt2 = 0;
                foreach (string item2 in arrdataCsv)
                {
                    arrdataCsvAll[cnt1, cnt2] = item2;
                    cnt2++;
                }
                cnt1++;
            }
        }

        public void createCSVdataAll()
        {
            string linedataCsv;
            for (int i = 0; i < arrdataCsvAll.GetLength(0); i++)
            {
                linedataCsv = "";
                for (int j = 0; j < arrdataCsvAll.GetLength(1); j++)
                {
                    if (j >= 30 && arrdataCsvAll[i, j] == null)
                        break;
                    linedataCsv += arrdataCsvAll[i, j] + ",";
                }
                linedataCsv = linedataCsv.Substring(0, linedataCsv.Length - 1);
                dataCsvAll += linedataCsv + "\r\n";
            }
        }


        public void setData(long indRow, long indCol, string InputData)
        {
            //string[] arrdataCsv;
            //string[] arrdatadataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] {"\r\n"}, StringSplitOptions.None);
            //arrdatadataCsv = arrdataCsv[indRow - 1].Split(',');
            //if(indCol > arrdatadataCsv.Length)
            //{
            //    Array.Resize(ref arrdatadataCsv, (int)indCol);
            //}
            //arrdatadataCsv[indCol - 1] = InputData;
            //arrdataCsv[indRow - 1] = string.Join(",", arrdatadataCsv);
            //dataCsvAll = string.Join("\r\n", arrdataCsv);
            
            arrdataCsvAll[indRow - 1, indCol - 1] = InputData;

        }

        public string getData(long indRow, long indCol)
        {
            //string[] arrdataCsv;
            //string[] arrdatadataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //arrdatadataCsv = arrdataCsv[indRow - 1].Split(',');
            //return arrdatadataCsv[indCol - 1];

            return arrdataCsvAll[indRow - 1, indCol - 1];
        }
        public long getDataMaxRow()
        {
            //string[] arrdataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //return arrdataCsv.Length - 1;

            return arrdataCsvAll.GetLength(0);
        }



        public long getDataRow(string strShortJyo, int racenum)
        {
            //string[] arrdataCsv;
            //string[] arrdatadataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //for (long i = 0;i < arrdataCsv.Length; i++)
            //{
            //    arrdatadataCsv = arrdataCsv[i].Split(',');
            //    if(arrdatadataCsv[2].Contains(strShortJyo) &&
            //        int.Parse(arrdatadataCsv[5]) == racenum)
            //    {
            //        return i+1;
            //    }
            //}

            //return 0;

            for (int i = 0; i < arrdataCsvAll.GetLength(0); i++)
            {
                if (arrdataCsvAll[i, 2].Contains(strShortJyo) &&
                    int.Parse(arrdataCsvAll[i, 5]) == racenum)
                    return i + 1;
            }
            return 0;

        }
    }
}
