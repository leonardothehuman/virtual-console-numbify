using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class PackExtractedWadGenerator
    {
        public static VirtualConsoleInjectionStep generate()
        {
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will pack a wad file";
            toReturn.pauseFinishedMessage = "Wad packing finnished";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await Helpers.CopyFileAsync(
                    Path.Combine(env.autoinjectwadPath, "common-key.bin"),
                    Path.Combine(new string[]{
                        env.workingExtracted, "common-key.bin"
                    })
                );

                com.reportProgress("Packing wad ...", toReturn.milestoneList[0]);
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.autoinjectwadPath, "wadpacker.exe"),
                    Path.Combine(env.workingExtracted),
                    new string[] {
                        env.extractedTitleId+".tik",
                        env.extractedTitleId+".tmd",
                        env.extractedTitleId+".cert",
                        "out.wad",
                        "-sign"
                    }, false, 10
                );


                await Helpers.CopyFileAsync(
                    Path.Combine(new string[]{
                        env.workingExtracted, "out.wad"
                    }),
                    Path.Combine(new string[]{
                        env.autoinjectwadPath, env.finalWadFile
                    })
                );
            };

            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Directory.Delete(env.workingExtracted, true);
            };
            return toReturn;
        }
    }
}
