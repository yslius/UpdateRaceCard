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

        public void setData(long indRow, long indCol, string InputData)
        {
            if(indRow == 419 && indCol == 6)
            {
                System.Diagnostics.Debug.WriteLine(InputData);
            }
            string[] arrdataCsv;
            string[] arrdatadataCsv;
            arrdataCsv = dataCsvAll.Split(new[] {"\r\n"}, StringSplitOptions.None);
            arrdatadataCsv = arrdataCsv[indRow - 1].Split(',');
            if(indCol > arrdatadataCsv.Length)
            {
                Array.Resize(ref arrdatadataCsv, (int)indCol);
            }
            arrdatadataCsv[indCol - 1] = InputData;
            arrdataCsv[indRow - 1] = string.Join(",", arrdatadataCsv);
            dataCsvAll = string.Join("\r\n", arrdataCsv);
        }

        public string getData(long indRow, long indCol)
        {
            string[] arrdataCsv;
            string[] arrdatadataCsv;
            arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            arrdatadataCsv = arrdataCsv[indRow - 1].Split(',');
            return arrdatadataCsv[indCol - 1];
        }
        public long getDataMaxRow()
        {
            string[] arrdataCsv;
            arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            return arrdataCsv.Length - 1;
        }



        public long getDataRow(string strShortJyo, int racenum)
        {
            string[] arrdataCsv;
            string[] arrdatadataCsv;
            arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            for (long i = 0;i < arrdataCsv.Length; i++)
            {
                arrdatadataCsv = arrdataCsv[i].Split(',');
                if(arrdatadataCsv[2].Contains(strShortJyo) &&
                    int.Parse(arrdatadataCsv[5]) == racenum)
                {
                    return i+1;
                }
            }
            
            return 0;
        }


    }
}
