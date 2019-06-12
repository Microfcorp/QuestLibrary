using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace QuestLibrary.QuestFiles.Sign
{
    public class Sign
    {
        /* ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT ProcessorId FROM Win32_Processor");
                    */
        private const string SecretKey = "MicQuestLibrarySCMICCQLQM";

        public static byte[] GetSign
        {
            get
            {
                return Crc32.CRC32(SecretKey);
            }
        }

        public static bool CorrectSign(byte[] Sign)
        {
            if (Sign.Length != 4)
                return false;
            if (Crc32.IsValidCRC(GetSign, Sign))
                return true;
            return false;
        }
    }
}
