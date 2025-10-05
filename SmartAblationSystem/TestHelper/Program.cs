
using BootLoader;
using RS232Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelper
{
    class Program
    {
      
        static void Main(string[] args)
        {
            //ASCIIToByteConverter aSCIIToByteConverter = new ASCIIToByteConverter();

            //aSCIIToByteConverter.ReadFile("NA");

            SerialPortManager _spManager = new SerialPortManager();

            _spManager.StartListening();

            //(0x4341fbf1, 0x99a1, 0x435e, { 0x8c, 0x31, 0x25, 0x78, 0x93, 0x1d, 0xc3, 0xd7 })
            _spManager.Write("CCMP1");
        }
    }
}
