using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BootLoader
{
    public class ASCIIToByteConverter
    {
        string[] cMCUData;
        string[] pMCUData;
        string[] repeaterData;
        string[] iCBData;
        string[] catheterData;

        public ASCIIToByteConverter()
        {

        }

        public string[] CMCUData
        {
            get => cMCUData;
            set => cMCUData = value;
        }
        public string[] PMCUData
        {
            get => pMCUData;
            set => pMCUData = value;
        }
        public string[] RepeaterData
        {
            get => repeaterData;
            set => repeaterData = value;
        }
        public string[] ICBData
        {
            get => iCBData;
            set => iCBData = value;
        }
        public string[] CatheterData
        {
            get => catheterData;
            set => catheterData = value;
        }

        public void GetFileFromUSB(string path)
        {

        }

        private string[] FormatLineToRS232Format(string[] line)
        {
            return line;
        }

        public void ReadFile(string path)
        {
            //TO DO: use path

        }

    }
    
}
