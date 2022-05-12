using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Virtual_Console_Numbify_fw.InjectionModels;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class PackExtractedWadGenerator{
        public static VirtualConsoleInjectionStep Generate(){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will pack a wad file";
            toReturn.pauseFinishedMessage = "Wad packing finnished";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await Helpers.CopyFileAsync(
                    Path.Combine(env.AutoinjectwadPath, "common-key.bin"),
                    Path.Combine(new string[]{
                        env.WorkingExtracted, "common-key.bin"
                    })
                );

                com.reportProgress("Packing wad ...", toReturn.milestoneList[0]);
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.AutoinjectwadPath, "wadpacker.exe"),
                    Path.Combine(env.WorkingExtracted),
                    new string[] {
                        env.ExtractedTitleId+".tik",
                        env.ExtractedTitleId+".tmd",
                        env.ExtractedTitleId+".cert",
                        "out.wad",
                        "-sign"
                    }, false, 10
                );


                await Helpers.CopyFileAsync(
                    Path.Combine(new string[]{
                        env.WorkingExtracted, "out.wad"
                    }),
                    Path.Combine(new string[]{
                        env.AutoinjectwadPath, env.FinalWadFile
                    })
                );
            };

            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Directory.Delete(env.WorkingExtracted, true);
            };
            return toReturn;
        }
    }
}
