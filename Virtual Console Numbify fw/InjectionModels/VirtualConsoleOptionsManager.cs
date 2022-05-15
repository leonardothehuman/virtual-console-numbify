using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.InjectionModels {
    public enum GameConsole{
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
    public class VirtualConsoleOptionsManager{
        public readonly ObservableCollection<KeyValuePair<GameConsole, string>> supportedConsoles = new ObservableCollection<KeyValuePair<GameConsole, string>>();
        public readonly Dictionary<GameConsole, string> extensions = new Dictionary<GameConsole, string>();
        public readonly string defaultExtensions = "Nes / Famicom rom file (*.nes)|*.nes|Snes / Super Famicom rom file (*.smc; *.sfc)|*.smc;*.sfc|Nintendo 64 rom file (*.z64)|*.z64|Mega Drive rom file (*.bin; *.gen; *.smd)|*.bin;*.gen;*.smd|Master System rom file (*.sms)|*.sms|PC Engine rom file (*.pce)|*.pce|All files (*.*)|*.*";
        public VirtualConsoleOptionsManager(){
            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.NES, "Nintendo Entertainment System"));
            extensions.Add(GameConsole.NES, "Nes / Famicom rom file (*.nes)|*.nes|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.SNES, "Super Nintendo Entertainment System"));
            extensions.Add(GameConsole.SNES, "Snes / Super Famicom rom file (*.smc; *.sfc)|*.smc;*.sfc|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.N64, "Nintendo 64"));
            extensions.Add(GameConsole.N64, "Nintendo 64 rom file (*.z64)|*.z64|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.SMD, "Sega Mega Drive"));
            extensions.Add(GameConsole.SMD, "Mega Drive rom file (*.bin; *.gen; *.smd; *.md)|*.bin;*.gen;*.smd;*.md|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.SMS, "Sega Master System"));
            extensions.Add(GameConsole.SMS, "Master System rom file (*.sms)|*.sms|All files (*.*)|*.*");

            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.PCE, "PC Engine"));
            extensions.Add(GameConsole.PCE, "PC Engine rom file (*.pce)|*.pce|All files (*.*)|*.*");
            //supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.PCECD, "PC Engine CD"));
            supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.NGAES, "Neo-Geo AES"));
            extensions.Add(GameConsole.NGAES, "");
            //supportedConsoles.Add(new KeyValuePair<GameConsole, string>(GameConsole.C64, "Commodore 64"));
        }
    }
}
