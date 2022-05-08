using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Virtual_Console_Numbify_fw{
    internal class InjectionEnviorunment{
        public string externalToolsBasePath;
        public string workingWad;
        public string workingExtracted;
        public string workingExtracted05;
        public string workingExtractedCcf;
        public string workingExtractedCcf2;
        public string workingNeoGeoBannerContainer = "";
        public string extractedTitleId;
        public string finalWadFile;
        public Console console;
        public string autoinjectwadPath {
            get{
                return Path.Combine(externalToolsBasePath, "autoinjectwad");
            }
        }
        public string devilkenInjectorPath{
            get{
                return Path.Combine(externalToolsBasePath, "VC");
            }
        }
        public string VCbrlytPath{
            get{
                return Path.Combine(externalToolsBasePath, @"HowardC_Tools\VCbrlyt9.0");
            }
        }
        public string VCiconPath{
            get{
                return Path.Combine(externalToolsBasePath, @"HowardC_Tools\VCIcon8.0");
            }
        }
        public string VCsaveInjectPath{
            get{
                return Path.Combine(externalToolsBasePath, @"HowardC_Tools\VCSaveInject5.0");
            }
        }
        public string U8tool{
            get{
                return Path.Combine(externalToolsBasePath, @"HowardC_Tools\u8tool10.1");
            }
        }
        public string ccftool{
            get{
                return Path.Combine(externalToolsBasePath, @"HowardC_Tools\ccftool2.0");
            }
        }
        public string zeroFourApp{
            get{
                return Path.Combine(externalToolsBasePath, @"00000004.app");
            }
        }
    }
}
