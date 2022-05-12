using libWiiSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Virtual_Console_Numbify_fw.InjectionModels;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class ExtractWadGenerator{
        public static VirtualConsoleInjectionStep Generate(){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will extract wad";
            toReturn.pauseFinishedMessage = "Wad extraction finished";
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Extracting wad for further customizations ...", toReturn.milestoneList[0]);
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.AutoinjectwadPath, "wadunpacker.exe"),
                    env.AutoinjectwadPath,
                    new string[] { env.WorkingWad }
                );
                WAD w = new WAD();
                w.LoadFile(env.WorkingWad);
                env.ExtractedTitleId = w.TitleID.ToString("X").PadLeft(16, '0').ToLower();
                env.WorkingExtracted = Path.Combine(env.AutoinjectwadPath, env.ExtractedTitleId);
                w.Dispose();
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await toReturn.preEverythingCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                File.Delete(env.WorkingWad);
                env.WorkingWad = "";
            };
            return toReturn;
        }
    }
}
