using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class ExtractZeroFiveGenerator{
        public static VirtualConsoleInjectionStep generate(string overrideFile = "00000005"){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will extract " + overrideFile + ".app";
            toReturn.pauseFinishedMessage = overrideFile + ".app extracted";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Extracting " + overrideFile + ".app ...", toReturn.milestoneList[0]);
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.U8tool, "U8Tool.exe"),
                    env.U8tool,
                    new string[] {
                        "-file", "\""+Path.Combine(new string[]{env.workingExtracted, overrideFile+".app" })+"\"",
                        "-folder", "\""+Path.Combine(new string[]{ env.workingExtracted, overrideFile+@"_app_OUT\" })+"\"",
                        "-source", "\""+Path.Combine(new string[]{ env.workingExtracted, overrideFile+".app" })+"\"",
                        "-extract"
                    },
                    true
                );
                if (overrideFile == "00000005"){
                    env.workingExtracted05 = Path.Combine(
                        new string[] { env.workingExtracted, overrideFile + @"_app_OUT\" }
                    );
                }
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
            };
            return toReturn;
        }
    }
}
