using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Virtual_Console_Numbify_fw{
    internal class InjectionEnviorunment{
        public string ExternalToolsBasePath = Path.Combine(Helpers.GetExeDirectory(), "externalTools");
        //enviorunment.ExternalToolsBasePath = @"C:\Users\Leonardo\Desktop";
        public string WorkingWad;
        public string WorkingExtracted;
        public string WorkingExtracted05;
        public string WorkingExtractedCcf;
        public string WorkingExtractedCcf2;
        public string WorkingNeoGeoBannerContainer = "";
        public string ExtractedTitleId;
        public string FinalWadFile;
        public Console console;
        public string AutoinjectwadPath {
            get{
                return Path.Combine(ExternalToolsBasePath, "autoinjectwad");
            }
        }
        public string DevilkenInjectorPath{
            get{
                return Path.Combine(ExternalToolsBasePath, "VC");
            }
        }
        public string VCbrlytPath{
            get{
                return Path.Combine(ExternalToolsBasePath, @"HowardC_Tools\VCbrlyt9.0");
            }
        }
        public string VCiconPath{
            get{
                return Path.Combine(ExternalToolsBasePath, @"HowardC_Tools\VCIcon8.0");
            }
        }
        public string VCsaveInjectPath{
            get{
                return Path.Combine(ExternalToolsBasePath, @"HowardC_Tools\VCSaveInject5.0");
            }
        }
        public string U8tool{
            get{
                return Path.Combine(ExternalToolsBasePath, @"HowardC_Tools\u8tool10.1");
            }
        }
        public string Ccftool{
            get{
                return Path.Combine(ExternalToolsBasePath, @"HowardC_Tools\ccftool2.0");
            }
        }
        public string ZeroFourApp{
            get{
                return Path.Combine(ExternalToolsBasePath, @"00000004.app");
            }
        }
    }
}
