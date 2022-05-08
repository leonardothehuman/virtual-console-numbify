using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify
{
    enum Console
    {
        NES = 0,
        SNES = 1,
        N64 = 2,
        SMD = 3,
        SMS = 4,
        PCE = 5,
        PCECD = 6,
        NGAES = 7,
        C64 = 8,
    }
    internal class VirtualConsoleOptionsManager{
        public readonly ObservableCollection<KeyValuePair<Console, string>> supportedConsoles = new();
        public VirtualConsoleOptionsManager(){
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.NES, "Nintendo Entertainment System"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.SNES, "Super Nintendo Entertainment System"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.N64, "Nintendo 64"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.SMD, "Sega Mega Drive"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.SMS, "Sega Master System"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.PCE, "PC Engine"));
            //supportedConsoles.Add(new KeyValuePair<Console, string>(Console.PCECD, "PC Engine CD"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.NGAES, "Neo-Geo AES"));
            //supportedConsoles.Add(new KeyValuePair<Console, string>(Console.C64, "Commodore 64"));
        }
    }
}
