using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class ReplaceSMSRom{
        public static VirtualConsoleInjectionStep Generate(string romFilePath){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will inject master system rom";
            toReturn.pauseFinishedMessage = "Master system rom injection finished";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Injecting SMS rom ...", toReturn.milestoneList[0]);
                string[] allFiles = Directory.GetFiles(env.WorkingExtractedCcf);
                foreach (string file in allFiles){
                    if (file.ToLower().EndsWith(".sms.zlib")){
                        File.Delete(file);
                        await Helpers.CopyFileAsync(romFilePath, file);
                    }
                }
            };

            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {};
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {};
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {};
            return toReturn;
        }
    }
}
