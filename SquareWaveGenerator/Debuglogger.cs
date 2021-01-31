using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareWaveGenerator
{
    class Debuglogger
    {
        public string strfilename = @"C:\GRL\AppData\Debug.txt";
        public void CreateFile()
        {
            if (File.Exists(strfilename))
            {
                try { File.Delete(strfilename); }
                catch (Exception ex)
                {

                }
            }
        }
        public void UdpateDebugLogger(byte[] databytes)
        {
            string strPayloaddata = BitConverter.ToString(databytes, 0, databytes.Length);
            strPayloaddata = strPayloaddata.Replace("-", " 0x");
            using (StreamWriter stre = new StreamWriter(strfilename, true))
            {
                stre.WriteLine(strPayloaddata);
            }
        }
    }
}
