using libWiiSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class ExtractWadGenerator
    {
        public static VirtualConsoleInjectionStep generate()
        {
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will extract wad";
            toReturn.pauseFinishedMessage = "Wad extraction finished";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Extracting wad for further customizations ...", toReturn.milestoneList[0]);
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.autoinjectwadPath, "wadunpacker.exe"),
                    env.autoinjectwadPath,
                    new string[] { env.workingWad }
                );
                WAD w = new WAD();
                w.LoadFile(env.workingWad);
                env.extractedTitleId = w.TitleID.ToString("X").PadLeft(16, '0').ToLower();
                env.workingExtracted = Path.Combine(env.autoinjectwadPath, env.extractedTitleId);
                w.Dispose();
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await toReturn.preEverythingCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                File.Delete(env.workingWad);
                env.workingWad = "";
            };
            return toReturn;
        }
    }
}
