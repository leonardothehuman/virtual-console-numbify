using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw
{
    public enum Console
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
    public class VirtualConsoleOptionsManager
    {
        public readonly ObservableCollection<KeyValuePair<Console, string>> supportedConsoles = new ObservableCollection<KeyValuePair<Console, string>>();
        public readonly Dictionary<Console, string> extensions = new Dictionary<Console, string>();
        public readonly string defaultExtensions = "Nes / Famicom rom file (*.nes)|*.nes|Snes / Super Famicom rom file (*.smc; *.sfc)|*.smc;*.sfc|Nintendo 64 rom file (*.n64; *.v64; *.z64)|*.n64;*.v64;*.z64|Mega Drive rom file (*.bin; *.gen; *.smd)|*.bin;*.gen;*.smd|Master System rom file (*.sms)|*.sms|PC Engine rom file (*.pce)|*.pce|All files (*.*)|*.*";
        public VirtualConsoleOptionsManager()
        {
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.NES, "Nintendo Entertainment System"));
            extensions.Add(Console.NES, "Nes / Famicom rom file (*.nes)|*.nes|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.SNES, "Super Nintendo Entertainment System"));
            extensions.Add(Console.SNES, "Snes / Super Famicom rom file (*.smc; *.sfc)|*.smc;*.sfc|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.N64, "Nintendo 64"));
            extensions.Add(Console.N64, "Nintendo 64 rom file (*.n64; *.v64; *.z64)|*.n64;*.v64;*.z64|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.SMD, "Sega Mega Drive"));
            extensions.Add(Console.SMD, "Mega Drive rom file (*.bin; *.gen; *.smd; *.md)|*.bin;*.gen;*.smd;*.md|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.SMS, "Sega Master System"));
            extensions.Add(Console.SMS, "Master System rom file (*.sms)|*.sms|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.PCE, "PC Engine"));
            extensions.Add(Console.PCE, "PC Engine rom file (*.pce)|*.pce|All files (*.*)|*.*");
            //supportedConsoles.Add(new KeyValuePair<Console, string>(Console.PCECD, "PC Engine CD"));
            supportedConsoles.Add(new KeyValuePair<Console, string>(Console.NGAES, "Neo-Geo AES"));
            extensions.Add(Console.NGAES, "");
            //supportedConsoles.Add(new KeyValuePair<Console, string>(Console.C64, "Commodore 64"));
        }
    }
}
