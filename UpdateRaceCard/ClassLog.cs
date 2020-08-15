using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UpdateRaceCard
{
    class ClassLog
    {
        public ClassLog()
        {
            if (!Directory.Exists("log"))
            {
                Directory.CreateDirectory("log");
            }
        }

        public void writeLog(string strLog)
        {
            string appendText;
            appendText = DateTime.Now.ToString("HH:mm:ss") + " " +
                strLog + Environment.NewLine;

            System.IO.File.AppendAllText("log\\" +
                                     DateTime.Now.ToString("yyMMdd") +
                                     "_log.txt", appendText);
        }

    }
}
