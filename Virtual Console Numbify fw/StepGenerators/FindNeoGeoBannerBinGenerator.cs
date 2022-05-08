using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class FindNeoGeoBannerBinGenerator
    {
        public static VirtualConsoleInjectionStep generate()
        {
            VirtualConsoleInjectionStep extractZeroSix = ExtractZeroFiveGenerator.generate("00000006");
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will find neogeo's banner.bin";
            toReturn.pauseFinishedMessage = "banner.bin search finished";
            toReturn.milestoneList = extractZeroSix.milestoneList;
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                if (File.Exists(
                    Path.Combine(
                        env.workingExtracted05,
                        "banner.bin"
                    )
                )){
                    env.workingNeoGeoBannerContainer = env.workingExtracted05;
                    return;
                }
                await extractZeroSix.process(env, com);
                env.workingNeoGeoBannerContainer = Path.Combine(
                    new string[] { env.workingExtracted, @"00000006_app_OUT\" }
                );
                if (File.Exists(
                    Path.Combine(
                        env.workingNeoGeoBannerContainer,
                        "banner.bin"
                    )
                )){
                    return;
                }
                //await com.showFrontendMessage("Will delete 6", "6", RecipeButtonsType.ok);
                Directory.Delete(Path.Combine(new string[] { env.workingExtracted, @"00000006_app_OUT\" }), true);
                env.workingNeoGeoBannerContainer = "";
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await extractZeroSix.errorCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await extractZeroSix.preEverythingCleanup(env, com);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await extractZeroSix.processCleanup(env, com);
            };
            return toReturn;
        }
    }
}
