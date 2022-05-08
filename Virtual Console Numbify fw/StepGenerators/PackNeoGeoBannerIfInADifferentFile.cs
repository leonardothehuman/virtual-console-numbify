using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class PackNeoGeoBannerIfInADifferentFile
    {
        public static VirtualConsoleInjectionStep generate()
        {
            VirtualConsoleInjectionStep packZeroSix = PackZeroFiveGenerator.generate("00000006");
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will pack 00000006.app";
            toReturn.pauseFinishedMessage = "00000006.app packing finnished";
            bool needsCleanUp = false;
            toReturn.milestoneList = packZeroSix.milestoneList;
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                if (env.workingNeoGeoBannerContainer == "") return;
                if (env.workingNeoGeoBannerContainer == env.workingExtracted05){
                    env.workingNeoGeoBannerContainer = "";
                    return;
                }
                await packZeroSix.process(env, com);
                env.workingNeoGeoBannerContainer = "";
                needsCleanUp = true;
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await packZeroSix.errorCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await packZeroSix.preEverythingCleanup(env, com);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                env.workingNeoGeoBannerContainer = "";
                if (needsCleanUp == false) return;
                await packZeroSix.processCleanup(env, com);
            };
            return toReturn;
        }
    }
}
