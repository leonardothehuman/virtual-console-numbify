using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virtual_Console_Numbify_fw.InjectionModels;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class PackZeroFiveGenerator{
        public static VirtualConsoleInjectionStep Generate(string overrideFile = "00000005"){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will pack " + overrideFile + ".app";
            toReturn.pauseFinishedMessage = overrideFile + ".app packed";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Repacking " + overrideFile + @".app ...", toReturn.milestoneList[0]);
                File.Move(
                    Path.Combine(new string[] { env.WorkingExtracted, overrideFile + ".app" }),
                    Path.Combine(new string[] { env.WorkingExtracted, overrideFile + ".old.app" })
                );
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.U8tool, "U8Tool.exe"),
                    env.U8tool,
                    new string[] {
                        "-file", "\""+Path.Combine(new string[]{ env.WorkingExtracted, overrideFile+".app" })+"\"",
                        "-folder", "\""+Path.Combine(new string[]{ env.WorkingExtracted, overrideFile+@"_app_OUT\" })+"\"",
                        "-source", "\""+Path.Combine(new string[]{ env.WorkingExtracted, overrideFile+".old.app" })+"\"",
                        "-pack"
                    },
                    true
                );
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Directory.Delete(Path.Combine(new string[] { env.WorkingExtracted, overrideFile + @"_app_OUT\" }), true);
                if (overrideFile == "00000005"){
                    env.WorkingExtracted05 = "";
                }
                File.Delete(Path.Combine(new string[] { env.WorkingExtracted, overrideFile+".old.app" }));
            };
            return toReturn;
        }
    }
}
